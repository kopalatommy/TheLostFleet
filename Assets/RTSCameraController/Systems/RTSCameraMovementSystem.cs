using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

namespace GalacticBoundStudios.RTSCamera
{
    // Make sure this updates after the input reader system
    [UpdateAfter(typeof(RTSCameraInputReaderSystem))]
    // This system is responsible for reading in the player input data and determining the camera's acceleration
    public partial struct RTSCameraMovementSystem : ISystem, ISystemStartStop
    {
        public void OnCreate(ref SystemState state)
        {
            // Only run the system when there is a container to hold the input data
            state.RequireForUpdate<RTSCameraMoveData>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var aspect in SystemAPI.Query<RTSCameraAspect>())
            {
                if (state.EntityManager.HasComponent<RTSCameraInitialTransformData>(aspect.entity))
                {
                    RTSCameraInitialTransformData initialTransformData = state.EntityManager.GetComponentData<RTSCameraInitialTransformData>(aspect.entity);

                    aspect.localTransform.ValueRW.Position = initialTransformData.initialPosition;
                    aspect.localTransform.ValueRW.Rotation = initialTransformData.initialRotation;

                    ecb.RemoveComponent<RTSCameraInitialTransformData>(aspect.entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        public void OnStopRunning(ref SystemState state)
        {

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

                // Apply the zoom settings to the camera
                position += aspect.localTransform.ValueRW.Forward() * aspect.moveData.ValueRO.zoom * aspect.movementSettings.ValueRO.zoomSpeed * deltaTime;

                // Check if there is bounds data
                if (state.EntityManager.HasComponent<RTSCameraBounds>(aspect.entity)) {
                    RTSCameraBounds bounds = state.EntityManager.GetComponentData<RTSCameraBounds>(aspect.entity);

                    // Clamp the camera's position to the bounds
                    position = math.clamp(position, bounds.minBounds, bounds.maxBounds);
                }

                // Calculate the rotation around the x-axis
                quaternion xRotation = quaternion.AxisAngle(new float3(1, 0, 0), math.radians(aspect.moveData.ValueRO.rotation.x * aspect.movementSettings.ValueRO.rotationSpeed * deltaTime));

                // Calculate the rotation around the y-axis
                quaternion yRotation = quaternion.AxisAngle(new float3(0, 1, 0), math.radians(aspect.moveData.ValueRO.rotation.y * aspect.movementSettings.ValueRO.rotationSpeed * deltaTime));

                // Combine the rotations
                quaternion combinedRotation = math.mul(yRotation, xRotation);

                // Apply the combined rotation to the local transform
                rotation = math.mul(rotation, combinedRotation);

                // Ensure no rotation occurs around the z-axis
                float3 euler = math.degrees(math.Euler(rotation));
                euler.z = 0;

                // Apply bounds to the rotation around the x-axis
                // Check if there is bounds data
                if (state.EntityManager.HasComponent<RTSCameraBounds>(aspect.entity)) {
                    RTSCameraBounds bounds = state.EntityManager.GetComponentData<RTSCameraBounds>(aspect.entity);

                    // Clamp the camera's position to the bounds
                    euler.x = math.clamp(euler.x, bounds.rotationBounds.x, bounds.rotationBounds.y);

                    Debug.Log(euler.x);
                }

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