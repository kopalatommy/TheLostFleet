using System;
using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Collections;
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

        public Action<HexCoord> setPathStartPos;
        public Action<HexCoord> setPathEndPos;
        public Action startPathFinder;

        #endregion // Events

        #region Map Data

        public NativeHashMap<HexCoord, float> mapCostData;

        #endregion // Map Data

        protected HexCoord selectedHexagon = new HexCoord(0, 0);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Debug.Log("HexMapManager.Awake");

            Instance = this;
            DontDestroyOnLoad(gameObject);

            mapCostData = new NativeHashMap<HexCoord, float>(128, Allocator.Persistent);

            SetUpEventListeners();
        }

        private void Destroy()
        {
            mapCostData.Dispose();
        }

        private void SetUpEventListeners()
        {
            onSelectHexagon += onSelectHexagonAction;
            onCreateHexagon += OnCreateNewCoord;
        }

        private void onSelectHexagonAction(HexCoord coord)
        {
            selectedHexagon = coord;
        }

        private void OnCreateNewCoord(HexCoord coord)
        {
            mapCostData.Add(coord, 0);
        }
    }
}