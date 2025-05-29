using GalacticBoundStudios.HexTech.PathFinding;
using GalacticBoundStudios.BattleBrain;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace GalacticBoundStudios.HexTech.MapEntities
{
    public class HexTechMapEntityAuthoring : MonoBehaviour
    {
        public HexCoord gridPosition;

        public float moveSpeed = 1;

        public GameObject selectedMarker = null;

        public class Baker : Baker<HexTechMapEntityAuthoring>
        {
            public override void Bake(HexTechMapEntityAuthoring authoring)
            {
                Debug.Log("HexTechMapEntityAuthoring.Baker.Bake");

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new HexTechMapEntityTag()
                {
                    gridPosition = authoring.gridPosition
                });

                AddComponent(entity, new HexTechMovementSettings()
                {
                    moveSpeed = authoring.moveSpeed
                });

                if (authoring.selectedMarker != null)
                {
                    AddComponent(entity, new SelectedMarkerPrefabData()
                    {
                        prefabEntity = GetEntity(authoring.selectedMarker, TransformUsageFlags.Renderable),
                        markerOffset = new float3(0, 1, 0)
                    });
                }

                AddComponent(entity, new SelectableTag());
            }
        }
    }
}
