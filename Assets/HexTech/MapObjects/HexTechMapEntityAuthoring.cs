using GalacticBoundStudios.HexTech.PathFinding;
using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.HexTech.MapEntities
{
    public class HexTechMapEntityAuthoring : MonoBehaviour
    {
        public HexCoord gridPosition;

        public float moveSpeed = 1;

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
            }
        }
    }
}
