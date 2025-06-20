using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using GalacticBoundStudios.HexTech;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public partial struct GalaxyMapPathFinderSystem : ISystem
    {
        // This struct allows us to run the job in a non-blocking way
        private struct JobContext
        {
            public JobHandle jobHandle;
            public GalaxyMapPathFinderJob job;
        }

        // --- Configuration Settings ---

        const int numParallelJobs = 4;

        // --- Manage active jobs ---
        private NativeList<JobContext> activeJobs;

        // --- Queries ---
        private EntityQuery updateQuery;

        private EntityQuery waitingQuery;

        private void OnCreate(ref SystemState state)
        {
            activeJobs = new NativeList<JobContext>(Allocator.Persistent);

            updateQuery = new EntityQueryBuilder(Allocator.Temp).WithAny<GalaxyMapPathRequest>().WithAny<GalaxyMapPathInProgressTag>().Build(ref state);
            state.RequireForUpdate(updateQuery);

            waitingQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GalaxyMapPathRequest>().WithNone<GalaxyMapPathInProgressTag>().Build(ref state);
        }

        private void OnUpdate(ref SystemState state)
        {
            NativeArray<Entity> requestEntities = waitingQuery.ToEntityArray(Allocator.Temp);
            if (requestEntities.Length > 0)
            {
                for (int i = 0; i < requestEntities.Length && activeJobs.Length < numParallelJobs; i++)
                {
                    StartRequest(requestEntities[i], ref state);
                }
            }

            if (activeJobs.Length > 0)
            {
                for (int i = 0; i < activeJobs.Length; i++)
                {
                    if (ProcessActiveJob(activeJobs[i], ref state))
                    {
                        activeJobs.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void StartRequest(Entity requester, ref SystemState state)
        {
            GalaxyMapPathFinderJob job = new GalaxyMapPathFinderJob
            {
                request = state.EntityManager.GetComponentData<GalaxyMapPathRequest>(requester),
                requester = requester,
                resultPath = new NativeList<HexCoord>(10, Allocator.Persistent)
            };

            state.EntityManager.AddComponentData(requester, new GalaxyMapPathInProgressTag());

            JobHandle jobHandle = job.Schedule();
            activeJobs.Add(new JobContext
            {
                jobHandle = jobHandle,
                job = job
            });
        }

        // returns true if the job is complete and has been processed
        private bool ProcessActiveJob(in JobContext jobContext, ref SystemState state)
        {
            // If the job is not yet complete, do nothing
            if (!jobContext.jobHandle.IsCompleted)
            {
                return false;
            }

            // Call complete on the job to access job data
            jobContext.jobHandle.Complete();

            // Remove data from job/request
            state.EntityManager.RemoveComponent<GalaxyMapPathInProgressTag>(jobContext.job.requester);
            state.EntityManager.RemoveComponent<GalaxyMapPathRequest>(jobContext.job.requester);
            if (state.EntityManager.HasComponent<GalaxyMapPath>(jobContext.job.requester))
            {
                state.EntityManager.RemoveComponent<GalaxyMapPath>(jobContext.job.requester);
            }

            if (jobContext.job.resultPath.IsEmpty)
            {
                Debug.Log("Failed to find path");
            }
            else
            {
                string pathStr = "(start: " + jobContext.job.request.start + ") ";
                DynamicBuffer<GalaxyMapPath> pathBuffer = state.EntityManager.AddBuffer<GalaxyMapPath>(jobContext.job.requester);
                for (int i = 0; i < jobContext.job.resultPath.Length; i++)
                {
                    pathStr += jobContext.job.resultPath[i].ToString() + " -> ";
                    pathBuffer.Add(jobContext.job.resultPath[i]);
                }
                pathStr.Remove(pathStr.Length - 4);

                pathStr += " (end: " + jobContext.job.request.target + ")";
                Debug.Log(pathStr);
            }
            jobContext.job.resultPath.Dispose();

            // The request is finished and has been processed, it can now be deleted
            return true;
        }

        private void OnDestroy(ref SystemState state)
        {

        }
    }
}