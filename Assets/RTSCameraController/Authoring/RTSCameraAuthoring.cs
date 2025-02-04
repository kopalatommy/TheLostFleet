using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace GalacticBoundStudios.RTSCamera
{
    // Notes:
    // 1. There is an option to include bounds for the camera. Often times, this will be set dynamically based on the game world.
    [CreateAssetMenu(menuName = "RTSCamera/RTSCameraConfig")]
    public class RTSCameraConfig : ScriptableObject
    {
        // Determines how fast the camera moves horizontally
        public float movementSpeed = 20f;
        // Determines how fast the camera rotates
        public float rotationSpeed = 50f;
        // Determines how fast the camera rotates when the right mouse button is held
        public float mouseRotationSpeed = 100f;
        // Determines how fast the camera zooms in and out
        public float zoomSpeed = 10f;
        // Determines how close the cursor has to be to the edge of the screen to trigger edge scrolling
        public float edgeMoveThreshold = 0.05f;

        // Should the bounding component be added
        public bool addBounds = false;
        // Determines the minimum bounds for the camera
        public float3 minBounds = new float3(-100, 5, -100);
        // Determines the maximum bounds for the camera
        public float3 maxBounds = new float3(100, 50, 100);
    }

    public class RTSCameraAuthoring : MonoBehaviour
    {
        [SerializeField]
        protected RTSCameraConfig config;

        protected class Baker : Baker<RTSCameraAuthoring>
        {
            public override void Bake(RTSCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RTSCameraMovementSettings
                {
                    movementSpeed = authoring.config.movementSpeed,
                    rotationSpeed = authoring.config.rotationSpeed,
                    mouseRotationSpeed = authoring.config.mouseRotationSpeed,
                    zoomSpeed = authoring.config.zoomSpeed,
                    edgeMoveThreshold = authoring.config.edgeMoveThreshold
                });

                AddComponent(entity, new RTSCameraMoveData
                {
                    horizontalMovement = float3.zero,
                    zoom = 0,
                    rotation = float3.zero
                });

                if (authoring.config.addBounds)
                {
                    AddComponent(entity, new RTSCameraBounds
                    {
                        minBounds = authoring.config.minBounds,
                        maxBounds = authoring.config.maxBounds
                    });
                }

                AddComponent(entity, new RTSCameraInitialTransformData
                {
                    initialPosition = Camera.main.transform.position,
                    initialRotation = Camera.main.transform.rotation
                });

                AddComponent(entity, new RTSCameraSettings
                {
                    orthographic = Camera.main.orthographic,
                    fieldOfView = Camera.main.fieldOfView,
                    aspect = Camera.main.aspect,
                    orthographicSize = Camera.main.orthographicSize
                });

                AddComponent(entity, new RTSCameraTag());
            }
        }
    }
}