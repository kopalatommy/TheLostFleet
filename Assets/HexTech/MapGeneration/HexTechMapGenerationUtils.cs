using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using GalacticBoundStudios.EchoesOfTheFarRim;
using System;

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    [BurstCompile]
    public static class HexTechMapGenerationUtils
    {
        public const int HEXAGON_SOLID_VERTS = 7;
        public const int HEXAGON_HOLLOW_VERTS = 12;

        public const int HEXAGON_SOLID_TRIS = 18;
        public const int HEXAGON_HOLLOW_TRIS = 36;

        #region Generate vertices

        public static float3 HexCornerOffset(in HexOrientation hexOrientation, float2 size, int cornerIndex, float radius, in HexMapTransformData transformData)
        {
            float angle = 2.0f * math.PI * (hexOrientation.startAngle + cornerIndex) / 6;
            return new float3((size.x * math.cos(angle) * radius) * transformData.scale.x, 0, (size.y * math.sin(angle) * radius) * transformData.scale.y) + transformData.origin;
        }


        public static void GenerateHexagonVertices_Hollow(int index, float xPos, float yPos, float innerRadius, in HexMapTransformData transformData, in HexOrientation hexOrientation, ref NativeArray<float3> vertices)
        {
            // Generate the vertices for the hexagon
            int indexOffset = index * HEXAGON_HOLLOW_VERTS;
            for (int i = 0; i < 6; i++)
            {
                vertices[indexOffset + i] = HexCornerOffset(hexOrientation, new float2(1, 1), i, innerRadius, in transformData) + new float3(xPos, 0, yPos) + transformData.origin;
                vertices[indexOffset + i + 6] = HexCornerOffset(hexOrientation, new float2(1, 1), i, 1, in transformData) + new float3(xPos, 0, yPos) + transformData.origin;
            }
        }

        public static void GenerateHexagonVertices_Solid(int index, float xPos, float yPos, float innerRadius, in HexMapTransformData transformData, in HexOrientation hexOrientation, ref NativeArray<float3> vertices)
        {
            // Generate the vertices for the hexagon
            int indexOffset = index * HEXAGON_SOLID_VERTS;

            vertices[indexOffset] = new float3(xPos, 0, yPos);
            for (int i = 0; i < 6; i++)
            {
                vertices[indexOffset + i + 1] = HexCornerOffset(hexOrientation, new float2(1, 1), i, 1, in transformData) + new float3(xPos, 0, yPos) + transformData.origin;
            }
        }

        #endregion // Generate vertices

        #region  Generate triangles

        public static void GenerateHexagonTriangles_Hollow(int index, ref NativeArray<int> triangles)
        {
            // Generate the triangles for the hexagon
            int indexOffset = index * HEXAGON_HOLLOW_TRIS;
            int vertexOffset = index * HEXAGON_HOLLOW_VERTS;
            for (int i = 0; i < 6; i++)
            {
                triangles[indexOffset + i * 6] = vertexOffset + i;
                triangles[indexOffset + i * 6 + 2] = vertexOffset + i + 6;
                triangles[indexOffset + i * 6 + 1] = vertexOffset + (i + 1) % 6;

                triangles[indexOffset + i * 6 + 3] = vertexOffset + i + 6;
                triangles[indexOffset + i * 6 + 5] = vertexOffset + (i + 1) % 6 + 6;
                triangles[indexOffset + i * 6 + 4] = vertexOffset + (i + 1) % 6;
            }
        }

        public static void GenerateHexagonTriangles_Solid(int index, ref NativeArray<int> triangles)
        {
            // Generate the triangles for the hexagon
            int indexOffset = index * HEXAGON_SOLID_TRIS;
            int vertexOffset = index * HEXAGON_SOLID_VERTS;

            int sideStartIndex = vertexOffset + 1;
            for (int i = 0; i < 6; i++)
            {
                triangles[indexOffset + i * 3] = vertexOffset;
                triangles[indexOffset + i * 3 + 1] = sideStartIndex + i;
                triangles[indexOffset + i * 3 + 2] = sideStartIndex + ((i + 1) % 6);
            }
        }

        #endregion // Generate triangles

        #region Generate colors

        public static void GenerateHexagonColors_Hollow(int index, ref NativeArray<float4> colors, ref Unity.Mathematics.Random random)
        {
            // Generate the colors for the hexagon
            int indexOffset = index * 12;
            float4 hexagonColor = new float4(random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), 1.0f);
            for (int i = 0; i < 12; i++)
            {
                colors[indexOffset + i] = hexagonColor;
            }
        }

        public static void GenerateHexagonColors_Solid(int index, ref NativeArray<float4> colors, ref Unity.Mathematics.Random random)
        {
            // Generate the colors for the hexagon
            int indexOffset = index * 7;
            float4 hexagonColor = new float4(random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), 1.0f);
            for (int i = 0; i < 7; i++)
            {
                colors[indexOffset + i] = hexagonColor;
            }
        }

        #endregion // Generate colors

        #region Add Meta Data

        public static void GenerateHexagonMetaData_Hollow(int index, HexCoord coord, ref NativeArray<float4> data)
        {
            int startIndex = index * HEXAGON_HOLLOW_VERTS;
            for (int i = 0; i < HEXAGON_HOLLOW_VERTS; i++)
            {
                data[startIndex++] = new Vector4(coord.q, coord.r, 0, 0);
            }
        }

        public static void GenerateHexagonMetaData_Solid(int index, HexCoord coord, ref NativeArray<float4> data)
        {
            int startIndex = index * HEXAGON_SOLID_VERTS;
            for (int i = 0; i < HEXAGON_SOLID_VERTS; i++)
            {
                data[startIndex++] = new Vector4(coord.q, coord.r, 0, 0);
            }
        }

        #endregion

        #region Activation Grid

        public static HexagonActivationGrid CreateDefaultMap(HexTechGridShape gridShape, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds, Allocator allocator)
        {
            HexagonActivationGrid activationGrid = new HexagonActivationGrid()
            {
                hexGrid = new NativeHashMap<HexCoord, byte>(100, allocator),
                gridHeight = chunkSize,
                gridWidth = chunkSize,
                randomSeed = (uint)DateTime.Now.Ticks,
            };

            switch (gridShape)
            {
                case HexTechGridShape.Hexagon:
                    PopulateHexagonGrid(ref activationGrid, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.Rectangle:
                    PopulateRectangleGrid(ref activationGrid, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.Triangle:
                    PopulateTriangleGrid(ref activationGrid, chunkSize, ref minBounds, ref maxBounds);
                    break;
                case HexTechGridShape.HexagonRing:
                    PopulateHexagonRingGrid(ref activationGrid, chunkSize, ref minBounds, ref maxBounds);
                    break;
            }

            return activationGrid;
        }

        private static void PopulateHexagonGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
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

        private static void PopulateRectangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
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

        private static void PopulateTriangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
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

        private static void PopulateHexagonRingGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
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

        #endregion

        public static void CreateGenerateMapRequest(in EntityManager entityManager, in HexagonActivationGrid activationGrid)
        {
            Entity e = entityManager.CreateEntity();

            entityManager.AddComponentData(e, new HexHollowData
            {
                isHollow = true,
                innerRadius = 0.85f
            });
            entityManager.AddComponentData(e, activationGrid);

            entityManager.AddComponentData(e, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });
            entityManager.AddComponentData(e, new LocalToWorld
            {
                Value = float4x4.Translate(new float3(0, 0, 0))
            });

            entityManager.AddComponentData(e, new GenerateSystemMapRequestFlag());
        }
    }
}