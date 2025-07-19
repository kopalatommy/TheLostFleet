using Unity.Entities;
using Unity.Collections;
using GalacticBoundStudios.HexTech;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public struct GalaxyMapPathRequest : IComponentData
    {
        public HexCoord start;
        public HexCoord target;
    }

    public struct GalaxyMapPathFindingHighlightTileFlag : IComponentData
    {

    }

    public struct GalaxyMapPathInProgressTag : IComponentData
    {

    }

    public struct GalaxyMapPath : IBufferElementData
    {
        public HexCoord Value;

        public static implicit operator HexCoord(GalaxyMapPath e)
        {
            return e.Value;
        }
        public static implicit operator GalaxyMapPath(HexCoord e)
        {
            return new GalaxyMapPath { Value = e };
        }
    }
}