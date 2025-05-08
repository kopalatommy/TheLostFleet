using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.RTSCore;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    // Handles starting pathFinding
    // Handles selecting hexagons

    // The plan for this system is to be the glue that ties a bunch of
    // different systems together that do not know about each other. For
    // example, the select units system does not know about the path finding 
    // system, yet the 2 need to work together.
    public partial class SystemMapManagerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<SystemMapEnableFlag>();

            CreateGenerateMapRequest();
        }

        protected void CreateGenerateMapRequest()
        {
            Debug.Log("SystemMapManagerSystem.CreateGenerateMapRequest");

            Entity mapEntity = EntityManager.CreateEntity();

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

            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

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
                receiveShadows: false,
                renderingLayerMask: 1);

            // Create an array of mesh and material required for runtime rendering.
            RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { new Material(Shader.Find("Universal Render Pipeline/Lit")) }, new Mesh[] { mesh });

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

        protected override void OnUpdate()
        {
            HighlightCurrentHexagon();
            HandleStartPathFinding();
        }

        protected void HighlightCurrentHexagon()
        {
            
        }

        protected void HandleStartPathFinding()
        {
            foreach ((SelectedTag selected, Entity e) in SystemAPI.Query<SelectedTag>().WithEntityAccess())
            {

            }
        }
    }
}
