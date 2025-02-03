using GalacticBoundStudios.DataScribes;
using Unity.Mathematics;

namespace GalacticBoundStudios.HexTech
{
    public static class HexMath
    {
        #region Arithmetic

        public static HexCoord Add(HexCoord a, HexCoord b)
        {
            return new HexCoord(a.q + b.q, a.r + b.r);
        }

        public static HexCoord Subtract(HexCoord a, HexCoord b)
        {
            return new HexCoord(a.q - b.q, a.r - b.r);
        }

        public static HexCoord Scale(HexCoord a, int k)
        {
            return new HexCoord(a.q * k, a.r * k);
        }

        #endregion // Arithmetic

        #region Distance

        public static int Length(HexCoord a)
        {
            return (int)((math.abs(a.q) + math.abs(a.r) + math.abs(a.s)) / 2);
        }

        public static int Distance(HexCoord a, HexCoord b)
        {
            return Length(Subtract(a, b));
        }

        #endregion // Distance

        #region Neighbors

        public static FixedArray<HexCoord> HexDirections()
        {
            FixedArray<HexCoord> hex_directions = new FixedArray<HexCoord>(6);

            hex_directions[0] = new HexCoord(1, 0);
            hex_directions[1] = new HexCoord(1, -1);
            hex_directions[2] = new HexCoord(0, -1);
            hex_directions[3] = new HexCoord(-1, 0);
            hex_directions[4] = new HexCoord(-1, 1);
            hex_directions[5] = new HexCoord(0, 1);

            return hex_directions;
        }

        public static HexCoord Direction(int direction)
        {
            switch (direction)
            {
                case 0:
                    return new HexCoord(1, 0);

                case 1:
                    return new HexCoord(1, -1);

                case 2:
                    return new HexCoord(0, -1);

                case 3:
                    return new HexCoord(-1, 0);

                case 4:
                    return new HexCoord(-1, 1);

                case 5:
                    return new HexCoord(0, 1);

                default:
                    return new HexCoord(0, 0);
            }
        }

        public static HexCoord Neighbor(HexCoord hex, int direction)
        {
            return Add(hex, Direction(direction));
        }

        #endregion // Neighbors

        #region Screen to Hex

        public static float2 HexToPixel(HexCoord coord, in HexMapTransformData layout)
        {
            HexOrientation orientation = layout.orientation;
            double x = (orientation.f0 * coord.q + orientation.f1 * coord.r) * layout.scale.x;
            double y = (orientation.f2 * coord.q + orientation.f3 * coord.r) * layout.scale.y;
            return new float2((float)(x + layout.origin.x), (float)(y + layout.origin.y));
        }

        public static HexCoord PixelToHex(float2 p, HexMapTransformData layout)
        {
            HexOrientation orientation = layout.orientation;
            float q = (orientation.b0 * p.x + orientation.b1 * p.y) / layout.scale.x;
            float r = (orientation.b2 * p.x + orientation.b3 * p.y) / layout.scale.y;
            return HexRound(new FractionalHexCoord(q, r));
        }

        public static HexCoord HexRound(FractionalHexCoord h)
        {
            int q = (int)(math.round(h.q));
            int r = (int)(math.round(h.r));
            int s = (int)(math.round(h.s));
            double q_diff = math.abs(q - h.q);
            double r_diff = math.abs(r - h.r);
            double s_diff = math.abs(s - h.s);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                q = -r - s;
            }
            else if (r_diff > s_diff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }
            return new HexCoord(q, r);
        }

        #endregion // Screen to Hex

        #region Mesh Utils

        public static float2 HexCornerOffset(in HexMapTransformData layout, int corner)
        {
            float2 scale = layout.scale;
            float angle = 2.0f * math.PI * (layout.orientation.startAngle + corner) / 6;
            return new float2(scale.x * math.cos(angle), scale.y * math.sin(angle));
        }

        public static FixedArray<float2> PolygonCorners(in HexMapTransformData layout, HexCoord hex)
        {
            FixedArray<float2> corners = new FixedArray<float2>(6);
            float2 center = HexToPixel(hex, in layout);
            for (int i = 0; i < 6; i++)
            {
                float2 offset = HexCornerOffset(layout, i);
                corners[i] = new float2(center.x + offset.x, center.y + offset.y);
            }
            return corners;
        }

        #endregion // Mesh Utils
    }
}