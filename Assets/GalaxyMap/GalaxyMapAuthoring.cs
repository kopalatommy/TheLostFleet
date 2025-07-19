using System.Collections.Generic;
using GalacticBoundStudios.GalaxyMap.Units;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public class GalaxyMapAuthoring : MonoBehaviour
    {
        public List<GameObject> unitPrefabs;

        public class Baker : Baker<GalaxyMapAuthoring>
        {
            public override void Bake(GalaxyMapAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new EnableGalaxyMapFlag());

                DynamicBuffer<GalaxyMapUnitPrefabsData> unitPrefabsArray = AddBuffer<GalaxyMapUnitPrefabsData>(e);

                foreach (GameObject unitPrefab in authoring.unitPrefabs)
                {
                    unitPrefabsArray.Add(new GalaxyMapUnitPrefabsData
                    {
                        Value = GetEntity(unitPrefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}