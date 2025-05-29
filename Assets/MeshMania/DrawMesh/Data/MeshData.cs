using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.DataScribes;
using Unity.Collections;

namespace GalacticBoundStudios.MeshMania
{
    public struct MeshData : IComponentData
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<float3> vertices;
        [NativeDisableParallelForRestriction]
        public NativeArray<int> triangles;
        [NativeDisableParallelForRestriction]
        public NativeArray<float3> normals;
        [NativeDisableParallelForRestriction]
        public NativeArray<float4> colors;
        //public FixedArray<float2> uvs;

        // This is used to embed meta data in the mesh
        public bool addUV2;
        [NativeDisableParallelForRestriction]
        public NativeArray<float4> uv2;
    }

    // This data is used to update the correct mesh data
    public struct MeshIndexData : IComponentData
    {
        public int index;
    }

    // This component is used to tag entities that should be drawn
    // so that the DrawMeshSystem can find them
    public struct DrawMeshTag : IComponentData
    {
    }
}