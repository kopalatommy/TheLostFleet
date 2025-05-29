using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public class BattleBrainTurnBasedAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BattleBrainTurnBasedAuthoring>
        {
            public override void Bake(BattleBrainTurnBasedAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new TurnBasedEnableFlag());
            }
        }
    }
}