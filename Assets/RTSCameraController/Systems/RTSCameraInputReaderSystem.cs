using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
            RequireForUpdate<RTSCameraInputData>();
            RequireForUpdate<RTSCameraEdgeScrollSettings>();

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
            // Read in the movement vector from the input system. This is input like WASD or arrow keys
            Vector2 moveInput = inputSystem.HexMap.Move.ReadValue<Vector2>();
            // Read in the zoom input. This is input like the mouse wheel
            float zoomInput = inputSystem.HexMap.Zoom.ReadValue<float>();

            // Read in the rotation input. This is input like Q and E
            float2 rotationInput = new float2(inputSystem.HexMap.Rotate.ReadValue<float>(), 0);

            foreach (var aspect in SystemAPI.Query<RTSCameraInputReaderAspect>()) {
                // Start by resetting all current input values
                aspect.inputData.ValueRW.cameraMoveAcceleration = float3.zero;
                aspect.inputData.ValueRW.cameraRotationAcceleration = float3.zero;


                // Horizontal movement
                float3 movement_acceleration = new float3(moveInput.x, 0, moveInput.y);
                ReadEdgeScrolling(EntityManager.GetComponentData<RTSCameraEdgeScrollSettings>(aspect.entity), ref movement_acceleration);
                if (movement_acceleration.x != 0 || movement_acceleration.z != 0)
                {
                    movement_acceleration = math.normalize(movement_acceleration);
                }

                // Vertical moveent
                // Make sure that the zoom input is analog. It should be either -1, 0, or 1
                movement_acceleration.y = zoomInput;

                // Rotation Input
                // The mouse will override the keyboard input for rotation
                float2 rot_acceleration = HandleRotateByMouse(ref aspect.inputData.ValueRW);

                if (!aspect.inputData.ValueRW.isRotatingByMouse)
                {
                    rot_acceleration = rotationInput;
                }


                // Update the data container with the updated values
                aspect.inputData.ValueRW.cameraMoveAcceleration = movement_acceleration;
                aspect.inputData.ValueRW.cameraRotationAcceleration = new float3(rot_acceleration.y, rot_acceleration.x, 0);
            }
        }

        // Handle reading edge scrolling input. Returns true if the resulting vector is not zero
        void ReadEdgeScrolling(in RTSCameraEdgeScrollSettings settings, ref float3 moveVector)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (Mouse.current.rightButton.isPressed)
            {
                return;
            }

            // Horizontal edge scrolling
            if (mousePos.x < (settings.edgeTolerance * Screen.width))
            {
                moveVector.x += -1;
            }
            else if (mousePos.x > ((1.0f - settings.edgeTolerance) * Screen.width))
            {
                moveVector.x += 1;
            }

            // Vertical edge scrolling
            if (mousePos.y < (settings.edgeTolerance * Screen.height))
            {
                moveVector.z += -1;
            }
            else if (mousePos.y > ((1.0f - settings.edgeTolerance) * Screen.height))
            {
                moveVector.z += 1;
            }
        }

        // Read in camera rotation input from the mouse. Returns a float2 based on the percentage of the screen the mouse has moved. Returns values in range of [-1 to 1]
        float2 HandleRotateByMouse(ref RTSCameraInputData inputData)
        {
            if (inputData.isRotatingByMouse)
            {
                if (Mouse.current.rightButton.isPressed)
                {
                    // Read the mouse delta
                    float2 currentMousePos = Mouse.current.position.ReadValue();

                    // Update the last mouse position
                    float2 delta = currentMousePos - inputData.lastMousePos;
                    inputData.lastMousePos = currentMousePos;

                    // Normalize the delta to be -1 to 1
                    delta.x /= Screen.width;
                    delta.y /= Screen.height;

                    return delta;
                }
                else
                {
                    inputData.isRotatingByMouse = false;
                }
            }
            else
            {
                if (Mouse.current.rightButton.isPressed)
                {
                    inputData.isRotatingByMouse = true;

                    // Read the mouse position
                    float2 currentMousePos = Mouse.current.position.ReadValue();
                    inputData.lastMousePos = currentMousePos;
                }
            }

            return new float2(0, 0);
        }


        public quaternion RotateByDegrees(quaternion original, float degrees, float3 axis)
        {
            // Convert degrees to radians
            float radians = math.radians(degrees);

            // Create a rotation quaternion around the specified axis
            quaternion rotation = quaternion.AxisAngle(axis, radians);

            // Apply the rotation to the original quaternion
            quaternion result = math.mul(original, rotation);

            return result;
        }

        protected RTSCameraCursorData GetCursorData(Vector3 mousePos)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera.orthographic) {
                return ScreenPointToRay_Orthographic(mousePos, mainCamera.aspect, mainCamera.transform.position, mainCamera.transform.rotation, mainCamera.orthographicSize, mainCamera.transform.forward);
            } else {
                return ScreenPointToRay_Standard(mousePos, mainCamera.fieldOfView, mainCamera.aspect, mainCamera.transform.position, mainCamera.transform.rotation);
            }
        }

        public RTSCameraCursorData ScreenPointToRay_Standard(Vector3 screenPos, float fieldOfView, float aspect, Vector3 position, Quaternion rotation)
        {
            // Remap so (0, 0) is the center of the window,
            // and the edges are at -0.5 and +0.5.
            Vector2 relative = new Vector2(
                screenPos.x / Screen.width - 0.5f,
                screenPos.y / Screen.height - 0.5f
            );

            // Angle in radians from the view axis
            // to the top plane of the view pyramid.
            float verticalAngle = 0.5f * Mathf.Deg2Rad * fieldOfView;

            // World space height of the view pyramid
            // measured at 1 m depth from the camera.
            float worldHeight = 2f * Mathf.Tan(verticalAngle);

            // Convert relative position to world units.
            Vector3 worldUnits = relative * worldHeight;
            worldUnits.x *= aspect;
            worldUnits.z = 1;

            // Rotate to match camera orientation.
            Vector3 direction = rotation * worldUnits;

            // Output a ray from camera position, along this direction.
            return new RTSCameraCursorData()
            {
                rayOrigin = position,
                rayDirection = direction
            };
        }

        // https://gamedev.stackexchange.com/questions/194575/what-is-the-logic-behind-of-screenpointtoray
        public RTSCameraCursorData ScreenPointToRay_Orthographic(Vector3 screenPos, float aspect, Vector3 position, Quaternion rotation, float orthographicSize, Vector3 forward)
        {
            // Remap so (0, 0) is the center of the window,
            // and the edges are at -0.5 and +0.5.
            Vector2 relative = new Vector2(
                screenPos.x / Screen.width - 0.5f,
                screenPos.y / Screen.height - 0.5f
            );

            // Scale using half-height of camera.
            Vector3 worldUnits = relative * orthographicSize * 2f;
            worldUnits.x *= aspect;

            // Orient and position to match camera transform.
            Vector3 origin = rotation * worldUnits;
            origin += position;
            
            // Output a ray from this point, along camera's axis.
            return new RTSCameraCursorData()
            {
                rayOrigin = origin,
                rayDirection = forward
            };
        }
    }
}