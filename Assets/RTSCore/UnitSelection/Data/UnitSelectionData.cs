using Unity.Entities;
using Unity.Mathematics;

namespace GalacticBoundStudios.RTSCore
{
    // Marks an object as selectable
    public struct SelectableTag : IComponentData
    {

    }

    // Marks an object as selected
    public struct SelectedTag : IComponentData
    {

    }

    // Used to specify a prefab to use as a selected marker and the relative position that marker should be placed
    public struct SelectedMarkerPrefabData : IComponentData
    {
        // The key for the prefab string
        public Entity prefabEntity;
        // The offset at which to place the prefab
        public float3 markerOffset;
    }

    // Used to link a selected marker to a unit
    public struct SelectedMarkerEntityData : IComponentData
    {
        public Entity markerEntity;
    }

    public struct DragSelectionState : IComponentData
    {
        public bool isDragging;
        public float2 dragStart;
        public float2 dragEnd;
    }
}