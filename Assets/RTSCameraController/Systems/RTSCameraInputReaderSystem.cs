using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;
using GalacticBoundStudios.HexTech;

namespace GalacticBoundStudios.RTSCamera
{
    // Make sure this updates before the movement system
    public partial class RTSCameraInputReaderSystem : SystemBase
    {
        static RTSCameraInputActions inputSystem;

        protected override void OnCreate()
        {
            Debug.Log("RTSCameraInputReaderSystem created");

            // Only run the system when there is a container to hold the input data
            RequireForUpdate<RTSCameraMoveData>();

            inputSystem = new RTSCameraInputActions();
            inputSystem.Enable();
        }

        protected override void OnDestroy()
        {
            inputSystem.Disable();
            inputSystem.Dispose();
            inputSystem = null;
        }

        protected override void OnUpdate()
        {
            foreach (var moveData in SystemAPI.Query<RTSCameraAspect>()) {
                moveData.moveData.ValueRW.horizontalMovement = ReadHorizontalMovement(moveData.movementSettings.ValueRO, moveData.localTransform.ValueRO);
                moveData.moveData.ValueRW.horizontalMovement += ReadEdgeScrolling(moveData.movementSettings.ValueRO, moveData.localTransform.ValueRO);
                moveData.moveData.ValueRW.zoom = ReadZoom();
                moveData.moveData.ValueRW.rotation = ReadRotation(moveData.localTransform.ValueRO) + ReadMouseRotation();

                if (inputSystem.HexMap.Click.triggered)
                {
                    if (moveData.cameraSettings.ValueRO.orthographic)
                    {
                        Ray ray = CameraUtilities.ScreenPointToRay_Orthographic(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), moveData.cameraSettings.ValueRO.aspect, moveData.localTransform.ValueRO.Position, moveData.localTransform.ValueRO.Rotation, moveData.cameraSettings.ValueRO.orthographicSize, moveData.localTransform.ValueRO.Forward());
                        float3 intersection = DetermineRayIntersection(ray);
                        HexMapManager.Instance.onSelectHexagon?.Invoke(HexMath.PixelToHex(new float2(intersection.x, intersection.z), HexMapManager.Instance.Config.TransformData));
                    }
                    else
                    {
                        Ray ray = CameraUtilities.ScreenPointToRay_Standard(inputSystem.HexMap.CursorPosition.ReadValue<Vector2>(), moveData.cameraSettings.ValueRO.fieldOfView, moveData.cameraSettings.ValueRO.aspect, moveData.localTransform.ValueRO.Position, moveData.localTransform.ValueRO.Rotation);
                        float3 intersection = DetermineRayIntersection(ray);
                        HexMapManager.Instance.onSelectHexagon?.Invoke(HexMath.PixelToHex(new float2(intersection.x, intersection.z), HexMapManager.Instance.Config.TransformData));
                    }
                }
            }
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
    }
}