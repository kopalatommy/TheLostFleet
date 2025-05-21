using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Collections;
using Unity.Entities;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    // Add this component to an entity to queue a path request
    public struct HexTechCreatePathRequest : IComponentData
    {
        public HexCoord startPos;
        public HexCoord endPos;
    }

    // This component is attached to a component when a path request is complete
    // The resulting path will include the current tile that the entity is on
    public struct HexTechMapPath : IBufferElementData
    {
        public HexCoord Value;

        public static implicit operator HexCoord(HexTechMapPath e)
        {
            return e.Value;
        }
        public static implicit operator HexTechMapPath(HexCoord e)
        {
            return new HexTechMapPath { Value = e };
        }
    }
}