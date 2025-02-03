using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.DataScribes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Collections.LowLevel.Unsafe;

namespace GalacticBoundStudios.HexTech.Generation
{
    [BurstCompile]
    public struct GenerateHexMeshJob : IJobParallelFor
    {
        public const int HEXAGON_SOLID_VERTS = 7;
        public const int HEXAGON_HOLLOW_VERTS = 12;

        public  const int HEXAGON_SOLID_TRIS = 18;
        public const int HEXAGON_HOLLOW_TRIS = 36;

        // The array of hexagons that should be created
        [ReadOnly]
        public NativeArray<HexCoord> activeHexagons;

        // This stores the data that determines if a hexagon should be created
        public HexagonMapAspect mapAspect;

        // This holds the results of the job
        public MeshData meshData;

        // Used to generate random colors
        public Random random;

        public void Execute(int hexIndex)
        {
            Debug.Log("Generating hexagon mesh");

            HexCoord coord = activeHexagons[hexIndex];

            // If the current hexagon is not marked as active, skip it
            if (mapAspect.activationGrid.ValueRO.hexGrid[coord] == 0) {
                Debug.Log("Hexagon is not active");
                return;
            }

            float2 worldPos = HexMath.HexToPixel(coord, mapAspect.transformData.ValueRO);

            if (mapAspect.hollowData.ValueRO.isHollow) {
                Debug.Log("Generating hollow hexagon");

                GenerateHexagonTriangles_Hollow(hexIndex, ref meshData.triangles);
                GenerateHexagonVertices_Hollow(hexIndex, worldPos.x, worldPos.y, mapAspect.hollowData.ValueRO.innerRadius, in mapAspect.transformData.ValueRO, mapAspect.transformData.ValueRO.orientation, ref meshData.vertices);
                GenerateHexagonColors_Hollow(hexIndex, ref meshData.colors, ref random);
            } else {
                Debug.Log("Generating solid hexagon");

                GenerateHexagonVertices_Solid(hexIndex, worldPos.x, worldPos.y, mapAspect.hollowData.ValueRO.innerRadius, in mapAspect.transformData.ValueRO, mapAspect.transformData.ValueRO.orientation, ref meshData.vertices);
                GenerateHexagonTriangles_Solid(hexIndex, ref meshData.triangles);
                GenerateHexagonColors_Solid(hexIndex, ref meshData.colors, ref random);
            }
        }

        #region Generate vertices

        private static float3 HexCornerOffset(in HexOrientation hexOrientation, float2 size, int cornerIndex, float radius, in HexMapTransformData transformData)
        {
            float angle = 2.0f * math.PI * (hexOrientation.startAngle + cornerIndex) / 6;
            return new float3((size.x * math.cos(angle) * radius) * transformData.scale.x, 0, (size.y * math.sin(angle) * radius) * transformData.scale.y) + transformData.origin;
        }


        public static void GenerateHexagonVertices_Hollow(int index, float xPos, float yPos, float innerRadius, in HexMapTransformData transformData, in HexOrientation hexOrientation, ref FixedArray<float3> vertices)
        {
            // Generate the vertices for the hexagon
            int indexOffset = index * HEXAGON_HOLLOW_VERTS;
            for (int i = 0; i < 6; i++)
            {
                vertices[indexOffset + i] = HexCornerOffset(hexOrientation, new float2(1, 1), i, innerRadius, in transformData) + new float3(xPos, 0, yPos) + transformData.origin;
                vertices[indexOffset + i + 6] = HexCornerOffset(hexOrientation, new float2(1, 1), i, 1, in transformData) + new float3(xPos, 0, yPos) + transformData.origin;
            }
        }

        public static void GenerateHexagonVertices_Solid(int index, float xPos, float yPos, float innerRadius, in HexMapTransformData transformData, in HexOrientation hexOrientation, ref FixedArray<float3> vertices)
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

        public static void GenerateHexagonTriangles_Hollow(int index, ref FixedArray<int> triangles)
        {
            // Generate the triangles for the hexagon
            int indexOffset = index * HEXAGON_HOLLOW_TRIS;
            int vertexOffset = index * HEXAGON_HOLLOW_VERTS;
            for (int i = 0; i < 6; i++)
            {
                triangles[indexOffset + i * 6] = vertexOffset + i;
                triangles[indexOffset + i * 6 + 1] = vertexOffset + i + 6;
                triangles[indexOffset + i * 6 + 2] = vertexOffset + (i + 1) % 6;
                
                triangles[indexOffset + i * 6 + 3] = vertexOffset + i + 6;
                triangles[indexOffset + i * 6 + 4] = vertexOffset + (i + 1) % 6 + 6;
                triangles[indexOffset + i * 6 + 5] = vertexOffset + (i + 1) % 6;
            }
        }
        
        public static void GenerateHexagonTriangles_Solid(int index, ref FixedArray<int> triangles)
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

        public static void GenerateHexagonColors_Hollow(int index, ref FixedArray<float4> colors, ref Unity.Mathematics.Random random)
        {
            // Generate the colors for the hexagon
            int indexOffset = index * 12;
            float4 hexagonColor = new float4(random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), 1.0f);
            for (int i = 0; i < 12; i++)
            {
                colors[indexOffset + i] = hexagonColor;
            }
        }

        public static void GenerateHexagonColors_Solid(int index, ref FixedArray<float4> colors, ref Unity.Mathematics.Random random)
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
    }
}