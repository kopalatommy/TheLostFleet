using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using GalacticBoundStudios.HexTech.PathFinding;

namespace GalacticBoundStudios.SpawnPrefabsSystem
{
    public partial struct SpawnPrefabsSystem : ISystem
    {
        private EntityQuery requestQuery;
        private EntityQuery definePrefabQuery;

        private NativeHashMap<int,Entity> prefabMap;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("SpawnPrefabsSystem.OnCreate");

            prefabMap = new NativeHashMap<int, Entity>(100, Allocator.Persistent);

            requestQuery = state.GetEntityQuery(ComponentType.ReadOnly<SpawnPrefabRequest>());
            definePrefabQuery = state.GetEntityQuery(ComponentType.ReadOnly<DeclarePrefabData>());

            //NativeArray<EntityQuery> queries = new NativeArray<EntityQuery>(2, Allocator.Temp);
            //state.RequireAnyForUpdate(queries);
            //queries.Dispose();
        }

        void OnDestroy(ref SystemState state)
        {
            prefabMap.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var (entry, entity) in SystemAPI.Query<DeclarePrefabData>().WithEntityAccess())
            {
                prefabMap[entry.keyHash] = entry.prefab;

                ecb.DestroyEntity(entity);
            }

            foreach (var (request, entity) in SystemAPI.Query<SpawnPrefabRequest>().WithEntityAccess())
            {
                Entity prefab = ecb.Instantiate(prefabMap[request.keyHash]);

                ecb.SetComponent(prefab, new LocalTransform
                {
                    Position = request.position,
                    Rotation = request.rotation,
                    Scale = 1
                });

                ecb.DestroyEntity(entity);

                Debug.Log("Spawning prefab: " + request.keyHash + " @ " + request.position);
            }

            // foreach (var (request, entity) in SystemAPI.Query<ReturnPooledObjectRequest>().WithEntityAccess())
            // {

            // }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
