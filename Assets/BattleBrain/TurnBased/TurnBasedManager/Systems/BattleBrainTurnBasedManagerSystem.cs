using Unity.Collections;
using Unity.Entities;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public partial struct BattleBrainTurnBasedManagerSystem : ISystem
    {
        EntityQuery queryPendingActions;
        EntityQuery queryAllActions;

        private void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurnBasedEnableFlag>();

            // queryAllActions = new EntityQueryBuilder(Allocator.Persistent).WithAll<BattleBrainTurnBasedHasActionFlag>().Build(ref state);
            // queryAllActions = new EntityQueryBuilder(Allocator.Persistent).WithAll<BattleBrainTurnBasedHasActionFlag>().WithAbsent<BattleBrainTurnBasedIgnoreActionFlag>().Build(ref state);
        }

        private void OnDestroy(ref SystemState state)
        {

        }

        private void OnUpdate(ref SystemState state)
        {
            
        }
    }
}