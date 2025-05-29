using Unity.Entities;
using Unity.Mathematics;

namespace GalacticBoundStudios.HexTech
{
    public static class HexTechStateUtils
    {
        public static void CreateStateEntity(in EntityManager entityManager)
        {
            Entity e = entityManager.CreateEntity();

            entityManager.AddComponentData(e, new HexMapTransformData
            {
                orientation = HexOrientation.FlatTop(),
                scale = new float2(1, 1),
                origin = new float3()
            });
        }
    }
}