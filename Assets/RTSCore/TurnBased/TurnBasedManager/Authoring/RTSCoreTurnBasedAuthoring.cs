using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.StrategyCore.TurnBased
{
    public class RTSCoreTurnBasedAuthoring : MonoBehaviour
    {
        public class Baker : Baker<RTSCoreTurnBasedAuthoring>
        {
            public override void Bake(RTSCoreTurnBasedAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new StrategyCoreTurnBasedEnableFlag()
                {
                    turnCounter = 0
                });
            }
        }
    }
}