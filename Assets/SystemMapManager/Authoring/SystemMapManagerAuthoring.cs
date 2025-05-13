using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.HexTech.Shaders;
using GalacticBoundStudios.MeshMania;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    public class SystemMapManagerAuthoring : MonoBehaviour
    {
        public class Baker : Baker<SystemMapManagerAuthoring>
        {
            public override void Bake(SystemMapManagerAuthoring authoring)
            {
                Debug.Log("SystemMapManagerAuthoring.Baker.Bake");

                Entity e = GetEntity(TransformUsageFlags.Renderable);

                AddComponent(e, new SystemMapEnableFlag());
                AddComponent(e, new GenerateSystemMapRequestFlag());
                AddComponent(e, new HighlightedAxialCoordsVector4Override
                {
                    Value = new float4(1, 1, 0, 0)
                });

                AddComponent(e, new URPMaterialPropertyBaseColor
                {
                    Value = new float4(0.5f, 0.5f, 0.5f, 1.0f)
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
        }
    }
}
