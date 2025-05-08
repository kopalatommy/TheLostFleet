using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    public partial struct HexTechPathfinderSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HexTechCreatePathRequest>();
        }

        void OnUpdate(ref SystemState state)
        {
            UnityEngine.Debug.Log("HexTechPathfinderSystem.Update");

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            EntityCommandBuffer.ParallelWriter parallelWriter = entityCommandBuffer.AsParallelWriter();

            HexTechCreatePathJob job = new HexTechCreatePathJob()
            {
                entityCommandBuffer = parallelWriter,
                // costMap = HexMapManager.Instance.mapCostData
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);

            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}