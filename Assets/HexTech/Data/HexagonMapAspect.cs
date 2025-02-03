using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace GalacticBoundStudios.HexTech
{
    public readonly partial struct HexagonMapAspect : IAspect
    {
        // Reference the entity that holds the hexagon map data
        public readonly Entity entity;

        [NativeDisableUnsafePtrRestriction]
        // This data holds the map rotation data
        public readonly RefRO<HexMapTransformData> transformData;

        [NativeDisableUnsafePtrRestriction]
        // This data defines the hexagon hollow data
        public readonly RefRO<HexHollowData> hollowData;

        [NativeDisableUnsafePtrRestriction]
        // This data defines the hexagon activation grid
        public readonly RefRO<HexagonActivationGrid> activationGrid;
    }
}