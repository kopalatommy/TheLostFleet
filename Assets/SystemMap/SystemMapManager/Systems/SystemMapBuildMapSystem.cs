using GalacticBoundStudios.EchoesOfTheFarRim.SystemMap;
using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.MeshMania;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.EchoesOfTheFarRim
{
    public struct GenerateSystemMapRequestFlag : IComponentData
    {

    }

    public partial class SystemMapBuildMapSystem : SystemBase
    {
        EntityQuery buildMapRequestQuery;

        protected override void OnCreate()
        {
            buildMapRequestQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<GenerateSystemMapRequestFlag>().Build(this);

            RequireForUpdate(buildMapRequestQuery);
        }

        protected override void OnUpdate()
        {
            // Get all entities matching the query
            NativeArray<Entity> entities = buildMapRequestQuery.ToEntityArray(Allocator.Temp);

            // It is done this way, outside of the jobs and parallelization b/c RenderMeshUtility
            // is required to add the proper render components but can't be used in parallel
            for (int i = 0; i < entities.Length; i++)
            {
                // Entity mapEntity = EntityManager.CreateEntity();
                Entity mapEntity = entities[i];

                EntityManager.RemoveComponent<GenerateSystemMapRequestFlag>(mapEntity);

                EntityManager.AddComponentData(mapEntity, new HexMapTransformData
                {
                    orientation = HexOrientation.FlatTop(),
                    scale = new float2(1, 1),
                    origin = new float3()
                });
                EntityManager.AddComponentData(mapEntity, new HexHollowData
                {
                    isHollow = true,
                    innerRadius = 0.85f
                });

                HexCoord minBounds = new HexCoord { q = int.MaxValue, r = int.MaxValue };
                HexCoord maxBounds = new HexCoord { q = int.MinValue, r = int.MinValue };
                HexagonActivationGrid activationGrid = HexTechMapGenerationUtils.CreateDefaultMap(HexTechGridShape.Hexagon, 15, ref minBounds, ref maxBounds, Allocator.Persistent);

                // HexagonActivationGrid activationGrid = new HexagonActivationGrid
                // {
                //     hexGrid = new NativeHashMap<HexCoord, byte>(100, Allocator.Persistent),
                //     randomSeed = (uint)System.DateTime.Now.Ticks,
                // };
                // HexCoord minBounds = new HexCoord { q = int.MaxValue, r = int.MaxValue };
                // HexCoord maxBounds = new HexCoord { q = int.MinValue, r = int.MinValue };
                // PopulateHexGrid(HexTechGridShape.Hexagon, ref activationGrid, 15, ref minBounds, ref maxBounds);
                EntityManager.AddComponentData(mapEntity, activationGrid);

                RefRW<SystemMapMovementCostData> movementCostData = SystemAPI.GetSingletonRW<SystemMapMovementCostData>();
                AssignMovementCosts(ref activationGrid, movementCostData.ValueRW.Value);

                Mesh mesh = new Mesh();
                mesh.vertices = new[] { Vector3.up * 1000, Vector3.left * 1000, Vector3.right * 1000, -Vector3.up * 1000 };
                mesh.triangles = new[] { 0, 1, 2, 1, 2, 3 };

                EntityManager.AddComponentData(mapEntity, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                EntityManager.AddComponentData(mapEntity, new LocalToWorld
                {
                    Value = float4x4.Translate(new float3(0, 0, 0))
                });

                // Create a RenderMeshDescription using the convenience constructor
                // with named parameters.
                RenderMeshDescription desc = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: false);

                // Create an array of mesh and material required for runtime rendering.
                Material mapMaterial = new Material(Shader.Find("HexTech/HexGridHighlightURPv2"));
                RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { mapMaterial }, new Mesh[] { mesh });

                RenderMeshUtility.AddComponents(
                    mapEntity,
                    EntityManager,
                    desc,
                    renderMeshArray,
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

                EntityManager.AddComponentData(mapEntity, new HexTechCreateMapTag());

                EntityManager.AddComponentData(mapEntity, new MeshIndexData
                {
                    index = mesh.GetInstanceID()
                });
            }
        }

        private void AssignMovementCosts(ref HexagonActivationGrid activationGrid, NativeHashMap<HexCoord, float> movementCostMap)
        {
            NativeArray<HexCoord> mapHexagons = activationGrid.hexGrid.GetKeyArray(Allocator.Temp);

            foreach (HexCoord coord in mapHexagons) {
                movementCostMap.Add(coord, 1.0f);
            }
        }
    }
}