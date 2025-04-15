using Unity.Collections;
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

    public struct HexTechCreatePathRequest : IComponentData
    {
        public HexCoord startPos;
        public HexCoord endPos;
        public Entity entity;
    }

    public struct HexTechPathData : IComponentData
    {
        public NativeList<HexCoord> path;
    }

    public readonly partial struct HexTechPathfindingEntity : IAspect
    {
        // The entity
        public readonly Entity entity;

        // The entity's position in the grid
        public readonly RefRO<HexTechEntityFlag> entityFlag;
        // The entity's movement settings
        public readonly RefRO<HexTechMovementSettings> movementSettings;
    }
}
