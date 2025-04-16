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
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            
            HexTechGeneratePathJob job = new HexTechGeneratePathJob()
            {

            };

            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}