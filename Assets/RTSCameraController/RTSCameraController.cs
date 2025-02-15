using UnityEngine;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.RTSCamera
{
    // Attach me to the target camera object
    public class RTSCameraController : MonoBehaviour
    {
        RTSCameraConfig config;

        RTSCameraInputActions inputSystem = null;

        private void Awake()
        {
            inputSystem = new RTSCameraInputActions();
            inputSystem.Enable();
        }

        void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
            HandleEdgeScrolling();
        }

        void HandleMovement()
        {
            // Get keyboard input for movement
            Vector2 move = inputSystem.HexMap.Move.ReadValue<Vector2>();

            // Calculate movement direction relative to camera orientation
            Vector3 forward = transform.forward;
            forward.y = 0; // Flatten the forward vector to horizontal plane
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0; // Flatten the right vector to horizontal plane
            right.Normalize();

            // Calculate final movement vector
            Vector3 movement = (forward * move.y + right * move.x) * config.movementSpeed * Time.deltaTime;
            transform.position += movement;

            // Clamp the camera's position to the bounds
            if (config.addBounds)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, config.minBounds.x, config.maxBounds.x),
                    Mathf.Clamp(transform.position.y, config.minBounds.y, config.maxBounds.y),
                    Mathf.Clamp(transform.position.z, config.minBounds.z, config.maxBounds.z)
                );
            }
        }

        void HandleRotation()
        {
            // Keyboard rotation with Q and E
            float keyboardRotation = inputSystem.HexMap.Rotate.ReadValue<float>();

            transform.Rotate(Vector3.up * keyboardRotation * config.rotationSpeed * Time.deltaTime, Space.World);

            // Mouse rotation when right mouse button is held
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouse_movement = Mouse.current.delta.ReadValue() * config.mouseRotationSpeed * Time.deltaTime;

                // Horizontal rotation (around world Y axis)
                transform.Rotate(Vector3.up * mouse_movement.x, Space.World);

                // Vertical rotation (around local X axis)
                transform.Rotate(Vector3.right * -mouse_movement.y, Space.Self);
            }
        }

        void HandleZoom()
        {
            float zoomInput = inputSystem.HexMap.Zoom.ReadValue<float>();

            transform.position += transform.forward * zoomInput * config.zoomSpeed * Time.deltaTime;
        }

        // Move the camera if the cursor is on the edge of the screen. Will only add horizontal movement
        void HandleEdgeScrolling()
        {
            // If the player is rotating the camera with the mouse, ignore edge scrolling
            if (Mouse.current.rightButton.isPressed)
            {
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            mousePos.x /= Screen.width;
            mousePos.y /= Screen.height;

            Vector3 moveVector = Vector3.zero;

            // Left right edge scrolling
            if (mousePos.x < config.edgeMoveThreshold)
            {
                moveVector.x -= 1;
            }
            else if (mousePos.x > 1.0f - config.edgeMoveThreshold)
            {
                moveVector.x += 1;
            }

            // Forward and back edge scrolling
            if (mousePos.y < config.edgeMoveThreshold)
            {
                moveVector.z -= 1;
            }
            else if (mousePos.y > 1.0f - config.edgeMoveThreshold)
            {
                moveVector.z += 1;
            }

            // Rotate the move vector by the camera's rotation
            moveVector = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveVector;

            // Move the camera
            transform.position += moveVector * config.movementSpeed * Time.deltaTime;
        }
    }
}
