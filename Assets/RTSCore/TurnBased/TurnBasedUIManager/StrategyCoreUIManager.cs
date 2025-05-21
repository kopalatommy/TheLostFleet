using UnityEngine;
using TMPro;

namespace GalacticBoundStudios.StrategyCore.TurnBased
{
    public class StrategyCoreUIManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI turnCounterText;
        [SerializeField]
        private TextMeshProUGUI turnProgressText;

        public void UpdateTurnCounter(int turnCount)
        {

        }

        public void UpdateTurnProgress(int numActionsTaken, int numActionsAvail)
        {

        }
    }
}