using GalacticBoundStudios.BattleBrain.CameraControls;
using GalacticBoundStudios.HexTech;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public partial class GalaxyMapInputHandling : SystemBase
    {
        private EntityQuery updateQuery;
        private EntityQuery rtsCameraQuery;

        private static RTSCameraInputActions inputSystem;

        private EntityQuery selectedTileQuery;
        private EntityQuery focusTileQuery;

        protected override void OnCreate()
        {
            EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp);

            updateQuery = queryBuilder.WithAll<EnableGalaxyMapFlag>().Build(EntityManager);
            RequireForUpdate(updateQuery);

            queryBuilder.Reset();
            rtsCameraQuery = queryBuilder.WithAll<RTSCameraMoveData>().Build(EntityManager);

            queryBuilder.Reset();
            selectedTileQuery = queryBuilder.WithAll<GalaxyMapSelectedTileFlag>().Build(EntityManager);

            queryBuilder.Reset();
            focusTileQuery = queryBuilder.WithAll<GalaxyMapFocusTileFlag>().Build(EntityManager);
        }

        protected override void OnUpdate()
        {
            HandleCameraInput();
            HandleMouseInput();
        }

        void HandleCameraInput()
        {
            Entity cameraEntity = rtsCameraQuery.GetSingletonEntity();

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

        float3 ReadHorizontalMovement(in RTSCameraMovementSettings moveSettings, in LocalTransform localTransform)
        {
            float2 move = inputSystem.HexMap.Move.ReadValue<Vector2>();

            return DetermineMoveDirection(localTransform, move);
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

        float ReadZoom()
        {
            return inputSystem.HexMap.Zoom.ReadValue<float>();
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

        private void HandleMouseInput()
        {
            Entity cameraEntity = rtsCameraQuery.GetSingletonEntity();

            RTSCameraSettings cameraSettings = EntityManager.GetComponentData<RTSCameraSettings>(cameraEntity);
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
            coord = HexMath.PixelToHex(new float2(intersection.x, intersection.z), HexMapTransformData.Default);

            NativeList<HexCoord> resultList = new NativeList<HexCoord>(Allocator.Temp);
            GalaxyMapTileManager.NodeMap.QueryKNearest(intersection, 1, resultList);

            // No reason to continue on if the cursor is not over a tile
            if (resultList.IsEmpty || !GalaxyMapTileManager.EntityMap.TryGetValue(coord, out Entity tileEntity))
            {
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // If already selected, do nothing
                if (!EntityManager.HasComponent<GalaxyMapSelectedTileFlag>(tileEntity))
                {
                    if (selectedTileQuery.CalculateEntityCount() > 0)
                    {
                        Entity selected = selectedTileQuery.GetSingletonEntity();
                        EntityManager.RemoveComponent<GalaxyMapSelectedTileFlag>(selected);
                        EntityManager.RemoveComponent<URPMaterialPropertyBaseColor>(selected);
                    }

                    EntityManager.AddComponentData(tileEntity, new GalaxyMapSelectedTileFlag());
                    // Also has the cursor focus
                    EntityManager.AddComponentData(tileEntity, new GalaxyMapFocusTileFlag());
                    EntityManager.AddComponentData<URPMaterialPropertyBaseColor>(tileEntity, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(1, 1, 0, 1)
                    });
                }
            }
            else if (!EntityManager.HasComponent<GalaxyMapFocusTileFlag>(tileEntity))
            {
                if (focusTileQuery.CalculateEntityCount() > 0)
                {
                    Entity selected = focusTileQuery.GetSingletonEntity();
                    EntityManager.RemoveComponent<GalaxyMapFocusTileFlag>(selected);
                    if (!EntityManager.HasComponent<GalaxyMapSelectedTileFlag>(selected))
                    {
                        EntityManager.RemoveComponent<URPMaterialPropertyBaseColor>(selected);
                    }
                }

                EntityManager.AddComponentData(tileEntity, new GalaxyMapFocusTileFlag());

                // Override color only if not already selected
                if (!EntityManager.HasComponent<GalaxyMapSelectedTileFlag>(tileEntity))
                {
                    EntityManager.AddComponentData<URPMaterialPropertyBaseColor>(tileEntity, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(0, 0, 1, 1)
                    });
                }
            }

            // if (inputSystem.HexMap.Click.triggered)
            // {
            //     Debug.Log("inputSystem.HexMap.Click.triggered");
            //     SystemAPI.GetSingletonRW<SystemMapSelectedHexagonData>().ValueRW.coord = coord;

            //     Entities.WithAll<SelectedTag, SelectableTag, HexTechMapEntityTag>().ForEach((Entity entity, in SelectableTag selectable, in SelectedTag selected, in HexTechMapEntityTag mapEntityTag) => {
            //         parallelWriter.AddComponent(entity.Index, entity, new HexTechCreatePathRequest
            //         {
            //             startPos = mapEntityTag.gridPosition,
            //             endPos = coord
            //         });
            //     }).Run();
            // }

            // SystemAPI.GetSingletonRW<HighlightedAxialCoordsVector4Override>().ValueRW.Value = new float4(coord.q, coord.r, 0, 0);
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

        protected override void OnStartRunning()
        {
            inputSystem = new RTSCameraInputActions();
            inputSystem.Enable();
        }

        protected override void OnStopRunning()
        {
            inputSystem.Disable();
            inputSystem.Dispose();
        }
    }
}