using System.Collections.Generic;
using UnityEngine;

namespace GalacticBoundStudios
{
    [System.Serializable]
    public struct SceneSpawnObject
    {
        public GameObject prefabToSpawn;
        public Vector3 spawnLocation;
        public Vector3 spawnRotation;
    }

    [CreateAssetMenu(fileName ="SceneConfig", menuName ="SceneSettings/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        public List<SceneSpawnObject> objectsToSpawn;
    }
}