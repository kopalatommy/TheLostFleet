using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.RTSCore
{
    public class DragSelectionOverlay : MonoBehaviour
    {
        private EntityQuery query;

        private static Texture2D _white;
        private static Texture2D WhiteTex => _white ??= Texture2D.whiteTexture;

        private void Awake()
        {
            query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(DragSelectionState));
        }

        private void OnGUI()
        {
            if (query.IsEmpty)
            {
                return;
            }

            var state = query.GetSingleton<DragSelectionState>();
            if (!state.isDragging)
            {
                return;
            }

            // Convert to GUI coordinates (top‑left origin)
            float y0 = Screen.height - state.dragStart.y;
            float y1 = Screen.height - state.dragEnd.y;
            float x0 = state.dragStart.x;
            float x1 = state.dragEnd.x;

            Rect guiRect = Rect.MinMaxRect(math.min(x0, x1), math.min(y0, y1), math.max(x0, x1), math.max(y0, y1));

            DrawFilled(guiRect, new Color(0.3f, 0.6f, 1f, 0.25f));
            DrawBorder(guiRect, 2f, new Color(0.3f, 0.6f, 1f, 1f));
        }

        private static void DrawFilled(Rect r, Color c)
        {
            var prev = GUI.color;
            GUI.color = c; GUI.DrawTexture(r, WhiteTex); GUI.color = prev;
        }

        private static void DrawBorder(Rect r, float t, Color c)
        {
            DrawFilled(new Rect(r.xMin, r.yMin, r.width, t), c);                 // Top
            DrawFilled(new Rect(r.xMin, r.yMax - t, r.width, t), c);             // Bottom
            DrawFilled(new Rect(r.xMin, r.yMin, t, r.height), c);                // Left
            DrawFilled(new Rect(r.xMax - t, r.yMin, t, r.height), c);            // Right
        }
    }
}