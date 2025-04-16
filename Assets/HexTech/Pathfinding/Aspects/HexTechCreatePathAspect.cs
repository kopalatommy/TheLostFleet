using Unity.Entities;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    // This aspect will cover an entity that is requesting a path
    public readonly partial struct HexTechCreatePathAspect : IAspect
    {
        // The entity that is requesting the path
        public readonly Entity entity;

        // The entity's position in the grid
        public readonly RefRO<HexTechMapEntityTag> entityFlag;
        // The entity's movement settings
        public readonly RefRO<HexTechMovementSettings> movementSettings;
    }
}