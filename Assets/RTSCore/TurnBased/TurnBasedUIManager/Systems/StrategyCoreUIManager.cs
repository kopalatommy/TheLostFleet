using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.StrategyCore.TurnBased
{
    public partial class StrategyCoreUIManagerSystem : SystemBase
    {
        GameObject uiObj;

        protected override void OnCreate()
        {
            RequireForUpdate<StrategyCoreTurnBasedEnableFlag>();

            InstantiateUI();
        }

        protected void InstantiateUI()
        {
            GameObject uiPrefab = Resources.Load("StrategyCoreUIPrefab") as GameObject;

            if (uiPrefab != null)
            {
                GameObject.Instantiate(uiPrefab);
            }
            else
            {
                Debug.Log("Failed to load UI Prefab");
            }
        }

        protected override void OnDestroy()
        {

        }

        protected override void OnUpdate()
        {
            // ToDo:
            // 1. Display turn number
            // 2. Display number of pending actions
        }
    }
}