#if UNITY_STANDALONE
using UnityEngine;

namespace ProjectWorlds
{
    public class ScreenShot
    {
        public static void TakeScreenShot(string path, int width, int height)
        {
            ScreenCapture.CaptureScreenshot(path, 1);
        }

        public static Texture2D Capture(Camera cam, Vector2Int resolution)
        {
            RenderTexture shot = new RenderTexture(resolution.x, resolution.y, 24);
            cam.targetTexture = shot;
            Texture2D screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = shot;
            screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
            RenderTexture.active = cam.targetTexture = null;
            MonoBehaviour.Destroy(shot);

            return screenShot;
        }
    }
}

#endif // UNITY_STANDALONE