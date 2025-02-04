using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;

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
                moveData.moveData.ValueRW.rotation = ReadRotation(moveData.localTransform.ValueRO) + ReadMouseRotation() + ReadEdgeScrolling(moveData.movementSettings.ValueRO, moveData.localTransform.ValueRO);
            }
        }

        float3 ReadHorizontalMovement(in RTSCameraMovementSettings moveSettings, in LocalTransform localTransform)
        {
            float2 move = inputSystem.HexMap.Move.ReadValue<Vector2>();

            // Calculate movement direction relative to camera orientation
            float3 forward = localTransform.Forward();
            forward.y = 0; // Flatten the forward vector to horizontal plane
            forward = math.normalize(forward);

            float3 right = localTransform.Right();
            right.y = 0; // Flatten the right vector to horizontal plane
            right = math.normalize(right);

            // Calculate final movement vector
            float3 movement = forward * move.y + right * move.x;

            return math.normalize(movement);
        }

        float3 ReadRotation(in LocalTransform localTransform)
        {
            float keyboardInput = inputSystem.HexMap.Rotate.ReadValue<float>();

            return localTransform.Up() * keyboardInput;
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

            float3 moveVector = float3.zero;

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
                moveVector.z -= 1;
            }
            else if (mousePos.y > 1 - settings.edgeMoveThreshold)
            {
                moveVector.z += 1;
            }
            
            float3 forward = localTransform.Forward();
            forward.y = 0; // Flatten the forward vector to horizontal plane
            forward = math.normalize(forward);

            float3 right = localTransform.Right();
            right.y = 0; // Flatten the right vector to horizontal plane
            right = math.normalize(right);

            // Calculate final movement vector
            float3 movement = forward * moveVector.z + right * moveVector.x;

            return math.normalize(movement);
        }












        

        // // Handle reading edge scrolling input. Returns true if the resulting vector is not zero
        // void ReadEdgeScrolling(in RTSCameraEdgeScrollSettings settings, ref float3 moveVector)
        // {
        //     Vector2 mousePos = Mouse.current.position.ReadValue();

        //     if (Mouse.current.rightButton.isPressed)
        //     {
        //         return;
        //     }

        //     // Horizontal edge scrolling
        //     if (mousePos.x < (settings.edgeTolerance * Screen.width))
        //     {
        //         moveVector.x += -1;
        //     }
        //     else if (mousePos.x > ((1.0f - settings.edgeTolerance) * Screen.width))
        //     {
        //         moveVector.x += 1;
        //     }

        //     // Vertical edge scrolling
        //     if (mousePos.y < (settings.edgeTolerance * Screen.height))
        //     {
        //         moveVector.z += -1;
        //     }
        //     else if (mousePos.y > ((1.0f - settings.edgeTolerance) * Screen.height))
        //     {
        //         moveVector.z += 1;
        //     }
        // }

        // // Read in camera rotation input from the mouse. Returns a float2 based on the percentage of the screen the mouse has moved. Returns values in range of [-1 to 1]
        // float2 HandleRotateByMouse(ref RTSCameraInputData inputData)
        // {
        //     if (inputData.isRotatingByMouse)
        //     {
        //         if (Mouse.current.rightButton.isPressed)
        //         {
        //             // Read the mouse delta
        //             float2 currentMousePos = Mouse.current.position.ReadValue();

        //             // Update the last mouse position
        //             float2 delta = currentMousePos - inputData.lastMousePos;
        //             inputData.lastMousePos = currentMousePos;

        //             // Normalize the delta to be -1 to 1
        //             delta.x /= Screen.width;
        //             delta.y /= Screen.height;

        //             return delta;
        //         }
        //         else
        //         {
        //             inputData.isRotatingByMouse = false;
        //         }
        //     }
        //     else
        //     {
        //         if (Mouse.current.rightButton.isPressed)
        //         {
        //             inputData.isRotatingByMouse = true;

        //             // Read the mouse position
        //             float2 currentMousePos = Mouse.current.position.ReadValue();
        //             inputData.lastMousePos = currentMousePos;
        //         }
        //     }

        //     return new float2(0, 0);
        // }


        // public quaternion RotateByDegrees(quaternion original, float degrees, float3 axis)
        // {
        //     // Convert degrees to radians
        //     float radians = math.radians(degrees);

        //     // Create a rotation quaternion around the specified axis
        //     quaternion rotation = quaternion.AxisAngle(axis, radians);

        //     // Apply the rotation to the original quaternion
        //     quaternion result = math.mul(original, rotation);

        //     return result;
        // }

        // protected RTSCameraCursorData GetCursorData(Vector3 mousePos)
        // {
        //     Camera mainCamera = Camera.main;
        //     if (mainCamera.orthographic) {
        //         return ScreenPointToRay_Orthographic(mousePos, mainCamera.aspect, mainCamera.transform.position, mainCamera.transform.rotation, mainCamera.orthographicSize, mainCamera.transform.forward);
        //     } else {
        //         return ScreenPointToRay_Standard(mousePos, mainCamera.fieldOfView, mainCamera.aspect, mainCamera.transform.position, mainCamera.transform.rotation);
        //     }
        // }

        // public RTSCameraCursorData ScreenPointToRay_Standard(Vector3 screenPos, float fieldOfView, float aspect, Vector3 position, Quaternion rotation)
        // {
        //     // Remap so (0, 0) is the center of the window,
        //     // and the edges are at -0.5 and +0.5.
        //     Vector2 relative = new Vector2(
        //         screenPos.x / Screen.width - 0.5f,
        //         screenPos.y / Screen.height - 0.5f
        //     );

        //     // Angle in radians from the view axis
        //     // to the top plane of the view pyramid.
        //     float verticalAngle = 0.5f * Mathf.Deg2Rad * fieldOfView;

        //     // World space height of the view pyramid
        //     // measured at 1 m depth from the camera.
        //     float worldHeight = 2f * Mathf.Tan(verticalAngle);

        //     // Convert relative position to world units.
        //     Vector3 worldUnits = relative * worldHeight;
        //     worldUnits.x *= aspect;
        //     worldUnits.z = 1;

        //     // Rotate to match camera orientation.
        //     Vector3 direction = rotation * worldUnits;

        //     // Output a ray from camera position, along this direction.
        //     return new RTSCameraCursorData()
        //     {
        //         rayOrigin = position,
        //         rayDirection = direction
        //     };
        // }

        // // https://gamedev.stackexchange.com/questions/194575/what-is-the-logic-behind-of-screenpointtoray
        // public RTSCameraCursorData ScreenPointToRay_Orthographic(Vector3 screenPos, float aspect, Vector3 position, Quaternion rotation, float orthographicSize, Vector3 forward)
        // {
        //     // Remap so (0, 0) is the center of the window,
        //     // and the edges are at -0.5 and +0.5.
        //     Vector2 relative = new Vector2(
        //         screenPos.x / Screen.width - 0.5f,
        //         screenPos.y / Screen.height - 0.5f
        //     );

        //     // Scale using half-height of camera.
        //     Vector3 worldUnits = relative * orthographicSize * 2f;
        //     worldUnits.x *= aspect;

        //     // Orient and position to match camera transform.
        //     Vector3 origin = rotation * worldUnits;
        //     origin += position;
            
        //     // Output a ray from this point, along camera's axis.
        //     return new RTSCameraCursorData()
        //     {
        //         rayOrigin = origin,
        //         rayDirection = forward
        //     };
        // }
    }
}