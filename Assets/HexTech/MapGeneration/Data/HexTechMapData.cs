using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.DataScribes;

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    public struct HexHollowData : IComponentData
    {
        public bool isHollow;
        // Percentage (0 - 1)
        public float innerRadius;
    }

    [CreateAssetMenu(menuName = "HexTech/HexMapConfig")]
    public class HexMapConfig : ScriptableObject
    {
        public bool pointyTopHexagons = true;
        public Vector2 mapScale = Vector2.one;
        public Vector3 mapOffset = Vector3.zero;

        public bool hollow = false;
        public float innerRadius = 0.7f;

        public int chunkSize = 10;

        public HexTechGridShape gridShape = HexTechGridShape.Hexagon;

        public HexMapTransformData TransformData
        {
            get
            {
                return new HexMapTransformData
                {
                    orientation = pointyTopHexagons ? HexOrientation.PointyTop() : HexOrientation.FlatTop(),
                    scale = mapScale,
                    origin = mapOffset
                };
            }
        }
    }
}