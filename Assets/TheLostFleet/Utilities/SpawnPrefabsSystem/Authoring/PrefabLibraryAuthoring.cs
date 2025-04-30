using GalacticBoundStudios.SpawnPrefabsSystem;
using Unity.Entities;
using UnityEngine;

namespace GalacticBoundStudios.SpawnPrefabsSystem
{
    public class PrefabLibraryAuthoring : MonoBehaviour
    {
        [System.Serializable]
        public struct Entry
        {
            public string key;
            public GameObject prefab;
            int poolSize;
        }

        public Entry prefab;

        public class Baker : Baker<PrefabLibraryAuthoring>
        {
            public override void Bake(PrefabLibraryAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new DeclarePrefabData
                {
                    keyHash = authoring.prefab.key.GetHashCode(),
                    prefab = GetEntity(authoring.prefab.prefab, TransformUsageFlags.Renderable)
                });
            }
        }
    }
}