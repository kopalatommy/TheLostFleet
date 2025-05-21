using Unity.Entities;

namespace GalacticBoundStudios.StrategyCore.TurnBased
{
    // Add this component to an entity to enable the turn based manager
    public struct StrategyCoreTurnBasedEnableFlag : IComponentData
    {
        // Tracks the number of turns that have taken place
        public int turnCounter;
    }

    // Attach this flag to an entity to signal that is has actions that take
    // place every turn
    public struct StrategyCoreTurnBasedHasActionFlag : IComponentData
    {

    }

    // Attach this to an entity when its turn based action has been completed
    public struct StrategyCoreTurnBasedHasTakenAction : IComponentData
    {

    }

    // Attach this component to an entity to ignore the action this turn
    public struct StrategyCoreTurnBasedIgnoreActionFlag : IComponentData
    {
        
    }
}