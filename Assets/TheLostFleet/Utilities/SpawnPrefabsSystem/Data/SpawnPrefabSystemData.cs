using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GalacticBoundStudios.SpawnPrefabsSystem
{
    public struct DeclarePrefabData : IComponentData
    {
        public int keyHash;
        public Entity prefab;
    }

    public struct SpawnPrefabRequest : IComponentData
    {
        public int keyHash;
        public float3 position;
        public quaternion rotation;
    }

    public struct ReturnPooledObjectRequest : IComponentData
    {
        
    }

    public struct ObjectPool : IComponentData
    {
        public NativeArray<Entity> pooledObjects;
        public int pooledCount;
    }
}