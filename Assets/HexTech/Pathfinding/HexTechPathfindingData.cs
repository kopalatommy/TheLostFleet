using Unity.Entities;
using Unity.Mathematics;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    // Apply this component to an entity to signify that it is a HexTech entity
    public struct HexTechEntityFlag : IComponentData
    {
        // Track the entity's position in the grid
        public HexCoord gridPosition;
    }

    // This struct defines the movement settings for a HexTech entity
    public struct HexTechMovementSettings : IComponentData
    {
        // The speed at which the entity moves
        public float moveSpeed;
    }

    // This struct signifies that this hexagon is a walkable tile
    public struct HexTechWalkable : IComponentData
    {
        // The cost to move to this tile
        public int moveCost;
    }
}
