using GalacticBoundStudios.BattleBrain;
using GalacticBoundStudios.GalaxyMap.Units;
using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.PathFinding;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.GalaxyMap.Units
{
    public class GalaxyMapUnitAuthoring : MonoBehaviour
    {
        public HexCoord gridPosition;

        public float moveSpeed = 1;

        public GameObject selectedMarker = null;

        public class Baker : Baker<GalaxyMapUnitAuthoring>
        {
            public override void Bake(GalaxyMapUnitAuthoring authoring)
            {
                Debug.Log("GalaxyMapUnitAuthoring.Baker.Bake");

                Entity e = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(e, new GalaxyMapUnitFlag());

                AddComponent(e, new HexTechMapEntityTag()
                {
                    gridPosition = authoring.gridPosition
                });

                AddComponent(e, new HexTechMovementSettings()
                {
                    moveSpeed = authoring.moveSpeed
                });

                if (authoring.selectedMarker != null)
                {
                    AddComponent(e, new SelectedMarkerPrefabData()
                    {
                        prefabEntity = GetEntity(authoring.selectedMarker, TransformUsageFlags.Renderable),
                        markerOffset = new float3(0, 1, 0)
                    });
                }

                AddComponent(e, new SelectableTag());
            }
        }
    }
}