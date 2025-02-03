using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.DataScribes;

namespace GalacticBoundStudios.HexTech
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

    // Stores in Axial coordinates
    public struct HexCoord : System.IEquatable<HexCoord>
    {
        public int q;
        public int r;
        public int s { get { return -q - r; } }

        public HexCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + q.GetHashCode();
            hash = hash * 31 + r.GetHashCode();
            return hash;
        }

        public bool Equals(HexCoord other)
        {
            return this.q == other.q && this.r == other.r;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", q, r, s);
        }
    }

    public struct FractionalHexCoord
    {
        public float q;
        public float r;
        public float s { get { return -q - r; } }

        public FractionalHexCoord(float q, float r)
        {
            this.q = q;
            this.r = r;
        }
    }

    // This struct contains a 2-D array of bools that define if a hexagon should
    // be created
    [ChunkSerializable]
    public struct HexagonActivationGrid : IComponentData
    {
        // This 2D array of bools holds which hexagons are active
        public NativeHashMap<HexCoord, byte> hexGrid;

        // Seed for random generation
        public uint randomSeed;

        public int gridWidth;
        public int gridHeight;
    }

    [CreateAssetMenu(menuName = "HexTech/HexMapConfig")]
    public class HexMapConfig : ScriptableObject
    {
        public bool pointyTopHexagons = true;
        public Vector2 mapScale = Vector2.one;
        public Vector3 mapOffset = Vector3.zero;

        public bool hollow = false;
        public float innerRadius = 0.7f;

        public HexGridShape gridShape = HexGridShape.Hexagon;
    }
}
