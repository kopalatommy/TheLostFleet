using GalacticBoundStudios.HexTech;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DrawHexCoords : MonoBehaviour
{
    [SerializeField]
    private GameObject hexCoordPrefab;

    [SerializeField]
    private Transform hexParent;

    private void Awake()
    {
        DrawCoords();
    }

    void DrawCoords()
    {
        List<HexCoord> hexCoords = new List<HexCoord>();

        for (int q = -5; q <= 5; q++)
        {
            for (int r = -5; r <= 5; r++)
            {
                if (q + r >= -5 && q + r <= 5)
                {
                    hexCoords.Add(new HexCoord { q = q, r = r });
                }
            }
        }

        HexMapTransformData transformData = new HexMapTransformData()
        {
            origin = new float3(0, 0, 0),
            orientation = HexOrientation.PointyTop(),
            scale = new float2(1, 1)
        };

        foreach (HexCoord key in hexCoords)
        {
            float2 worldPos = HexMath.HexToPixel(key, in transformData);

            GameObject locMarker = GameObject.Instantiate(hexCoordPrefab, new Vector3(worldPos.x, 0.1f, worldPos.y), Quaternion.identity, hexParent);

            // Get the text mesh pro component from the prefab to write the hex coords
            TMP_Text text = locMarker.GetComponentInChildren<TMP_Text>();

            text.SetText(key.q + ", " + key.r + ", " + (-key.q - key.r));
        }
    }
}
