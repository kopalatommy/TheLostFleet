using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.RTSCamera
{
    public class RTSCameraAuthoring : MonoBehaviour
    {
        [Header("Camera Movement Parameters")]

        [Header("Horizontal Movement")]
        [SerializeField]
        protected float horizontalAcceleration = 5;
        [SerializeField]
        protected float maxHorizontalSpeed = 10;
        [SerializeField]
        protected float horizontalDamping = 15;

        [Header("Zoom")]
        [SerializeField]
        protected float zoomAcceleration = 2f;
        [SerializeField]
        protected float maxZoomSpeed = 5;
        [SerializeField]
        protected float zoomDamping = 75f;
        [SerializeField]
        protected float minHeight = 5;
        [SerializeField]
        protected float maxHeight = 20;

        [Header("Rotation")]
        [SerializeField]
        protected float rotationAcceleration = 2f;
        [SerializeField]
        protected float maxRotationSpeed = 8;
        [SerializeField]
        protected float rotationDamping = 7.5f;

        [Header("Edge Scrolling")]
        [SerializeField]
        protected float edgeTolerance = 0.05f;

        [Header("Constraints")]
        [SerializeField]
        protected float2 zoomRange = new float2(5, 20);
        [SerializeField]
        protected float2 verticalAngleBounds = new float2(0, 90);

        protected class Baker : Baker<RTSCameraAuthoring>
        {
            public override void Bake(RTSCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RTSCameraHorizontalMovementSettings
                {
                    horizontalAcceleration = authoring.horizontalAcceleration,
                    maxHorizontalSpeed = authoring.maxHorizontalSpeed,
                    horizontalDamping = authoring.horizontalDamping,
                });

                AddComponent(entity, new RTSCameraZoomSettings
                {
                    zoomAcceleration = authoring.zoomAcceleration,
                    maxZoomSpeed = authoring.maxZoomSpeed,
                    zoomDamping = authoring.zoomDamping,
                });

                AddComponent(entity, new RTSCameraRotationSettings
                {
                    rotationAcceleration = authoring.rotationAcceleration,
                    maxRotationSpeed = authoring.maxRotationSpeed,
                    rotationDamping = authoring.rotationDamping,
                });

                AddComponent(entity, new RTSCameraEdgeScrollSettings
                {
                    edgeTolerance = authoring.edgeTolerance
                });

                AddComponent(entity, new RTSCameraInputData
                {
                    cameraMoveAcceleration = new int3(0, 0, 0),
                    cameraRotationAcceleration = new int3(0, 0, 0),

                    isRotatingByMouse = false,
                    lastMousePos = new float2(0, 0)
                });

                AddComponent(entity, new RTSCameraVelocityData
                {
                    positionVelocity = new float3(0, 0, 0),
                    rotationVelocity = quaternion.identity,
                    zoomVelocity = 0
                });

                AddComponent(entity, new RTSCameraCursorData
                {
                    rayOrigin = new float3(0, 0, 0),
                    rayDirection = new float3(0, 0, 0)
                });

                AddComponent(entity, new RTSCameraConstraints
                {
                    zoomRange = authoring.zoomRange,
                    verticalAngleBounds = authoring.verticalAngleBounds
                });
            }
        }
    }
}