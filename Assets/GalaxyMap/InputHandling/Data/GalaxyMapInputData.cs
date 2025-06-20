using Unity.Entities;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    // Flags that the tile has been selected with a click
    public struct GalaxyMapSelectedTileFlag : IComponentData { }
    // Flags that the tile has the cursor focus
    public struct GalaxyMapFocusTileFlag : IComponentData { }
}