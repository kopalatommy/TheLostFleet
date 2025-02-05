using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GalacticBoundStudios.RTSCamera
{
    // Make sure this updates after the input reader system
    [UpdateAfter(typeof(RTSCameraInputReaderSystem))]
    // This system is responsible for reading in the player input data and determining the camera's acceleration
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

                // Apply the movement settings to the camera
                position += aspect.moveData.ValueRO.horizontalMovement * aspect.movementSettings.ValueRO.movementSpeed * deltaTime;

                // Apply the zoom settings to the camera
                position += aspect.localTransform.ValueRW.Forward() * aspect.moveData.ValueRO.zoom * aspect.movementSettings.ValueRO.zoomSpeed * deltaTime;
                
                aspect.localTransform.ValueRW.Position = position;

                // Apply rotation around the x-axis
                aspect.localTransform.ValueRW = aspect.localTransform.ValueRW.Rotate(quaternion.Euler(math.radians(new float3(1, 0, 0)) * aspect.moveData.ValueRO.rotation.x * aspect.movementSettings.ValueRO.rotationSpeed * deltaTime));

                // Apply rotation around the y-axis
                aspect.localTransform.ValueRW = aspect.localTransform.ValueRW.Rotate(quaternion.Euler(math.radians(new float3(0, 1, 0)) * aspect.moveData.ValueRO.rotation.y * aspect.movementSettings.ValueRO.rotationSpeed * deltaTime));

                // Ensure no rotation occurs around the z-axis
                float3 euler = math.degrees(math.Euler(aspect.localTransform.ValueRW.Rotation));
                euler.z = 0;
                aspect.localTransform.ValueRW.Rotation = quaternion.Euler(math.radians(euler));

                // Update the Camera GameObject
                Camera.main.transform.position = position;
                Camera.main.transform.rotation = aspect.localTransform.ValueRO.Rotation;
            }
        }
    }
}