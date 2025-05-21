using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using GalacticBoundStudios.HexTech.MapGeneration;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    public partial struct HexTechFollowPathSystem : ISystem
    {
        private EntityQuery movingUnitsQuery;
        private EntityQuery mapTransformDataQuery;

        private void OnCreate(ref SystemState state)
        {
            movingUnitsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<HexTechMapPath>().Build(ref state);
            state.RequireForUpdate(movingUnitsQuery);

            mapTransformDataQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<HexMapTransformData>().Build(ref state);
        }

        private void OnUpdate(ref SystemState state)
        {
            NativeArray<Entity> movingUnits = movingUnitsQuery.ToEntityArray(Allocator.Temp);

            Debug.Log("HexTechFollowPathSystem.OnUpdate: " + movingUnits.Length);

            float deltaTime = state.World.Time.DeltaTime;
            HexMapTransformData hexMapTransformData = mapTransformDataQuery.GetSingleton<HexMapTransformData>();

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            EntityCommandBuffer.ParallelWriter parallelWriter = ecb.AsParallelWriter();

            HexTechFollowPathJob job = new HexTechFollowPathJob
            {
                ecb = parallelWriter,
                mapConfig = hexMapTransformData,
                deltaTime = deltaTime
            };

            state.Dependency = job.Schedule(state.Dependency);
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}