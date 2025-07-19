using Unity.Entities;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    // This is used to prevent updates in the system. It is intended to create the 
    // galaxy map on start up and not do anything else
    public struct DidCreateGalaxyMapTag : IComponentData
    {

    }

    // This is used to pass unit prefabs to the generate map system
    public struct GalaxyMapUnitPrefabsData : IBufferElementData
    {
        // The prefab entity, Called Value because this seems to be the Unity Standard
        public Entity Value;
    }
}