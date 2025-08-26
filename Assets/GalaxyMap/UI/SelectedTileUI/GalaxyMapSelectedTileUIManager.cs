using GalacticBoundStudios.HexTech;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Scenes;
using Unity.Entities;
using UnityEngine.SceneManagement;

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

        public void OnClick_Enter()
        {
            Debug.Log("Enter system: " + currentCoord);

            // SceneSystem sceneSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SceneSystem>();

            // sceneSystem.Get

            SceneManager.LoadSceneAsync("SectorScene");
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            // int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            // var t = SceneManager.UnloadSceneAsync(sceneIndex);

            // SceneManager.LoadSceneAsync(sceneIndex);
        }
    }
}
