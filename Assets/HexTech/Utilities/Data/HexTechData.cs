using Unity.Collections;
using Unity.Entities;

namespace GalacticBoundStudios.HexTech
{
    // Stores in Axial coordinates
    public struct HexCoord : System.IEquatable<HexCoord>, IComponentData
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
}