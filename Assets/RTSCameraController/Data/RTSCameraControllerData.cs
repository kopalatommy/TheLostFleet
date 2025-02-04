using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GalacticBoundStudios.RTSCamera
{
    public struct RTSCameraMovementSettings : IComponentData
    {
        // Determines how fast the camera moves horizontally
        public float movementSpeed;
        // Determines how fast the camera rotates
        public float rotationSpeed;
        // Determines how fast the camera rotates when the right mouse button is held
        public float mouseRotationSpeed;
        // Determines how fast the camera zooms in and out
        public float zoomSpeed;
        // Determines how close the cursor has to be to the edge of the screen to trigger edge scrolling
        public float edgeMoveThreshold;
    }

    public struct RTSCameraMoveData : IComponentData
    {
        public float3 horizontalMovement;
        public float zoom;
        public float3 rotation;
    }

    // This component defines the bounds for the camera
    public struct RTSCameraBounds : IComponentData
    {
        public float3 minBounds;
        public float3 maxBounds;
    }

    public readonly partial struct RTSCameraAspect : IAspect
    {
        public readonly Entity entity;
        public readonly RefRW<LocalTransform> localTransform;

        public readonly RefRO<RTSCameraMovementSettings> movementSettings;
        public readonly RefRW<RTSCameraMoveData> moveData;
        public readonly RefRO<RTSCameraSettings> cameraSettings;
    }

    public struct RTSCameraInitialTransformData : IComponentData
    {
        public float3 initialPosition;
        public quaternion initialRotation;
    }

    public struct RTSCameraSettings : IComponentData
    {
        public bool orthographic;
        public float fieldOfView;
        public float aspect;

        public float orthographicSize;
    }

    public struct RTSCameraTag : IComponentData
    {

    }
}