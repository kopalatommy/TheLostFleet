using System.Collections;
using UnityEditor;
using UnityEngine;

namespace GalacticBoundStudios
{
    public class SceneSpawner : MonoBehaviour
    {
        public SceneConfig sceneConfigSettings;

        IEnumerator Start()
        {
            if (sceneConfigSettings == null)
            {
                yield return null;
            }

            foreach (SceneSpawnObject obj in sceneConfigSettings.objectsToSpawn)
            {
                Instantiate(obj.prefabToSpawn, obj.spawnLocation, Quaternion.Euler(obj.spawnRotation));
                // EditorApplication.isPaused = true;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Finished spawing scene");
        }
    }
}