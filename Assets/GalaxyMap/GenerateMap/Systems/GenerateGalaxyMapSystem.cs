using GalacticBoundStudios.BattleBrain.CameraControls;
using GalacticBoundStudios.BattleBrain.TurnBased;
using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.MeshMania;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    // This is intended to be run once, when the app is started
    public partial struct GenerateGalaxyMapSystem : ISystem, ISystemStartStop
    {
        private EntityQuery updateQuery;

        private void OnCreate(ref SystemState state)
        {
            updateQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<EnableGalaxyMapFlag>().WithNone<DidCreateGalaxyMapTag>().Build(ref state);
            state.RequireForUpdate(updateQuery);
        }

        private void OnDestroy(ref SystemState state)
        {
            if (updateQuery.TryGetSingletonEntity<DidCreateGalaxyMapTag>(out Entity didCreateTagEntity))
            {
                state.EntityManager.DestroyEntity(didCreateTagEntity);
            }

            DestroyAllHexTiles(ref state);
        }

        private void OnUpdate(ref SystemState state)
        {
            // Intentionally left empty
        }

        public void OnStartRunning(ref SystemState state)
        {
            Debug.Log("GenerateGalaxyMapSystem.OnStartRunning");

            CreateEmptyMap(ref state);
            SpawnCameraEntity(ref state);
            StartUISystem(ref state);

            // Create the tag to prevent updates
            Entity e = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(e, new DidCreateGalaxyMapTag());
        }

        private void SpawnCameraEntity(ref SystemState state)
        {
            Entity e = state.EntityManager.CreateEntity();

            state.EntityManager.SetName(e, "RTSCameraEntity");


            RTSCameraConfig cameraConfig = RTSCameraConfig.Default;

            state.EntityManager.AddComponentData(e, new RTSCameraMovementSettings
            {
                movementSpeed = cameraConfig.movementSpeed,
                rotationSpeed = cameraConfig.rotationSpeed,
                mouseRotationSpeed = cameraConfig.mouseRotationSpeed,
                zoomSpeed = cameraConfig.zoomSpeed,
                edgeMoveThreshold = cameraConfig.edgeMoveThreshold
            });
            state.EntityManager.AddComponentData(e, new RTSCameraMoveData
            {
                horizontalMovement = float3.zero,
                zoom = 0,
                rotation = float3.zero
            });
            state.EntityManager.AddComponentData(e, new RTSCameraInitialTransformData
            {
                initialPosition = UnityEngine.Camera.main.transform.position,
                initialRotation = UnityEngine.Camera.main.transform.rotation
            });
            state.EntityManager.AddComponentData(e, new RTSCameraSettings
            {
                orthographic = UnityEngine.Camera.main.orthographic,
                fieldOfView = UnityEngine.Camera.main.fieldOfView,
                aspect = UnityEngine.Camera.main.aspect,
                orthographicSize = UnityEngine.Camera.main.orthographicSize
            });
            state.EntityManager.AddComponentData(e, new RTSCameraTag());
            state.EntityManager.AddComponentData(e, new LocalTransform
            {
                Position = Camera.main.transform.position,
                Rotation = Camera.main.transform.rotation,
                Scale = 1
            });
        }

        private void CreateEmptyMap(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            EntityArchetype hexArchetype = GalaxyMapCreateUtils.CreateCellPrefab(ref entityManager);

            RenderMeshArray renderMeshArray = GalaxyMapCreateUtils.DefaultRenderMeshArray();

            HexMapTransformData hexMapTransformData = HexMapTransformData.Default;

            NativeArray<Entity> entities = entityManager.CreateEntity(hexArchetype, 25 * 25, Allocator.Temp);

            RenderMeshDescription desc = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: false,
                    renderingLayerMask: 1);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];

                int q = i / 25;
                int r = i % 25;

                entityManager.SetName(e, "(" + q + ", " + r + ")");

                float2 point = HexMath.HexToPixel(new HexCoord(q, r), hexMapTransformData);
                entityManager.SetComponentData(e, new LocalTransform
                {
                    Position = new float3(point.x, 0, point.y),
                    Scale = 1,
                    Rotation = quaternion.identity
                });
                // Should be unnecessary
                // entityManager.SetComponentData(e, new LocalToWorld
                // {
                //     Value = float4x4.TRS(float3.zero, quaternion.identity, new float3(1, 1, 1))
                // });
                entityManager.SetSharedComponentManaged(e, renderMeshArray);
                entityManager.SetComponentData(e, new HexCoord { q = q, r = r });
                entityManager.SetComponentData(e, new MeshIndexData
                {
                    index = renderMeshArray.MeshReferences[0].Value.GetInstanceID()
                });


                RenderMeshUtility.AddComponents(e,
                                                state.EntityManager,
                                                desc,
                                                renderMeshArray,
                                                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            }

            // for (int q = 0; q < 25; q++)
            // {
            //     for (int r = 0; r < 25; r++)
            //     {
            //         Entity e = state.EntityManager.CreateEntity(hexArchetype);

            //         float2 point = HexMath.HexToPixel(new HexCoord(q, r), hexMapTransformData);
            //         entityManager.SetComponentData(e, new LocalTransform
            //         {
            //             Position = new float3(point.x, 0, point.y),
            //             Scale = 1,
            //             Rotation = quaternion.identity
            //         });
            //         // Should be unnecessary
            //         // entityManager.SetComponentData(e, new LocalToWorld
            //         // {
            //         //     Value = float4x4.TRS(float3.zero, quaternion.identity, new float3(1, 1, 1))
            //         // });
            //         entityManager.SetSharedComponentManaged(e, renderMeshArray);
            //         entityManager.SetComponentData(e, new HexCoord { q = q, r = r });
            //         entityManager.SetComponentData(e, new MeshIndexData
            //         {
            //             index = renderMeshArray.MeshReferences[0].Value.GetInstanceID()
            //         });
            //     }
            // }
        }

        void StartUISystem(ref SystemState state)
        {
            Entity e = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData<TurnBasedEnableFlag>(e, new TurnBasedEnableFlag());
        }

        public void OnStopRunning(ref SystemState state)
        {
            // Intentionally left empty
        }

        private void DestroyAllHexTiles(ref SystemState state)
        {
            EntityQuery hexTileQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<HexCoord>().Build(ref state);

            state.EntityManager.DestroyEntity(hexTileQuery.ToEntityArray(Allocator.Temp));
        }
    }
}