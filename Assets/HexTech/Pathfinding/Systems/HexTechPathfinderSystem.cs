using System;
using GalacticBoundStudios.EchoesOfTheFarRim.SystemMap;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    public struct HexTechPathfinderActiveJobsFlag : IComponentData
    {
        
    }

    public struct HexTechPathfinderPathInProgressFlag : IComponentData
    {

    }

    public partial struct HexTechPathfinderSystem : ISystem
    {
        public struct JobInformation
        {
            public JobHandle jobHandle;
            public HexTechCreatePathJobSingle job;
        }

        // --- Configuration Settings ---

        const int numParallelJobs = 4;

        // --- Manage active jobs ---
        private NativeList<JobInformation> activeJobs;

        private bool jobsInitialized;
        private bool jobsScheduledAndRunning;
        private int jobsToProcess;

        // --- Queries ---
        private EntityQuery getPendingRequestsQuery;
        // Get the map cost data
        private EntityQuery getMapCostDataQuery;

        void OnCreate(ref SystemState state)
        {
            jobsInitialized = false;
            jobsScheduledAndRunning = false;
            jobsToProcess = 0;

            activeJobs = new NativeList<JobInformation>(1, Allocator.Persistent);

            Debug.Log("HexTechPathfinderSystem.OnCreate");

            jobsInitialized = true;

            getPendingRequestsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<HexTechCreatePathRequest>().WithNone<HexTechPathfinderPathInProgressFlag>().Build(ref state);
            getMapCostDataQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<SystemMapMovementCostData>().Build(ref state);

            state.RequireAnyForUpdate(new EntityQueryBuilder(Allocator.Persistent).WithAny<HexTechPathfinderActiveJobsFlag, HexTechCreatePathRequest>().Build(ref state));
        }

        void OnDestroy(ref SystemState state)
        {
            activeJobs.Dispose();
        }

        void OnUpdate(ref SystemState state)
        {
            NativeArray<Entity> requestEntities = getPendingRequestsQuery.ToEntityArray(Allocator.TempJob);
            if (requestEntities.Length > 0)
            {
                NativeHashMap<HexCoord, float> costMap = getMapCostDataQuery.GetSingleton<SystemMapMovementCostData>().Value;
                
                // ToDo: Move to separate function
                for (int i = 0; i < requestEntities.Length; i++)
                {
                    StartRequest(requestEntities[i], ref state, ref costMap);
                }
            }

            if (!activeJobs.IsEmpty)
            {
                for (int i = 0; i < activeJobs.Length; i++)
                {
                    if (ProcessActiveJob(activeJobs[i], ref state))
                    {
                        // Remove the job from the list of active jobs
                        activeJobs.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void StartRequest(Entity requester, ref SystemState state, ref NativeHashMap<HexCoord, float> costMap)
        {
            Debug.Log("Creating path request job");

            HexTechCreatePathJobSingle job = new HexTechCreatePathJobSingle
            {
                requester = requester,
                request = state.EntityManager.GetComponentData<HexTechCreatePathRequest>(requester),
                resultPath = new NativeList<HexCoord>(Allocator.Persistent),
                costMap = costMap
            };

            state.EntityManager.AddComponentData(requester, new HexTechPathfinderPathInProgressFlag());

            JobHandle jobHandle = job.Schedule();
            activeJobs.Add(new JobInformation
            {
                jobHandle = jobHandle,
                job = job
            });
        }

        // Returns true if the job is complete and should be removed from the list
        private bool ProcessActiveJob(in JobInformation jobInformation, ref SystemState state)
        {
            // If the job is not yet complete, do nothing
            if (!jobInformation.jobHandle.IsCompleted)
            {
                return false;
            }

            Debug.Log("Find path job is finished");

            // Call complete on the job to access job data
            jobInformation.jobHandle.Complete();

            // Remove data from job/request
            state.EntityManager.RemoveComponent<HexTechPathfinderPathInProgressFlag>(jobInformation.job.requester);
            state.EntityManager.RemoveComponent<HexTechCreatePathRequest>(jobInformation.job.requester);
            if (state.EntityManager.HasComponent<HexTechMapPath>(jobInformation.job.requester))
            {
                state.EntityManager.RemoveComponent<HexTechMapPath>(jobInformation.job.requester);
            }

            // If the path was made, attach it to the entity
            if (jobInformation.job.resultPath.IsEmpty)
            {
                Debug.Log("Failed to find path");
                jobInformation.job.resultPath.Dispose();
            }
            else
            {
                string pathStr = "(start: " + jobInformation.job.request.startPos + ") ";
                DynamicBuffer<HexTechMapPath> pathBuffer = state.EntityManager.AddBuffer<HexTechMapPath>(jobInformation.job.requester);
                for (int i = 0; i < jobInformation.job.resultPath.Length; i++)
                {
                    pathStr += jobInformation.job.resultPath[i].ToString() + " -> ";
                    pathBuffer.Add(jobInformation.job.resultPath[i]);
                }
                pathStr.Remove(pathStr.Length - 4);

                pathStr += " (end: " + jobInformation.job.request.endPos + ")";
                Debug.Log(pathStr);

                // state.EntityManager.AddComponentData(jobInformation.job.requester, new HexTechMapPath
                // {
                //     path = jobInformation.job.resultPath
                // });
            }

            // The request is finished and has been processed, it can now be deleted
            return true;
        }
    }
}