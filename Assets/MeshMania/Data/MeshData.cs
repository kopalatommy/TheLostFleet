using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.DataScribes;

namespace GalacticBoundStudios.MeshMania
{
    public struct MeshData : IComponentData
    {
        public FixedArray<float3> vertices;
        public FixedArray<int> triangles;
        public FixedArray<float3> normals;
        public FixedArray<float4> colors;
        //public FixedArray<float2> uvs;
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