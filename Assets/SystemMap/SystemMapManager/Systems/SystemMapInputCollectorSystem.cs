using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.HexTech.PathFinding;
using GalacticBoundStudios.HexTech.Shaders;
using GalacticBoundStudios.BattleBrain.CameraControls;
using GalacticBoundStudios.BattleBrain;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    public partial class SystemMapInputCollectorSystem : SystemBase
    {
        static RTSCameraInputActions inputSystem;

        private EntityQuery selectedUnitsQuery;


        // Used to get the camera entity singleton
        private EntityQuery cameraSingletonQuery;
        private EntityQuery mapConfigQuery;

        // This is the query that is used to determine if this can update
        private EntityQuery updateQuery;

        protected override void OnCreate()
        {
            Debug.Log("SystemMapInputCollectorSystem.OnCreate");

            updateQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SystemMapEnableFlag>().WithAll<HexMapTransformData>().Build(EntityManager);
            RequireForUpdate(updateQuery);

            Entity inputDataEntity = EntityManager.CreateEntity(typeof(SystemMapInputData));
            // Entity rtsInputEntity = EntityManager.CreateEntity(typeof(RTSCameraMoveData));
            Entity focusCoordEntity = EntityManager.CreateEntity(typeof(SystemMapFocusHexagonData));
            Entity selectedCoordEntity = EntityManager.CreateEntity(typeof(SystemMapSelectedHexagonData));

            inputSystem = new RTSCameraInputActions();
            inputSystem.Enable();

            selectedUnitsQuery = GetEntityQuery(
                ComponentType.ReadOnly<SelectableTag>(),
                ComponentType.ReadOnly<SelectedTag>()
            );

            cameraSingletonQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<RTSCameraMovementSettings>().Build(EntityManager);
            mapConfigQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<HexMapTransformData>().Build(EntityManager);
        }

        protected override void OnDestroy()
        {
            inputSystem.Disable();
            inputSystem.Dispose();
            inputSystem = null;
        }

        protected override void OnUpdate()
        {
            HandleCameraInput();
        }

        #region Camera Input

        protected void HandleCameraInput()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            EntityCommandBuffer.ParallelWriter parallelWriter = ecb.AsParallelWriter();

            Entity cameraEntity = cameraSingletonQuery.GetSingletonEntity();

            UpdateCameraMoveData(cameraEntity);
            HandleMouseInput(cameraEntity, parallelWriter);








            // foreach (var moveData in SystemAPI.Query<RTSCameraAspect>()) {
            //     moveData.moveData.ValueRW.horizontalMovement = ReadHorizontalMovement(moveData.movementSettings.ValueRO, moveData.localTransform.ValueRO);
            //     moveData.moveData.ValueRW.horizontalMovement += ReadEdgeScrolling(moveData.movementSettings.ValueRO, moveData.localTransform.ValueRO);
            //     moveData.moveData.ValueRW.zoom = ReadZoom();
            //     moveData.moveData.ValueRW.rotation = ReadRotation(moveData.localTransform.ValueRO) + ReadMouseRotation();

            //     Ray ray;
            //     HexCoord coord;
            //     if (moveData.cameraSettings.ValueRO.orthographic)
            //     {
            //         ray = CameraUtilities.ScreenPointToRay_Orthographic(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), moveData.cameraSettings.ValueRO.aspect, moveData.localTransform.ValueRO.Position, moveData.localTransform.ValueRO.Rotation, moveData.cameraSettings.ValueRO.orthographicSize, moveData.localTransform.ValueRO.Forward());
            //     }
            //     else
            //     {
            //         ray = CameraUtilities.ScreenPointToRay_Standard(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), moveData.cameraSettings.ValueRO.fieldOfView, moveData.cameraSettings.ValueRO.aspect, moveData.localTransform.ValueRO.Position, moveData.localTransform.ValueRO.Rotation);
            //     }
            //     float3 intersection = DetermineRayIntersection(ray);
            //     coord = HexMath.PixelToHex(new float2(intersection.x, intersection.z), mapConfigData.ValueRO);
            //     SystemAPI.GetSingletonRW<SystemMapFocusHexagonData>().ValueRW.coord = coord;

            //     if (inputSystem.HexMap.Click.triggered)
            //     {
            //         Debug.Log("inputSystem.HexMap.Click.triggered");
            //         SystemAPI.GetSingletonRW<SystemMapSelectedHexagonData>().ValueRW.coord = coord;

            //         Entities.WithAll<SelectedTag, SelectableTag, HexTechMapEntityTag>().ForEach((Entity entity, in SelectableTag selectable, in SelectedTag selected, in HexTechMapEntityTag mapEntityTag) => {
            //             parallelWriter.AddComponent(entity.Index, entity, new HexTechCreatePathRequest
            //             {
            //                 startPos = mapEntityTag.gridPosition,
            //                 endPos = coord
            //             });
            //         }).Run();
            //     }

            //     SystemAPI.GetSingletonRW<HighlightedAxialCoordsVector4Override>().ValueRW.Value = new float4(coord.q, coord.r, 0, 0);
            // }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        void HandleMouseInput(Entity cameraEntity, EntityCommandBuffer.ParallelWriter parallelWriter)
        {
            RTSCameraSettings cameraSettings = EntityManager.GetComponentData<RTSCameraSettings>(cameraEntity);
            HexMapTransformData mapConfigData = mapConfigQuery.GetSingleton<HexMapTransformData>();
            LocalTransform cameraTransform = EntityManager.GetComponentData<LocalTransform>(cameraEntity);

            Ray ray;
            HexCoord coord;
            if (cameraSettings.orthographic)
            {
                ray = CameraUtilities.ScreenPointToRay_Orthographic(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation, cameraSettings.orthographicSize, cameraTransform.Forward());
            }
            else
            {
                ray = CameraUtilities.ScreenPointToRay_Standard(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), cameraSettings.fieldOfView, cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation);
            }
            float3 intersection = DetermineRayIntersection(ray);
            coord = HexMath.PixelToHex(new float2(intersection.x, intersection.z), mapConfigData);
            SystemAPI.GetSingletonRW<SystemMapFocusHexagonData>().ValueRW.coord = coord;

            if (inputSystem.HexMap.Click.triggered)
            {
                Debug.Log("inputSystem.HexMap.Click.triggered");
                SystemAPI.GetSingletonRW<SystemMapSelectedHexagonData>().ValueRW.coord = coord;

                Entities.WithAll<SelectedTag, SelectableTag, HexTechMapEntityTag>().ForEach((Entity entity, in SelectableTag selectable, in SelectedTag selected, in HexTechMapEntityTag mapEntityTag) => {
                    parallelWriter.AddComponent(entity.Index, entity, new HexTechCreatePathRequest
                    {
                        startPos = mapEntityTag.gridPosition,
                        endPos = coord
                    });
                }).Run();
            }

            SystemAPI.GetSingletonRW<HighlightedAxialCoordsVector4Override>().ValueRW.Value = new float4(coord.q, coord.r, 0, 0);
        }

        public float3 DetermineRayIntersection(in Ray ray)
        {
            float intersection = CalculateIntersection(ray.origin, ray.direction, Vector3.zero, Vector3.up);

            if (float.IsNaN(intersection))
            {
                return Vector3.zero;
            }
            return ray.origin + (ray.direction * intersection);
        }

        public float CalculateIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePoint, Vector3 planeNormal)
        {
            float denominator = Vector3.Dot(planeNormal, rayDirection);
            // Make sure the ray is not parallel to the plane
            if (Mathf.Abs(denominator) < 1e-6)
            {
                return float.NaN;
            }

            Vector3 difference = planePoint - rayOrigin;
            float t = Vector3.Dot(difference, planeNormal) / denominator;
            return t;
        }

        void UpdateCameraMoveData(Entity cameraEntity)
        {
            RTSCameraMovementSettings moveSettings = EntityManager.GetComponentData<RTSCameraMovementSettings>(cameraEntity);
            LocalTransform cameraTransform = EntityManager.GetComponentData<LocalTransform>(cameraEntity);

            RTSCameraMoveData moveData = new RTSCameraMoveData()
            {
                horizontalMovement = ReadHorizontalMovement(moveSettings, cameraTransform) + ReadEdgeScrolling(moveSettings, cameraTransform),
                zoom = ReadZoom(),
                rotation = ReadRotation(in cameraTransform) + ReadMouseRotation()
            };

            EntityManager.SetComponentData(cameraEntity, moveData);
        }

        float3 DetermineMoveDirection(in LocalTransform localTransform, in float2 move)
        {
            // Calculate movement direction relative to camera orientation
            float3 forward = localTransform.Forward();
            forward.y = 0; // Flatten the forward vector to horizontal plane
            forward = math.normalize(forward);

            float3 right = localTransform.Right();
            right.y = 0; // Flatten the right vector to horizontal plane
            right = math.normalize(right);

            // Calculate final movement vector
            float3 movement = forward * move.y + right * move.x;

            if (math.length(movement) > 1)
            {
                movement = math.normalize(movement);
            }

            return movement;
        }

        float3 ReadHorizontalMovement(in RTSCameraMovementSettings moveSettings, in LocalTransform localTransform)
        {
            float2 move = inputSystem.HexMap.Move.ReadValue<Vector2>();

            return DetermineMoveDirection(localTransform, move);
        }

        float3 ReadEdgeScrolling(in RTSCameraMovementSettings settings, in LocalTransform localTransform)
        {
            // If the player is rotating the camera with the mouse, ignore edge scrolling
            if (Mouse.current.rightButton.isPressed)
            {
                return float3.zero;
            }

            float2 mousePos = Mouse.current.position.ReadValue();

            mousePos.x /= Screen.width;
            mousePos.y /= Screen.height;

            float2 moveVector = float2.zero;

            // Left right edge scrolling
            if (mousePos.x < settings.edgeMoveThreshold)
            {
                moveVector.x -= 1;
            }
            else if (mousePos.x > 1 - settings.edgeMoveThreshold)
            {
                moveVector.x += 1;
            }

            // Forward and back edge scrolling
            if (mousePos.y < settings.edgeMoveThreshold)
            {
                moveVector.y -= 1;
            }
            else if (mousePos.y > 1 - settings.edgeMoveThreshold)
            {
                moveVector.y += 1;
            }

            if (math.length(moveVector) > 0)
            {
                moveVector = math.normalize(moveVector);
                return DetermineMoveDirection(localTransform, moveVector);
            }
            
            return float3.zero;
        }

        float3 ReadRotation(in LocalTransform localTransform)
        {
            return (new float3(0, 1, 0)) * inputSystem.HexMap.Rotate.ReadValue<float>();
        }

        float3 ReadMouseRotation()
        {
            if (Mouse.current.rightButton.isPressed)
            {
                // Read the mouse delta
                float2 currentMousePos = Mouse.current.position.ReadValue();

                // Update the last mouse position
                float2 delta = Mouse.current.delta.ReadValue();

                // Horizontal rotation (around world Y axis)
                float3 horizontalRotation = new float3(0, delta.x, 0);

                // Vertical rotation (around local X axis)
                float3 verticalRotation = new float3(-delta.y, 0, 0);

                return horizontalRotation + verticalRotation;
            }

            return new float3(0, 0, 0);
        }

        float ReadZoom()
        {
            return inputSystem.HexMap.Zoom.ReadValue<float>();
        }

        #endregion // Camera Input
    }
}