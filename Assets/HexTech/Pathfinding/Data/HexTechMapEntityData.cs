

using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Entities;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    // Apply this component to an entity to signify that it is a HexTech entity
    public struct HexTechMapEntityTag : IComponentData
    {
        public HexCoord gridPosition;
    }

    // This struct defines the movement settings for a HexTech entity
    public struct HexTechMovementSettings : IComponentData
    {
        // The speed at which the entity moves. Tiles per second
        public float moveSpeed;
    }
}