using GalacticBoundStudios.HexTech;
using Unity.Entities;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    public struct SystemMapInputData : IComponentData
    {
        
    }

    // This data tracks what hexagon the cursor is pointing at
    public struct SystemMapFocusHexagonData : IComponentData
    {
        public HexCoord coord;
    }

    // This data tracks which hexagon is currently selected
    public struct SystemMapSelectedHexagonData : IComponentData
    {
        public HexCoord coord;
    }
}