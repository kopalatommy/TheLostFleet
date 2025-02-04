using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.RTSCamera
{
    public static class CameraUtilities
    {
        public static Ray ScreenPointToRay_Orthographic(Vector3 screenPoint, float aspect, Vector3 cameraPosition, Quaternion cameraRotation, float orthographicSize, Vector3 cameraForward)
        {
            Vector3 origin = cameraPosition + cameraForward * orthographicSize;
            Vector3 direction = new Vector3((screenPoint.x / Screen.width - 0.5f) * aspect, (screenPoint.y / Screen.height - 0.5f), 0);
            direction = cameraRotation * direction;
            return new Ray(origin, direction);
        }

        public static Ray ScreenPointToRay_Standard(Vector3 screenPos, float fieldOfView, float aspect, Vector3 position, Quaternion rotation)
        {
            // Remap so (0, 0) is the center of the window,
            // and the edges are at -0.5 and +0.5.
            Vector2 relative = new Vector2(
                screenPos.x / Screen.width - 0.5f,
                screenPos.y / Screen.height - 0.5f
            );

            // Angle in radians from the view axis
            // to the top plane of the view pyramid.
            float verticalAngle = 0.5f * Mathf.Deg2Rad * fieldOfView;

            // World space height of the view pyramid
            // measured at 1 m depth from the camera.
            float worldHeight = 2f * Mathf.Tan(verticalAngle);

            // Convert relative position to world units.
            Vector3 worldUnits = relative * worldHeight;
            worldUnits.x *= aspect;
            worldUnits.z = 1;

            // Rotate to match camera orientation.
            Vector3 direction = rotation * worldUnits;

            // Output a ray from camera position, along this direction.
            return new Ray(position, direction);
        }

        public static float3 DetermineWhereRayIntersectsPlain(in Ray ray, float3 planePoint, float3 planeNormal)
        {
            float denominator = math.dot(ray.direction, planeNormal);

            if (math.abs(denominator) < 1e-6)
            {
                return float3.zero;
            }

            float3 origin = ray.origin;
            float3 difference = planePoint - origin;
            float t = math.dot(difference, planeNormal) / denominator;
            return ray.origin + ray.direction * t;
        }
    }
}