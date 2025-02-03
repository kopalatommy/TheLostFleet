using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.RTSCamera
{
    public struct RTSCameraHorizontalMovementSettings : IComponentData
    {
        public float horizontalAcceleration;
        public float maxHorizontalSpeed;
        public float horizontalDamping;
    }

    public struct RTSCameraZoomSettings : IComponentData
    {
        public float zoomAcceleration;
        public float maxZoomSpeed;
        public float zoomDamping;
    }

    public struct RTSCameraRotationSettings : IComponentData
    {
        public float rotationAcceleration;
        public float maxRotationSpeed;
        public float rotationDamping;
    }

    public struct RTSCameraEdgeScrollSettings : IComponentData
    {
        public float edgeTolerance;
    }

    // This struct defines the camera's position and rotation velocity
    public struct RTSCameraVelocityData : IComponentData
    {
        // Tracked in world space
        public float3 positionVelocity;
        // Tracked in world space
        public quaternion rotationVelocity;
        // Tracked in degrees
        public float zoomVelocity;
    }

    // This holds the input values for camera movement. Each field represents a percentage of the acceleration to apply. 
    public struct RTSCameraInputData : IComponentData
    {
        // This tracks the x and z movement of the camera
        public float3 cameraMoveAcceleration;
        // This tracks the rotation input, in euler angles
        public float3 cameraRotationAcceleration;

        public bool isRotatingByMouse;
        public float2 lastMousePos;
    }

    // This is used to track where the player is pointing with the cursor
    public struct RTSCameraCursorData : IComponentData
    {
        public float3 rayOrigin;
        public float3 rayDirection;
    }

    // This struct defines the camera constraints
    public struct RTSCameraConstraints : IComponentData
    {
        // Defines the min and max height for the camera
        public float2 zoomRange;
        // Defines the min and max vertical angle for the camera
        public float2 verticalAngleBounds;
    }

    // This aspect is used when reading input data for the camera
    public readonly partial struct RTSCameraInputReaderAspect : IAspect
    {
        public readonly Entity entity;
        public readonly RefRW<RTSCameraInputData> inputData;
        public readonly RefRW<RTSCameraCursorData> cursorData;
    }

    // This aspect is used when updating the camera movement
    public readonly partial struct RTSCameraMovementAspect : IAspect
    {
        public readonly Entity entity;
        public readonly RefRW<RTSCameraHorizontalMovementSettings> movementParameters;
        public readonly RefRW<RTSCameraZoomSettings> zoomSettings;
        public readonly RefRW<RTSCameraRotationSettings> rotationSettings;
        public readonly RefRW<RTSCameraEdgeScrollSettings> edgeScrollSettings;
        public readonly RefRW<RTSCameraVelocityData> velocityData;
        public readonly RefRW<RTSCameraInputData> inputData;
        public readonly RefRW<LocalTransform> localTransform;
        public readonly RefRO<RTSCameraConstraints> constraints;
    }
}