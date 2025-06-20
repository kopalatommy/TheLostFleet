using Unity.Entities;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public struct BattleBrainTurnBaseUIData : IComponentData
    {
        public int turnCounter;
        public int numActionsTaken;
        public int totalActionsCount;
    }
}