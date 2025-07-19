using GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap;
using GalacticBoundStudios.HexTech;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public partial class GalaxyMapUISystem : SystemBase
    {
        private GalaxyMapUIManager galaxyMapUIManager;
        private GalaxyMapSelectedTileUIManager galaxyMapSelectedTileUIManager;

        private EntityQuery selectedTileQuery;

        protected override void OnCreate()
        {
            RequireForUpdate<EnableGalaxyMapFlag>();

            selectedTileQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GalaxyMapSelectedTileFlag>().Build(EntityManager);
        }

        protected override void OnStartRunning()
        {
            InstantiateUIManager();
            InstantiateSelectedTileUI();
        }

        protected void InstantiateUIManager()
        {
            GameObject uiPrefab = Resources.Load("GalaxyMapUICanvas") as GameObject;

            if (uiPrefab != null)
            {
                galaxyMapUIManager = GameObject.Instantiate(uiPrefab).GetComponent<GalaxyMapUIManager>();
            }
            else
            {
                Debug.Log("Failed to load UI Prefab");
            }
        }

        private void InstantiateSelectedTileUI()
        {
            GameObject uiPrefab = Resources.Load("SelectedTileUIPrefab") as GameObject;

            if (uiPrefab != null)
            {
                galaxyMapSelectedTileUIManager = GameObject.Instantiate(uiPrefab).GetComponent<GalaxyMapSelectedTileUIManager>();
                galaxyMapSelectedTileUIManager.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Failed to load UI Prefab");
            }
        }

        protected override void OnStopRunning()
        {

        }

        protected override void OnUpdate()
        {
            if (selectedTileQuery.CalculateEntityCount() > 0)
            {
                if (!galaxyMapSelectedTileUIManager.gameObject.activeSelf)
                {
                    galaxyMapSelectedTileUIManager.gameObject.SetActive(true);
                }

                Entity e = selectedTileQuery.GetSingletonEntity();

                HexCoord coord = EntityManager.GetComponentData<HexCoord>(e);

                float2 worldPos = HexMath.HexToPixel(coord, HexMapTransformData.Default);

                galaxyMapSelectedTileUIManager.transform.position = new Vector3(worldPos.x, 2.5f, worldPos.y);

                galaxyMapSelectedTileUIManager.UpdateInformation(coord);
            }
            else
            {
                galaxyMapSelectedTileUIManager.gameObject.SetActive(false);
            }
        }
    }
}