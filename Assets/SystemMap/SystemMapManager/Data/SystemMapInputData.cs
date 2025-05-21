using GalacticBoundStudios.HexTech;
using Unity.Collections;
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

    // This struct holds the movement cost data for the system map
    public struct SystemMapMovementCostData : IComponentData
    {
        public NativeHashMap<HexCoord, float> Value;
    }
}