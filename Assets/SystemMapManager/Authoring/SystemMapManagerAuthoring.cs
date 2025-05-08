using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    public class SystemMapManagerAuthoring : MonoBehaviour
    {
        public class Baker : Baker<SystemMapManagerAuthoring>
        {
            public override void Bake(SystemMapManagerAuthoring authoring)
            {
                Debug.Log("SystemMapManagerAuthoring.Baker.Bake");

                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new SystemMapEnableFlag());
            }
        }
    }
}
