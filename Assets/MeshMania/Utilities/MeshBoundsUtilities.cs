

using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.MeshMania
{
    public static class MeshBoundUtilities
    {
        // Helper to transform Unity.Bounds to Unity.Mathematics.AABB
        public static AABB TransformBounds(float4x4 matrix, Bounds bounds)
        {
            float3 size = bounds.size;
            float3 extents = size * 0.5f;
            float3 center = bounds.center;

            // Transform the 8 corners of the local bounding box to world space
            float3 p0 = math.transform(matrix, center + new float3(-extents.x, -extents.y, -extents.z));
            float3 p1 = math.transform(matrix, center + new float3( extents.x, -extents.y, -extents.z));
            float3 p2 = math.transform(matrix, center + new float3(-extents.x,  extents.y, -extents.z));
            float3 p3 = math.transform(matrix, center + new float3( extents.x,  extents.y, -extents.z));
            float3 p4 = math.transform(matrix, center + new float3(-extents.x, -extents.y,  extents.z));
            float3 p5 = math.transform(matrix, center + new float3( extents.x, -extents.y,  extents.z));
            float3 p6 = math.transform(matrix, center + new float3(-extents.x,  extents.y,  extents.z));
            float3 p7 = math.transform(matrix, center + new float3( extents.x,  extents.y,  extents.z));

            // Find the min and max of the transformed corners to create the new AABB
            float3 min = math.min(math.min(math.min(p0, p1), math.min(p2, p3)), math.min(math.min(p4, p5), math.min(p6, p7)));
            float3 max = math.max(math.max(math.max(p0, p1), math.max(p2, p3)), math.max(math.max(p4, p5), math.max(p6, p7)));

            return new AABB { Center = (min + max) * 0.5f, Extents = (max - min) * 0.5f };
        }
    }
}