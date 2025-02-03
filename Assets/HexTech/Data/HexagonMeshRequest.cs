using Unity.Entities;

using GalacticBoundStudios.MeshMania;

namespace GalacticBoundStudios.HexTech.MeshGeneration
{
    public readonly partial struct HexagonMeshRequestAspect : IAspect
    {
        // Reference the entity that holds the hexagon map data
        public readonly Entity entity;

        // This data defines the rotation of the hexagons
        public readonly RefRO<HexMapTransformData> mapTransformData;

        // This data defines if the hexagons are hollow
        public readonly RefRO<HexHollowData> hollowData;

        // The entity will have a CreateMapTag to signal that the mesh should be created
        public readonly RefRO<CreateMapTag> creationTag;

        // Container for the resulting mesh data
        public readonly RefRW<MeshData> meshData;

        // Defines which hexagons are to be generated
        public readonly RefRO<HexagonActivationGrid> gridCreationData;
    }
}