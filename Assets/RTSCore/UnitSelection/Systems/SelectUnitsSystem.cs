using GalacticBoundStudios.SpawnPrefabsSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GalacticBoundStudios.RTSCore
{
    public partial class UnitSelectionSystem : SystemBase
    {
        private Camera mainCamera;

        private EntityQuery selectedQuery;
        private EntityQuery notSelectedQuery;

        protected override void OnCreate()
        {
            mainCamera = Camera.main;

            Entity e = EntityManager.CreateEntity(typeof(DragSelectionState));
            EntityManager.SetName(e, "DragSelectionState");

            // Query for selected entities
            selectedQuery = GetEntityQuery(
                ComponentType.ReadOnly<SelectableTag>(),
                ComponentType.ReadOnly<SelectedTag>()
            );

            // Query for selectable but not selected entities
            notSelectedQuery = GetEntityQuery(
                ComponentType.ReadOnly<SelectableTag>(),
                ComponentType.Exclude<SelectedTag>()
            );

        }

        protected override void OnDestroy()
        {
            mainCamera = null;
        }

        protected override void OnStopRunning()
        {
            mainCamera = null;
        }

        protected override void OnUpdate()
        {
            Mouse mouse = Mouse.current;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            if (mouse == null || mainCamera == null) 
            {
                Debug.Log("UnitSelectionSystem.OnUpdate: null obj. Mouse: " + (mouse == null) + " Camera: " + (mainCamera == null));
                return;
            }

            RefRW<DragSelectionState> state = SystemAPI.GetSingletonRW<DragSelectionState>();
            ref DragSelectionState stateRef = ref state.ValueRW;

            if (mouse.middleButton.wasPressedThisFrame)
            {
                stateRef.isDragging = true;
                stateRef.dragStart = mouse.position.ReadValue();
                stateRef.dragEnd = stateRef.dragStart;
            }
            else if (stateRef.isDragging)
            {
                stateRef.dragEnd = mouse.position.ReadValue();

                SelectEntitiesRealtime(GetCurrentRect(stateRef), mainCamera);

                if (mouse.middleButton.wasReleasedThisFrame)
                {
                    stateRef.isDragging = false;
                    // SelectEntitiesInside(GetCurrentRect(stateRef), mainCamera);

                    EntityQuery getSelectedQuery = GetEntityQuery(typeof(SelectedTag));
                    Debug.Log("Selected " + getSelectedQuery.CalculateEntityCount() + " units");
                }
            }
        }

        private void SelectEntitiesRealtime(Rect rect, Camera camera)
        {
            // This is used to modify the entities
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            float2 xBounds = new float2(rect.xMin, rect.xMax);
            float2 yBounds = new float2(rect.yMin, rect.yMax);
            // Foreach entity that is already selected, check if still selected
            Entities.WithAll<SelectedTag, SelectableTag, LocalTransform>().ForEach((Entity entity, in SelectableTag selectable, in SelectedTag selected, in LocalTransform localTransform) => {
                Vector3 sp = camera.WorldToScreenPoint(localTransform.Position);

                // If not in bounds, remove data
                if ((sp.x < xBounds.x || sp.x > xBounds.y) || (sp.y < yBounds.x || sp.y > yBounds.y))
                {
                    ecb.RemoveComponent<SelectedTag>(entity);

                    if (EntityManager.HasComponent<SelectedMarkerEntityData>(entity))
                    {
                        SelectedMarkerEntityData markerData = EntityManager.GetComponentData<SelectedMarkerEntityData>(entity);

                        ecb.RemoveComponent<SelectedMarkerEntityData>(entity);
                        ecb.DestroyEntity(markerData.markerEntity);
                    }
                }
            }).WithoutBurst().Run();

            Entities.WithAll<SelectableTag, LocalTransform>().WithAbsent<SelectedTag>().ForEach((Entity entity, in SelectableTag selectable, in LocalTransform localTransform) => {
                Vector3 sp = camera.WorldToScreenPoint(localTransform.Position);

                // If in selected area, add the selected tag
                if ((sp.x >= xBounds.x && sp.x <= xBounds.y) && (sp.y >= yBounds.x && sp.y <= yBounds.y))
                {
                    ecb.AddComponent<SelectedTag>(entity);

                    // If the entity has a selection marker, instantiate it
                    if (EntityManager.HasComponent<SelectedMarkerPrefabData>(entity))
                    {
                        SelectedMarkerPrefabData prefabData = EntityManager.GetComponentData<SelectedMarkerPrefabData>(entity);

                        Entity prefab = ecb.Instantiate(prefabData.prefabEntity);
                        ecb.SetComponent(prefab, new LocalTransform()
                        {
                            Position = localTransform.Position + prefabData.markerOffset,
                            Rotation = quaternion.identity,
                            Scale = 1
                        });
                        ecb.AddComponent(entity, new SelectedMarkerEntityData
                        {
                            markerEntity = prefab
                        });
                    }
                }
            }).WithoutBurst().Run();


            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void SelectEntitiesInside(Rect rect, Camera camera)
        {
            // This is used to modify the entities
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Clear the previous selection
            Entities.WithAll<SelectedTag>().ForEach((Entity e) => {
                ecb.RemoveComponent<SelectedTag>(e);

                if (EntityManager.HasComponent<SelectedMarkerEntityData>(e))
                {
                    SelectedMarkerEntityData markerData = EntityManager.GetComponentData<SelectedMarkerEntityData>(e);
                    
                    ecb.RemoveComponent<SelectedMarkerEntityData>(e);
                    ecb.DestroyEntity(markerData.markerEntity);
                }
            }).WithoutBurst().Run();

            // Select the entities in the bounds
            // float4 r = new float4(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
            float2 xBounds = new float2(rect.xMin, rect.xMax);
            float2 yBounds = new float2(rect.yMin, rect.yMax);
            Entities.WithAll<SelectableTag>().ForEach((Entity e, in LocalToWorld ltw) =>
            {
                float3 world = ltw.Position;
                Vector3 sp = camera.WorldToScreenPoint(world);
                if ((sp.x >= xBounds.x && sp.x <= xBounds.y) && (sp.y >= yBounds.x && sp.y <= yBounds.y))
                {
                    ecb.AddComponent<SelectedTag>(e);

                    if (EntityManager.HasComponent<SelectedMarkerPrefabData>(e))
                    {
                        SelectedMarkerPrefabData prefabData = EntityManager.GetComponentData<SelectedMarkerPrefabData>(e);

                        Entity prefab = ecb.Instantiate(prefabData.prefabEntity);
                        ecb.SetComponent(prefab, new LocalTransform()
                        {
                            Position = world + prefabData.markerOffset,
                            Rotation = quaternion.identity,
                            Scale = 1
                        });
                        ecb.AddComponent(e, new SelectedMarkerEntityData
                        {
                            markerEntity = prefab
                        });
                    }
                }
            }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private static Rect GetCurrentRect(in DragSelectionState state)
        {
            float2 min = math.min(state.dragStart, state.dragEnd);
            float2 max = math.max(state.dragStart, state.dragEnd);

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void InstallOverlay()
        {
            // Create a hidden GameObject to drive OnGUI; easier than a custom Render System.
            var go = new GameObject("DragSelectionOverlay") { hideFlags = HideFlags.HideAndDontSave };
            go.AddComponent<DragSelectionOverlay>();
        }
#endif
    }
}