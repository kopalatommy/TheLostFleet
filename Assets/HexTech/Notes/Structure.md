# Hex Tech Structure

## Components

### Generation

#### HexagonMeshGenerator

    This is a ISystem that generates the mesh for the hexagon grid. It handles converting a map of hexagons into mesh data.

### Managers

#### HexMapManager

    This is a MonoBehaviour that acts as an intermediate between the DOTS and MonoBehaviour systems. It listens for various events that can be forwarded to the DOTS systems.

    1. OnCreateHexagon(HexCoord coord)
    2. OnSelectHexagon(HexCoord coord)

### Shaders

#### ColorableMaterial

    This is a material that uses the color array provided to meshes.

### UI



### Utilities