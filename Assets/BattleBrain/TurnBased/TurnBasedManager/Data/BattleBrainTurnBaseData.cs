using Unity.Entities;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public struct TurnBasedEnableFlag : IComponentData
    {

    }

    public struct TurnBasedState : IComponentData
    {
        public int turnCount;
    }

    public struct TurnBasedTurnActor : IComponentData
    {
        public int turnOrderPriority;
    }

    public struct PlayerControlledTag : IComponentData
    {

    }

    public struct WantsToEndTurn : IComponentData
    {

    }
      
}