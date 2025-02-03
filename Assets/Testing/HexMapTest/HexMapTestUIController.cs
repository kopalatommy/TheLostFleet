using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Entities;
using GalacticBoundStudios.HexTech;
using Unity.Mathematics;

namespace GalacticBoundStudios.Testing
{
    public class HexMapTestUIController : MonoBehaviour
    {
        // This text field wil display the current hexagon coordinates of the cursor
        [SerializeField]
        protected TextMeshProUGUI cursorHexCoordsText;

        void Awake()
        {
            cursorHexCoordsText.text = "Cursor Hex Coords: (0, 0)";
        }

        void Update()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity singleton = entityManager.CreateEntityQuery(typeof(HexMapTransformData)).GetSingletonEntity();

            // Get the hexagon map transform data
            HexMapTransformData hexMapTransformData = entityManager.GetComponentData<HexMapTransformData>(singleton);

            // Get the cursor position in world space
            float3 cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Get the hexagon coordinates of the cursor
            HexCoord cursorHexCoords = HexMath.PixelToHex(new float2(cursorWorldPos.x, cursorWorldPos.z), hexMapTransformData);
            
            // Update the text field with the current hexagon coordinates
            cursorHexCoordsText.text = "Cursor Hex Coords: (" + cursorHexCoords.q + ", " + cursorHexCoords.r + ")";
        }
    }
}