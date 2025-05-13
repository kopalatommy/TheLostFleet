using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace GalacticBoundStudios.HexTech.Shaders
{
    [MaterialProperty("_HighlightedAxialCoords")]
    public struct HighlightedAxialCoordsVector4Override : IComponentData
    {
        public float4 Value;
    }
}