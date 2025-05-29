using UnityEngine;
using TMPro;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public class BattleBrainUIManager : MonoBehaviour
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