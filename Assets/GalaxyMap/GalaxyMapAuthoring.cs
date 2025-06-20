using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public class GalaxyMapAuthoring : MonoBehaviour
    {
        public class Baker : Baker<GalaxyMapAuthoring>
        {
            public override void Bake(GalaxyMapAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new EnableGalaxyMapFlag());
            }
        }
    }
}