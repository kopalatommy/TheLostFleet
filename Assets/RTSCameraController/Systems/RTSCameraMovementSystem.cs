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
            state.RequireForUpdate<RTSCameraMoveData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // Get the delta time
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var aspect in SystemAPI.Query<RTSCameraAspect>()) {
                float3 position = aspect.localTransform.ValueRO.Position;
                quaternion rotation = aspect.localTransform.ValueRO.Rotation;

                // Apply the movement settings to the camera
                position += aspect.moveData.ValueRO.horizontalMovement * aspect.movementSettings.ValueRO.movementSpeed * deltaTime;

                // Apply the rotation settings to the camera
                rotation = math.mul(rotation, quaternion.Euler(math.radians(aspect.moveData.ValueRO.rotation * aspect.movementSettings.ValueRO.rotationSpeed * deltaTime)));

                // Clamp the camera's Y rotation
                float3 euler = math.degrees(math.Euler(rotation));
                euler.x = math.clamp(euler.x, -89, 89);
                rotation = quaternion.Euler(math.radians(euler));

                // Update the camera's position and rotation
                aspect.localTransform.ValueRW.Position = position;
                aspect.localTransform.ValueRW.Rotation = rotation;

                // Update the Camera GameObject
                Camera.main.transform.position = position;
                Camera.main.transform.rotation = rotation;
            }
        }
    }
}