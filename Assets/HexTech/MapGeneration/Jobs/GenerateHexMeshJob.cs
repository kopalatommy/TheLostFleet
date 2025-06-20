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

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    [BurstCompile]
    public struct GenerateHexMeshJob : IJobParallelFor
    {
        // The array of hexagons that should be created
        [ReadOnly]
        public NativeArray<HexCoord> activeHexagons;
        [ReadOnly]
        public HexagonActivationGrid activationGrid;

        public HexHollowData hexHollowData;
        public HexMapTransformData mapTransformData;

        // This holds the results of the job
        public MeshData meshData;

        // Used to generate random colors
        public Random random;

        public void Execute(int hexIndex)
        {
            HexCoord coord = activeHexagons[hexIndex];

            // If the current hexagon is not marked as active, skip it
            if (activationGrid.hexGrid[coord] == 0) {
                Debug.Log("Hexagon is not active");
                return;
            }

            float2 worldPos = HexMath.HexToPixel(coord, in mapTransformData);

            if (hexHollowData.isHollow) {
                HexTechMapGenerationUtils.GenerateHexagonTriangles_Hollow(hexIndex, ref meshData.triangles);
                HexTechMapGenerationUtils.GenerateHexagonVertices_Hollow(hexIndex, worldPos.x, worldPos.y, hexHollowData.innerRadius, in mapTransformData, ref meshData.vertices);
                HexTechMapGenerationUtils.GenerateHexagonColors_Hollow(hexIndex, ref meshData.colors, ref random);
                HexTechMapGenerationUtils.GenerateHexagonMetaData_Hollow(hexIndex, coord, ref meshData.uv2);
            } else {
                HexTechMapGenerationUtils.GenerateHexagonVertices_Solid(hexIndex, worldPos.x, worldPos.y, in mapTransformData, ref meshData.vertices);
                HexTechMapGenerationUtils.GenerateHexagonTriangles_Solid(hexIndex, ref meshData.triangles);
                HexTechMapGenerationUtils.GenerateHexagonColors_Solid(hexIndex, ref meshData.colors, ref random);
                HexTechMapGenerationUtils.GenerateHexagonMetaData_Solid(hexIndex, coord, ref meshData.uv2);
            }
        }
    }
}