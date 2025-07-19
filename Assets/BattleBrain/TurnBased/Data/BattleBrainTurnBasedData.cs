using Unity.Entities;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    // Flags that an object has an action for the current turn
    public struct BattleBrainTurnBasedActionData : IComponentData
    {
        public int numTurnActions;
        public int numActionsTaken;
    }
}