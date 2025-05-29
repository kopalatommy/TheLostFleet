using Unity.Entities;
using Unity.Mathematics;

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
}