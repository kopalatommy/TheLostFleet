using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

namespace GalacticBoundStudios.Utilities
{
    public struct SpawnPrefabRequest : IComponentData
    {
        public Entity prefab;
        public float3 positon;
        public quaternion rotation;
    }

    public partial struct SpawnPrefabsSystem : ISystem
    {
        private EntityQuery requestQuery;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("SpawnPrefabsSystem.OnCreate");

            state.RequireForUpdate<SpawnPrefabRequest>();

            requestQuery = state.GetEntityQuery(ComponentType.ReadOnly<SpawnPrefabRequest>());
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer();

            foreach (var (request, entity) in SystemAPI.Query<SpawnPrefabRequest>().WithEntityAccess())
            {
                Entity instance = ecb.Instantiate(request.prefab);

                ecb.SetComponent(instance, new LocalTransform()
                {
                    Position = request.positon,
                    Rotation = request.rotation,
                    Scale = 1
                });

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
