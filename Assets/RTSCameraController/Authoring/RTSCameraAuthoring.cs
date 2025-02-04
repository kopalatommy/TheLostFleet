using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.RTSCamera
{
    public class RTSCameraAuthoring : MonoBehaviour
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

        protected class Baker : Baker<RTSCameraAuthoring>
        {
            public override void Bake(RTSCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RTSCameraMovementSettings
                {
                    movementSpeed = authoring.movementSpeed,
                    rotationSpeed = authoring.rotationSpeed,
                    mouseRotationSpeed = authoring.mouseRotationSpeed,
                    zoomSpeed = authoring.zoomSpeed,
                    edgeMoveThreshold = authoring.edgeMoveThreshold
                });

                AddComponent(entity, new RTSCameraMoveData
                {
                    horizontalMovement = float3.zero,
                    zoom = 0,
                    rotation = float3.zero
                });
            }
        }
    }
}