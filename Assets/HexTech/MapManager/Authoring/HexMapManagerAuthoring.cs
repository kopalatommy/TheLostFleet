using UnityEngine;
using Unity.Entities;
using GalacticBoundStudios.HexTech.MapGeneration;

namespace GalacticBoundStudios.HexTech
{
    public class HexMapManagerAuthoring : MonoBehaviour
    {
        public HexMapConfig mapConfig;

        public class Baker : Baker<HexMapManagerAuthoring>
        {
            public override void Bake(HexMapManagerAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, authoring.mapConfig.TransformData);
                AddComponent(e, new HexHollowData
                {
                    isHollow = authoring.mapConfig.hollow,
                    innerRadius = authoring.mapConfig.innerRadius
                });
            }
        }
    }
}