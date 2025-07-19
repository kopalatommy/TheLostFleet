using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using GalacticBoundStudios.HexTech;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public partial class GalaxyMapTileManager : SystemBase
    {
        private EntityQuery notProcessedTilesQuery;

        public static NativeKDMap<HexCoord> NodeMap { get; private set; }
        public static NativeHashMap<HexCoord, Entity> EntityMap { get; private set; }
        public static NativeHashMap<HexCoord, float> MovementCostMap { get; private set; }

        protected override void OnCreate()
        {
            Debug.Log("GalaxyMapTileManager.OnCreate");

            NodeMap = new NativeKDMap<HexCoord>(Allocator.Persistent);
            EntityMap = new NativeHashMap<HexCoord, Entity>(100, Allocator.Persistent);
            MovementCostMap = new NativeHashMap<HexCoord, float>(100, Allocator.Persistent);

            EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp);

            notProcessedTilesQuery = queryBuilder.WithAll<HexCoord>().WithNone<GalaxyMapProcessedTag>().Build(EntityManager);

            EntityQuery updateQuery = queryBuilder.WithAll<EnableGalaxyMapFlag>().WithAll<HexCoord>().WithNone<GalaxyMapProcessedTag>().Build(EntityManager);

            RequireForUpdate(updateQuery);
        }

        protected override void OnUpdate()
        {
            Debug.Log("GalaxyMapTileManager.OnUpdate");

            if (!NodeMap.IsCreated)
            {
                Debug.Log("Not created");
                return;
            }

            NativeKDMap<HexCoord> updated = NodeMap;
            NativeHashMap<HexCoord, Entity> updatedEntityMap = EntityMap;
            NativeHashMap<HexCoord, float> movementCostMap = MovementCostMap;

            NativeArray<Entity> notProcessed = notProcessedTilesQuery.ToEntityArray(Allocator.Temp);

            NativeArray<float3> pointsToAdd = new NativeArray<float3>(notProcessed.Length, Allocator.Temp);
            NativeArray<HexCoord> valuesToAdd = new NativeArray<HexCoord>(notProcessed.Length, Allocator.Temp);

            HexMapTransformData hexMapTransformData = HexMapTransformData.Default;
            for (int i = 0; i < notProcessed.Length; i++)
            {
                valuesToAdd[i] = EntityManager.GetComponentData<HexCoord>(notProcessed[i]);
                float2 point = HexMath.HexToPixel(valuesToAdd[i], in hexMapTransformData);
                pointsToAdd[i] = new float3(point.x, 0, point.y);

                EntityMap.Add(valuesToAdd[i], notProcessed[i]);

                // ToDo, need to add component that tracks movement cost
                movementCostMap[valuesToAdd[i]] = 1;

                updatedEntityMap[valuesToAdd[i]] = notProcessed[i];

                EntityManager.AddComponentData(notProcessed[i], new GalaxyMapProcessedTag());
            }

            updated.AddData(pointsToAdd, valuesToAdd);
            NodeMap = updated;

            MovementCostMap = movementCostMap;
            EntityMap = updatedEntityMap;
        }

        private void OnDestroy(ref SystemState state)
        {
            Debug.Log("GalaxyMapTileManager.OnDestroy");

            NodeMap.Dispose();
            EntityMap.Dispose();
            MovementCostMap.Dispose();

            NodeMap = default;
            EntityMap = default;
            MovementCostMap = default;
        }
    }
}