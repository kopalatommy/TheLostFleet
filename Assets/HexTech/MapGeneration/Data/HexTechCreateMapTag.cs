using Unity.Entities;

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    // This is an empty struct that is used to signal that a map should be generated 
    // with the data present on the entity
    public struct HexTechCreateMapTag : IComponentData
    {

    }
}