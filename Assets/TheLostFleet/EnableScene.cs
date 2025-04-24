using Unity.Scenes;
using UnityEngine;

public class EnableScene : MonoBehaviour
{
    private void Start()
    {
        SubScene subScene = GetComponentInChildren<SubScene>();
        subScene.enabled = true;
    }
}
