using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using GalacticBoundStudios.HexTech.Generation;

using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.DataScribes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Collections.LowLevel.Unsafe;

namespace GalacticBoundStudios.HexTech.MeshGeneration
{
    public partial struct HexagonMeshGenerator : ISystem
    {
        

        void OnCreate(ref SystemState state)
        {
            // Only run this system when there is a request to generate a mesh
            state.RequireForUpdate<CreateMapTag>();
        }

        void OnUpdate(ref SystemState state)
        {
            Debug.Log("Generating hexagon mesh");

            // Get the aspect with the data required to make the mesh
            Entity entity = SystemAPI.GetSingletonEntity<CreateMapTag>();

            // Get all related map information required to build the map
            HexagonMapAspect hexMapData = SystemAPI.GetAspect<HexagonMapAspect>(entity);

            // Create random number generator for the mesh
            Random random = new Random(hexMapData.activationGrid.ValueRO.randomSeed);

            GenerateMesh(ref hexMapData, in random, out MeshData meshData);

            // Remove the request
            state.EntityManager.RemoveComponent<CreateMapTag>(entity);
            state.EntityManager.AddComponentData<MeshData>(entity, meshData);
            state.EntityManager.AddComponent<DrawMeshTag>(entity);

            Debug.Log("Finished generating hexagon mesh");
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

        // void OnDestroy(ref SystemState state)
        // {
        //     EntityQuery entityQuery = state.GetEntityQuery(ComponentType.ReadOnly<MeshData>());

        //     EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        //     DestroyMeshDataJob job = new DestroyMeshDataJob()
        //     {
        //         commandBuffer = commandBuffer,
        //     };

        //     job.Schedule(entityQuery, state.Dependency).Complete();

        //     // commandBuffer.Playback(state.EntityManager);
        // }

        void GenerateMesh(ref HexagonMapAspect request, in Random random, out MeshData meshData)
        {
            meshData = new MeshData();
            
            int numHexagons = request.activationGrid.ValueRO.hexGrid.Count;
            NativeArray<HexCoord> hexagonsToCreate = request.activationGrid.ValueRO.hexGrid.GetKeyArray(Allocator.Persistent);

            // Allocate the arrays for the mesh data
            // int numHexagons = request.activationGrid.ValueRO.gridWidth * request.activationGrid.ValueRO.gridHeight;
            if (request.hollowData.ValueRO.isHollow) {
                AllocateMeshDataArrays_Hollow(numHexagons, ref meshData);
            } else {
                AllocateMeshDataArrays_Solid(numHexagons, ref meshData);
            }

            Debug.Log("Running hexagon mesh job");

            GenerateHexMeshJob job = new GenerateHexMeshJob
            {
                activeHexagons = hexagonsToCreate,
                mapAspect = request,
                random = random,
                meshData = meshData,
            };
            job.Schedule(hexagonsToCreate.Length, 1).Complete();

            Debug.Log("Finished hexagon mesh job");

            hexagonsToCreate.Dispose();
        }

        private void AllocateMeshDataArrays_Solid(int numHexagons, ref MeshData meshData)
        {
            Debug.Log("Allocating solid mesh data arrays");

            meshData.vertices = new FixedArray<float3>(numHexagons * GenerateHexMeshJob.HEXAGON_SOLID_VERTS);
            meshData.triangles = new FixedArray<int>(numHexagons * GenerateHexMeshJob.HEXAGON_SOLID_TRIS);
            meshData.normals = new FixedArray<float3>(numHexagons * GenerateHexMeshJob.HEXAGON_SOLID_VERTS);
            meshData.colors = new FixedArray<float4>(numHexagons * GenerateHexMeshJob.HEXAGON_SOLID_VERTS);
        }

        private void AllocateMeshDataArrays_Hollow(int numHexagons, ref MeshData meshData)
        {
            Debug.Log("Allocating hollow mesh data arrays");

            meshData.vertices = new FixedArray<float3>(numHexagons * GenerateHexMeshJob.HEXAGON_HOLLOW_VERTS);
            meshData.triangles = new FixedArray<int>(numHexagons * GenerateHexMeshJob.HEXAGON_HOLLOW_TRIS);
            meshData.normals = new FixedArray<float3>(numHexagons * GenerateHexMeshJob.HEXAGON_HOLLOW_VERTS);
            meshData.colors = new FixedArray<float4>(numHexagons * GenerateHexMeshJob.HEXAGON_HOLLOW_VERTS);
        }
    }
}