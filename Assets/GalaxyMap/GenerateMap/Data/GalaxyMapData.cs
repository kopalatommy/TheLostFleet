using Unity.Entities;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    // This is used to prevent updates in the system. It is intended to create the 
    // galaxy map on start up and not do anything else
    public struct DidCreateGalaxyMapTag : IComponentData
    {

    }
}