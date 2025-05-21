using GalacticBoundStudios.HexTech;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.RTSCore;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    // Handles starting pathFinding
    // Handles selecting hexagons

    // The plan for this system is to be the glue that ties a bunch of
    // different systems together that do not know about each other. For
    // example, the select units system does not know about the path finding 
    // system, yet the 2 need to work together.
    public partial class SystemMapManagerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<SystemMapEnableFlag>();

            Entity mapCostData = EntityManager.CreateEntity(typeof(SystemMapMovementCostData));
            EntityManager.SetComponentData(mapCostData, new SystemMapMovementCostData
            {
                Value = new NativeHashMap<HexCoord, float>(100, Allocator.Persistent)
            });
        }

        protected void CreateGenerateMapRequest()
        {
            Debug.Log("SystemMapManagerSystem.CreateGenerateMapRequest");

            Entity e = EntityManager.CreateEntity();
            EntityManager.AddComponent<GenerateSystemMapRequestFlag>(e);
        }

        protected override void OnUpdate()
        {
            HighlightCurrentHexagon();
            HandleStartPathFinding();
        }

        protected void HighlightCurrentHexagon()
        {
            SystemMapFocusHexagonData focusedHex = SystemAPI.GetSingleton<SystemMapFocusHexagonData>();
            
            FixedString64Bytes propertyName = new FixedString64Bytes("_HighlightedAxialCoords");
            float4 shaderValue = new float4(focusedHex.coord.q, focusedHex.coord.r, 0, 0);
            
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetVector("_HighlightedAxialCoords", shaderValue);

            if (SystemAPI.TryGetSingletonEntity<HexTechMapEntityTag>(out Entity mapEntity))
            {
                //Debug.Log("Coord: " + shaderValue);

                // EntityManager.AddComponentData(mapEntity, new HighlightAxialCoords
                // {
                //     Value = new float4(0.0f, 1.0f, 0.0f, 1.0f)
                // });

                // Renderer renderer = EntityManager.GetComponentObject<Renderer>(mapEntity);

                // if (renderer == null)
                // {
                //     // This entity doesn't have a UnityEngine.Renderer component instance.
                //     // This might happen if it's a pure DOTS entity not yet fully processed by Hybrid Renderer,
                //     // or if the setup is incorrect.
                //     Debug.LogWarning($"Entity {mapEntity.ToString()} with HexGridTag does not have a UnityEngine.Renderer.");
                //     return; // Skip this entity
                // }

                // MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                // // Get all other property values to not change any other values
                // renderer.GetPropertyBlock(materialPropertyBlock, 0);
                // // Update the highlighted value
                // materialPropertyBlock.SetVector("_HighlightedAxialCoords", shaderValue);
                // // Send the updated values to the renderer
                // renderer.SetPropertyBlock(materialPropertyBlock, 0);
                

                
            }
        }

        protected void HandleStartPathFinding()
        {
            foreach ((SelectedTag selected, Entity e) in SystemAPI.Query<SelectedTag>().WithEntityAccess())
            {

            }
        }
    }
}
