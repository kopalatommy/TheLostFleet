using GalacticBoundStudios.HexTech;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public class GalaxyMapSelectedTileUIManager : MonoBehaviour
    {
        // UI Objects

        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private Button enterSystemButton;

        // Cache the current coord to limit unnecessary updates (extra redraws)
        private HexCoord currentCoord;

        public void UpdateInformation(HexCoord coord)
        {
            if (!currentCoord.Equals(coord))
            {
                currentCoord = coord;
                titleText.text = coord.ToString();
            }
        }
    }
}
