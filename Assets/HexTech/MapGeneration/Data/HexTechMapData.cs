using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.DataScribes;

namespace GalacticBoundStudios.HexTech.MapGeneration
{
    // This struct defines a hexagon orientation. It is a 
    // 2d matrix that defines the layout of the hexagons.
    public struct HexOrientation : IComponentData
    {
        public float f0, f1, f2, f3;
        public float b0, b1, b2, b3;
        public float startAngle;

        public static HexOrientation PointyTop()
        {
            return new HexOrientation
            {
                f0 = math.sqrt(3.0f),
                f1 = math.sqrt(3.0f) / 2.0f,
                f2 = 0.0f,
                f3 = 3.0f / 2.0f,

                b0 = math.sqrt(3.0f) / 3.0f,
                b1 = -1.0f / 3.0f,
                b2 = 0.0f,
                b3 = 2.0f / 3.0f,

                startAngle = 0.5f
            };


            //return new HexOrientation(math.sqrt(3.0), math.sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, math.sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);
        }

        public static HexOrientation FlatTop()
        {
            return new HexOrientation
            {
                f0 = 3.0f / 2.0f,
                f1 = 0.0f,
                f2 = math.sqrt(3.0f) / 2.0f,
                f3 = math.sqrt(3.0f),

                b0 = 2.0f / 3.0f,
                b1 = 0.0f,
                b2 = -1.0f / 3.0f,
                b3 = math.sqrt(3.0f) / 3.0f,

                startAngle = 0.0f
            };

            //return new HexOrientation(3.0 / 2.0, 0.0, math.sqrt(3) / 2, math.sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3, math.sqrt(3) / 3, 0.0);
        }
    }

    public struct HexMapTransformData : IComponentData
    {
        public HexOrientation orientation;
        public float2 scale;
        public float3 origin;
    }

    public struct HexHollowData : IComponentData
    {
        public bool isHollow;
        // Percentage (0 - 1)
        public float innerRadius;
    }

    public struct HexTechMapEntityTag : IComponentData
    {
        
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