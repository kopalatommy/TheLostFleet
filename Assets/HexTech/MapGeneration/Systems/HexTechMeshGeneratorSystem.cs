using Unity.Entities;
using Unity.Mathematics;
using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.DataScribes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using GalacticBoundStudios.HexTech.PathFinding;

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    public partial struct HexTechMeshGeneratorSystem : ISystem
    {
        private EntityQuery mapRequestQuery;
        private EntityQuery mapSettingsQuery;

        void OnCreate(ref SystemState state)
        {
            mapRequestQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<HexTechCreateMapTag>().Build(ref state);
            mapSettingsQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<HexMapTransformData>().Build(ref state);

            // Only run this system when there is a request to generate a mesh
            state.RequireForUpdate<HexTechCreateMapTag>();
        }

        void OnUpdate(ref SystemState state)
        {
            Debug.Log("Generating hexagon mesh");

            // Get the global map settings
            Entity mapStateEntity = mapSettingsQuery.GetSingletonEntity();
            HexMapTransformData mapTransformData = state.EntityManager.GetComponentData<HexMapTransformData>(mapStateEntity);
            HexHollowData hexHollowData = state.EntityManager.GetComponentData<HexHollowData>(mapStateEntity);

            // Get all entities that have the map request component
            NativeArray<Entity> mapRequestEntities = mapRequestQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < mapRequestEntities.Length; i++)
            {
                Entity e = mapRequestEntities[i];

                // Remove the create map flag to prevent creating multiple meshes for the same entity
                state.EntityManager.RemoveComponent<HexTechCreateMapTag>(e);

                HexagonActivationGrid activationGrid = state.EntityManager.GetComponentData<HexagonActivationGrid>(e);
                NativeArray<HexCoord> hexagonsToCreate = activationGrid.hexGrid.GetKeyArray(Allocator.Persistent);

                MeshData meshData = new MeshData();

                if (hexHollowData.isHollow)
                {
                    AllocateMeshDataArrays_Hollow(hexagonsToCreate.Length, ref meshData);
                }
                else
                {
                    AllocateMeshDataArrays_Solid(hexagonsToCreate.Length, ref meshData);
                }

                // For now, we will not worry about multi-threading
                GenerateMeshData(in hexagonsToCreate, in activationGrid, in hexHollowData, in mapTransformData, ref meshData);

                state.EntityManager.RemoveComponent<HexTechCreateMapTag>(e);
                state.EntityManager.AddComponentData<MeshData>(e, meshData);
                state.EntityManager.AddComponent<DrawMeshTag>(e);
            }
        }

        public partial struct DestroyMeshDataJob : IJobEntity
        {
            public EntityCommandBuffer commandBuffer;

            void Execute(Entity entity, ref MeshData meshData)
            {
                meshData.colors.Dispose();
                meshData.normals.Dispose();
                meshData.triangles.Dispose();
                meshData.vertices.Dispose();

                // commandBuffer.RemoveComponent<MeshData>(entity);
            }
        }

        public void GenerateMeshData(in NativeArray<HexCoord> activeHexagons, in HexagonActivationGrid activationGrid, in HexHollowData hollowData, in HexMapTransformData mapTransformData, ref MeshData meshData)
        {
            Random rand = new Random(activationGrid.randomSeed);

            GenerateHexMeshJob job = new GenerateHexMeshJob()
            {
                activeHexagons = activeHexagons,
                activationGrid = activationGrid,
                hexHollowData = hollowData,
                mapTransformData = mapTransformData,
                random = rand,
                meshData = meshData,
            };

            meshData.addUV2 = true;

            job.Schedule(activeHexagons.Length, activeHexagons.Length / 10).Complete();
        }
        private void AllocateMeshDataArrays_Solid(int numHexagons, ref MeshData meshData)
        {
            Debug.Log("Allocating solid mesh data arrays");

            meshData.vertices = new NativeArray<float3>(numHexagons * HexTechMapGenerationUtils.HEXAGON_SOLID_VERTS, Allocator.Persistent);
            meshData.triangles = new NativeArray<int>(numHexagons * HexTechMapGenerationUtils.HEXAGON_SOLID_TRIS, Allocator.Persistent);
            meshData.normals = new NativeArray<float3>(numHexagons * HexTechMapGenerationUtils.HEXAGON_SOLID_VERTS, Allocator.Persistent);
            meshData.colors = new NativeArray<float4>(numHexagons * HexTechMapGenerationUtils.HEXAGON_SOLID_VERTS, Allocator.Persistent);
            meshData.uv2 = new NativeArray<float4>(numHexagons * HexTechMapGenerationUtils.HEXAGON_SOLID_VERTS, Allocator.Persistent);
        }

        private void AllocateMeshDataArrays_Hollow(int numHexagons, ref MeshData meshData)
        {
            Debug.Log("Allocating hollow mesh data arrays");

            meshData.vertices = new NativeArray<float3>(numHexagons * HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Persistent);
            meshData.triangles = new NativeArray<int>(numHexagons * HexTechMapGenerationUtils.HEXAGON_HOLLOW_TRIS, Allocator.Persistent);
            meshData.normals = new NativeArray<float3>(numHexagons * HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Persistent);
            meshData.colors = new NativeArray<float4>(numHexagons * HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Persistent);
            meshData.uv2 = new NativeArray<float4>(numHexagons * HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Persistent);
        }
    }
}