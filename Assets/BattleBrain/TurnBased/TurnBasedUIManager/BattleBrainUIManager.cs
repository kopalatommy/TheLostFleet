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

        // Internal data to prevent unnecessary updates
        int turnCount = 0;
        int numActionsTaken = 0;
        int numActionsAvail = 0;

        public void UpdateTurnCounter(int turnCount)
        {
            if (this.turnCount == turnCount)
            {
                return;
            }
            this.turnCount = turnCount;

            turnCounterText.SetText(turnCount.ToString());
        }

        public void UpdateTurnProgress(int numActionsTaken, int numActionsAvail)
        {
            if (this.numActionsTaken == numActionsTaken && this.numActionsAvail == numActionsAvail)
            {
                return;
            }
            this.numActionsAvail = numActionsAvail;
            this.numActionsTaken = numActionsTaken;

            turnProgressText.SetText(numActionsTaken + " / " + numActionsAvail);
        }
    }
}