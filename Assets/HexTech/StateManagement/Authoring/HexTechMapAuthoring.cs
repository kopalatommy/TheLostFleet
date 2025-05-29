using UnityEngine;
using Unity.Entities;
using GalacticBoundStudios.HexTech.MapGeneration;

namespace GalacticBoundStudios.HexTech
{
    public class HexTechMapAuthoring : MonoBehaviour
    {
        public HexMapTransformData mapTransformData;
        public HexHollowData hexHollowData;

        public class Baker : Baker<HexTechMapAuthoring>
        {
            public override void Bake(HexTechMapAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, in authoring.mapTransformData);
                AddComponent(e, in authoring.hexHollowData);
            }
        }
    }
}