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

                HexagonActivationGrid activationGrid = new HexagonActivationGrid
                {
                    hexGrid = new NativeHashMap<HexCoord, byte>(100, Allocator.Persistent),
                    randomSeed = (uint)System.DateTime.Now.Ticks,
                };
                HexCoord minBounds = new HexCoord { q = int.MaxValue, r = int.MaxValue };
                HexCoord maxBounds = new HexCoord { q = int.MinValue, r = int.MinValue };
                PopulateHexGrid(HexTechGridShape.Hexagon, ref activationGrid, 15, ref minBounds, ref maxBounds);
                EntityManager.AddComponentData(mapEntity, activationGrid);

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

        private void PopulateHexGrid(HexTechGridShape gridShape, ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
        {
            switch (gridShape)
            {
                case HexTechGridShape.Hexagon:
                    PopulateHexagonGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.Rectangle:
                    PopulateRectangleGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.Triangle:
                    PopulateTriangleGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.HexagonRing:
                    PopulateHexagonRingGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                    break;
            }
        }

        private void PopulateHexagonGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
        {
            for (int q = -chunkSize; q <= chunkSize; q++)
            {
                for (int r = -chunkSize; r <= chunkSize; r++)
                {
                    if (q + r >= -chunkSize && q + r <= chunkSize)
                    {
                        if (minBounds.q > q)
                        {
                            minBounds.q = q;
                        }
                        if (maxBounds.q < q)
                        {
                            maxBounds.q = q;
                        }

                        if (minBounds.r > r)
                        {
                            minBounds.r = r;
                        }
                        if (maxBounds.r < r)
                        {
                            maxBounds.r = r;
                        }

                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                    }
                }
            }
        }

        private void PopulateRectangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
        {
            for (int q = -chunkSize; q <= chunkSize; q++)
            {
                for (int r = -chunkSize; r <= chunkSize; r++)
                {
                    if (minBounds.q > q)
                    {
                        minBounds.q = q;
                    }
                    if (maxBounds.q < q)
                    {
                        maxBounds.q = q;
                    }

                    if (minBounds.r > r)
                    {
                        minBounds.r = r;
                    }
                    if (maxBounds.r < r)
                    {
                        maxBounds.r = r;
                    }

                    gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                }
            }
        }

        private void PopulateTriangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
        {
            for (int q = 0; q <= chunkSize; q++)
            {
                for (int r = 0; r <= chunkSize - q; r++)
                {
                    if (minBounds.q > q)
                    {
                        minBounds.q = q;
                    }
                    if (maxBounds.q < q)
                    {
                        maxBounds.q = q;
                    }

                    if (minBounds.r > r)
                    {
                        minBounds.r = r;
                    }
                    if (maxBounds.r < r)
                    {
                        maxBounds.r = r;
                    }

                    gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                }
            }
        }

        private void PopulateHexagonRingGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
        {
            for (int q = -chunkSize; q <= chunkSize; q++)
            {
                for (int r = -chunkSize; r <= chunkSize; r++)
                {
                    if (math.abs(q + r) == chunkSize)
                    {
                        if (minBounds.q > q)
                        {
                            minBounds.q = q;
                        }
                        if (maxBounds.q < q)
                        {
                            maxBounds.q = q;
                        }

                        if (minBounds.r > r)
                        {
                            minBounds.r = r;
                        }
                        if (maxBounds.r < r)
                        {
                            maxBounds.r = r;
                        }

                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                    }
                }
            }
        }
    }
}