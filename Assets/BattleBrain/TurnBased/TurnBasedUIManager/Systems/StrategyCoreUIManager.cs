using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.BattleBrain.TurnBased
{
    public partial class BattleBrainUIManagerSystem : SystemBase
    {
        private BattleBrainUIManager uiObj;
        private Entity stateData;

        protected override void OnCreate()
        {
            RequireForUpdate<TurnBasedEnableFlag>();
        }

        protected override void OnStartRunning()
        {
            InstantiateUI();

            stateData = EntityManager.CreateEntity();
            EntityManager.AddComponentData(stateData, new BattleBrainTurnBaseUIData
            {
                numActionsTaken = 0,
                totalActionsCount = 1,
                turnCounter = 0
            });
        }

        protected void InstantiateUI()
        {
            GameObject uiPrefab = Resources.Load("StrategyCoreUIPrefab") as GameObject;

            if (uiPrefab != null)
            {
                uiObj = GameObject.Instantiate(uiPrefab).GetComponent<BattleBrainUIManager>();
            }
            else
            {
                Debug.Log("Failed to load UI Prefab");
            }
        }

        protected override void OnStopRunning()
        {
            if (uiObj)
            {
                GameObject.Destroy(uiObj);
                uiObj = null;
            }
            EntityManager.DestroyEntity(stateData);
        }

        protected override void OnDestroy()
        {
            
        }

        protected override void OnUpdate()
        {
            // ToDo:
            // 1. Display turn number
            // 2. Display number of pending actions
            BattleBrainTurnBaseUIData data = EntityManager.GetComponentData<BattleBrainTurnBaseUIData>(stateData);

            data.turnCounter = (int)(SystemAPI.Time.ElapsedTime / 5);


            uiObj.UpdateTurnCounter(data.turnCounter);
            uiObj.UpdateTurnProgress(data.numActionsTaken, data.totalActionsCount);
        }
    }
}