using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public partial class BattleBrainUIManagerSystem : SystemBase
    {
        GameObject uiObj;

        protected override void OnCreate()
        {
            RequireForUpdate<TurnBasedEnableFlag>();

            InstantiateUI();
        }

        protected void InstantiateUI()
        {
            GameObject uiPrefab = Resources.Load("StrategyCoreUIPrefab") as GameObject;

            if (uiPrefab != null)
            {
                uiObj = GameObject.Instantiate(uiPrefab);
            }
            else
            {
                Debug.Log("Failed to load UI Prefab");
            }
        }

        protected override void OnDestroy()
        {
            if (uiObj)
            {
                GameObject.Destroy(uiObj);
                uiObj = null;
            }
        }

        protected override void OnUpdate()
        {
            // ToDo:
            // 1. Display turn number
            // 2. Display number of pending actions
        }
    }
}