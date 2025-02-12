using System;
using UnityEngine;

namespace GalacticBoundStudios.HexTech
{
    // This class holds the global data for the HexMap
    // 1. The HexMapConfig holds the configuration data for the HexMap
    public class HexMapManager : MonoBehaviour
    {
        public static HexMapManager Instance { get; private set; }

        [SerializeField]
        protected HexMapConfig config;

        public HexMapConfig Config => config;

        #region Events

        // This action is triggered when a new hexagon is created. It is primarily used to create UI
        // elements that are associated with the hexagon.
        public Action<HexCoord> onCreateHexagon;

        // This action is triggered when a hexagon is selected
        public Action<HexCoord> onSelectHexagon;

        #endregion // Events

        protected HexCoord selectedHexagon = new HexCoord(0, 0);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                SetUpEventListeners();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void SetUpEventListeners()
        {
            onSelectHexagon += onSelectHexagonAction;
        }

        private void onSelectHexagonAction(HexCoord coord)
        {
            selectedHexagon = coord;
        }
    }
}