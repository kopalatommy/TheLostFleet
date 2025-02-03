using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GalacticBoundStudios.RTSCamera
{
    // Make sure this updates after the input reader system
    [UpdateAfter(typeof(RTSCameraInputReaderSystem))]


    // This system is resposible for reading in the player input data and determining the camera's acceleration
    public partial struct RTSCameraMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // Only run the system when there is a container to hold the input data
            state.RequireForUpdate<RTSCameraInputData>();
            state.RequireForUpdate<RTSCameraHorizontalMovementSettings>();
            state.RequireForUpdate<RTSCameraZoomSettings>();
            state.RequireForUpdate<RTSCameraRotationSettings>();
            state.RequireForUpdate<RTSCameraVelocityData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // Get the delta time
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var aspect in SystemAPI.Query<RTSCameraMovementAspect>()) {
                // Get the camera velocity data
                RTSCameraVelocityData cameraVelocityData = aspect.velocityData.ValueRW;

                float horizontalInput = aspect.inputData.ValueRO.cameraMoveAcceleration.y;
                aspect.inputData.ValueRW.cameraMoveAcceleration.y = 0;

                // Handle updating the camera velocity data
                cameraVelocityData.positionVelocity = DetermineHorizontalMovementVelocity(in aspect.inputData.ValueRO.cameraMoveAcceleration, in cameraVelocityData.positionVelocity, in aspect.localTransform.ValueRW.Rotation, in aspect.movementParameters.ValueRO);
                // Handle updating the camera zoom velocity data
                cameraVelocityData.zoomVelocity = DetermineZoomVelocity(horizontalInput, cameraVelocityData.zoomVelocity, in aspect.zoomSettings.ValueRO);
                // Handle updating the camera rotation velocity data
                cameraVelocityData.rotationVelocity = DetermineRotationVelocity(in aspect.inputData.ValueRO.cameraRotationAcceleration, in cameraVelocityData.rotationVelocity, in aspect.rotationSettings.ValueRO);

                // Update the camera position, updates x and z
                aspect.localTransform.ValueRW.Position += cameraVelocityData.positionVelocity * deltaTime;
                // Update the camera zoom
                aspect.localTransform.ValueRW.Position.y += cameraVelocityData.zoomVelocity * deltaTime;
                // Update the camera rotation

                // Remove all z rotation from the delta
                float3 euler = math.EulerZXY(math.mul(aspect.localTransform.ValueRW.Rotation, cameraVelocityData.rotationVelocity));
                euler.z = 0;
                if (euler.x < 0f)
                {
                    euler.x = 0f;
                }
                else if (euler.x > 1.4f)
                {
                    euler.x = 1.4f;
                }

                quaternion newRotationDelta = quaternion.EulerZXY(euler);

                aspect.localTransform.ValueRW.Rotation = newRotationDelta;

                Camera mainCamera = Camera.main;
                // Update the camera's position and rotation
                mainCamera.transform.position = aspect.localTransform.ValueRW.Position;
                mainCamera.transform.rotation = aspect.localTransform.ValueRW.Rotation;
            }
        }

        float3 DetermineHorizontalMovementVelocity(in float3 moveVector, in float3 currentVelocity, in quaternion currentRotation, in RTSCameraHorizontalMovementSettings settings)
        {
            // If the move vector is not zero, then we need to set the velocity to the max speed
            if (moveVector.x != 0 || moveVector.y != 0 || moveVector.z != 0) {
                float3 rotatedMoveVector = math.mul(currentRotation, moveVector);
                rotatedMoveVector.y = 0;
                rotatedMoveVector = math.normalize(rotatedMoveVector);

                float3 newMoveSpeed = (rotatedMoveVector * settings.horizontalAcceleration) + currentVelocity;
                float newMoveSpeedMagnitude = math.length(newMoveSpeed);
                if (newMoveSpeedMagnitude > settings.maxHorizontalSpeed)
                {
                    return math.normalize(newMoveSpeed) * settings.maxHorizontalSpeed;
                }
                else
                {
                    return newMoveSpeed;
                }
            } else {
                // If the move vector is zero, then we need to apply damping to the velocity
                return currentVelocity * settings.horizontalDamping;
            }
        }

        float DetermineZoomVelocity(float zoomInput, float currentZoomVelocity, in RTSCameraZoomSettings settings)
        {
            // If the zoom delta is not zero, then we need to set the velocity to the max speed
            if (zoomInput != 0) {
                return math.clamp((zoomInput * settings.zoomAcceleration) + currentZoomVelocity, -settings.maxZoomSpeed, settings.maxZoomSpeed);
            } else {
                // If the zoom delta is zero, then we need to apply damping to the velocity
                return currentZoomVelocity * settings.zoomDamping;
            }
        }

        bool QuaternionsAreEqual(quaternion q1, quaternion q2, float tolerance = 1e-6f)
        {
            return math.all(math.abs(q1.value - q2.value) < tolerance);
        }

        public static float GetQuaternionAngleInDegrees(quaternion q)
        {
            // Convert quaternion to axis-angle representation
            float angleInRadians = 2.0f * Mathf.Acos(q.value.w);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

            return angleInDegrees;
        }

        public static quaternion ClampQuaternionAngle(quaternion q, float maxAngleInDegrees)
        {
            float angleInDegrees = GetQuaternionAngleInDegrees(q);
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // Clamp the angle
            if (angleInDegrees > maxAngleInDegrees)
            {
                float clampedAngleInRadians = maxAngleInDegrees * Mathf.Deg2Rad;
                float scale = Mathf.Sin(clampedAngleInRadians / 2.0f) / Mathf.Sin(angleInRadians / 2.0f);

                // Scale the quaternion's vector part
                quaternion clampedQuaternion = new quaternion(q.value.x * scale, q.value.y * scale, q.value.z * scale, Mathf.Cos(clampedAngleInRadians / 2.0f));
                return clampedQuaternion;
            }

            return q;
        }

        quaternion DetermineRotationVelocity(in float3 rotationDelta, in quaternion currentRotationVelocity, in RTSCameraRotationSettings settings)
        {
            // If the rotation delta is not zero, then we need to set the velocity to the max speed
            if (rotationDelta.x != 0 || rotationDelta.y != 0 || rotationDelta.z != 0) {
                // Scale the rotation delta by the acceleration and convert to quaternion
                quaternion scaledRotationDelta = quaternion.Euler(math.radians(rotationDelta * settings.rotationAcceleration));

                // Combine the scaled rotation delta with the current rotation velocity
                quaternion newRotationVelocity = math.mul(scaledRotationDelta, currentRotationVelocity);

                // Make sure the camera does not start rotating too fast
                if (GetQuaternionAngleInDegrees(newRotationVelocity) > settings.maxRotationSpeed)
                {
                    newRotationVelocity = ClampQuaternionAngle(newRotationVelocity, settings.maxRotationSpeed);
                }

                // Normalize the quaternion to avoid drift
                return math.normalize(newRotationVelocity);
            } else {
                // If the rotation delta is zero, then we need to apply damping to the velocity
                return math.slerp(currentRotationVelocity, quaternion.identity, settings.rotationDamping);
            }
        }
    }
}