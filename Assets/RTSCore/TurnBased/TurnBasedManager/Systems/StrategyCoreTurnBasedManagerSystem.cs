using Unity.Collections;
using Unity.Entities;

namespace GalacticBoundStudios.StrategyCore.TurnBased
{
    public partial struct StrategyCoreTurnBasedManagerSystem : ISystem
    {
        EntityQuery queryPendingActions;
        EntityQuery queryAllActions;

        private void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StrategyCoreTurnBasedEnableFlag>();

            queryAllActions = new EntityQueryBuilder(Allocator.Persistent).WithAll<StrategyCoreTurnBasedHasActionFlag>().Build(ref state);
            queryAllActions = new EntityQueryBuilder(Allocator.Persistent).WithAll<StrategyCoreTurnBasedHasActionFlag>().WithAbsent<StrategyCoreTurnBasedIgnoreActionFlag>().Build(ref state);
        }

        private void OnDestroy(ref SystemState state)
        {

        }

        private void OnUpdate(ref SystemState state)
        {
            
        }
    }
}