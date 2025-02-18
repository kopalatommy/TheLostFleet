namespace ProjectWorlds
{
    [System.Serializable]
    public struct Vector4d
    {
        public static Vector4d zero
        {
            get
            {
                return new Vector4d(0, 0, 0, 0);
            }
        }

        public static Vector4d one
        {
            get
            {
                return new Vector4d(1, 1, 1, 1);
            }
        }

        public static Vector4d right
        {
            get
            {
                return new Vector4d(1, 0, 0, 0);
            }
        }

        public static Vector4d left
        {
            get
            {
                return new Vector4d(-1, 0, 0, 0);
            }
        }

        public static Vector4d up
        {
            get
            {
                return new Vector4d(0, 1, 0, 0);
            }
        }

        public static Vector4d down
        {
            get
            {
                return new Vector4d(0, -1, 0, 0);
            }
        }

        public static Vector4d forward
        {
            get
            {
                return new Vector4d(0, 0, 1, 0);
            }
        }

        public static Vector4d back
        {
            get
            {
                return new Vector4d(0, 0, -1, 0);
            }
        }

        public static Vector4d infinity
        {
            get
            {
                return new Vector4d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            }
        }

        public static Vector4d negativeInfinity
        {
            get
            {
                return new Vector4d(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            }
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Vector4d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Vector4d index!");
                }
            }
        }

        public double magnitude
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        public Vector4d normalized
        {
            get
            {
                return Vector4d.Normalize(this);
            }
        }

        public double x;
        public double y;
        public double z;
        public double w;

#if NETCOREAPP3_1
        public Vector4d()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }
#endif
        public Vector4d(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        
        public void Set(double newX, double newY, double newZ, double newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        public void Scale(Vector4d scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        public static Vector4d Scale(Vector4d a, Vector4d b)
        {
            return new Vector4d(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public static Vector4d Cross(Vector4d a, Vector4d b)
        {
            return new Vector4d(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x, 0);
        }

        public void Normalize()
        {
            double num = magnitude;
            if (num > 9.99999974737875E-06)
            {
                this = this / num;
            }
            else
            {
                this = zero;
            }
        }

        public static Vector4d Normalize(Vector4d a)
        {
            double num = a.magnitude;
            if (num > 9.99999974737875E-06)
            {
                return a / num;
            }
            return zero;
        }

        public static Vector4d Reflect(Vector4d inDirection, Vector4d inNormal)
        {
            return -2 * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static double Dot(Vector4d a, Vector4d b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Vector4d Project(Vector4d a, Vector4d b)
        {
            return b * Dot(a, b) / Dot(b, b);
        }

        public static Vector4d ProjectOnPlane(Vector4d vector, Vector4d planeNormal)
        {
            return vector - Vector4d.Project(vector, planeNormal);
        }

        public static double Distance(Vector4d a, Vector4d b)
        {
            return (a - b).magnitude;
        }

        public static double Magnitude(Vector4d a)
        {
            return a.magnitude;
        }

        public static double SqrMagnitude(Vector4d a)
        {
            return a.sqrMagnitude;
        }

        public static Vector4d Min(Vector4d lhs, Vector4d rhs)
        {
            return new Vector4d(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y), System.Math.Min(lhs.z, rhs.z), System.Math.Min(lhs.w, rhs.w));
        }

        public static Vector4d Max(Vector4d lhs, Vector4d rhs)
        {
            return new Vector4d(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y), System.Math.Max(lhs.z, rhs.z), System.Math.Max(lhs.w, rhs.w));
        }

        public static double Angle(Vector4d a, Vector4d b)
        {
            return System.Math.Acos(Mathd.Clamp(Vector4d.Dot(a.normalized, b.normalized), -1, 1)) * 57.29578;
        }

        public static double SignedAngle(Vector4d a, Vector4d b, Vector4d axis)
        {
            Vector4d normalized = Vector4d.Cross(a, b).normalized;
            return Vector4d.Angle(a, b) * (Vector4d.Dot(normalized, axis) < 0 ? -1 : 1);
        }

        public static Vector4d operator +(Vector4d a, double d)
        {
            return new Vector4d(a.x + d, a.y + d, a.z + d, a.w + d);
        }

        public static Vector4d operator +(Vector4d a, Vector4d b)
        {
            return new Vector4d(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4d operator -(Vector4d a, Vector4d b)
        {
            return new Vector4d(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4d operator -(Vector4d a)
        {
            return new Vector4d(-a.x, -a.y, -a.z, -a.w);
        }

        public static Vector4d operator *(Vector4d a, double d)
        {
            return new Vector4d(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4d operator *(double d, Vector4d a)
        {
            return new Vector4d(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4d operator /(Vector4d a, double d)
        {
            return new Vector4d(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        public static bool operator ==(Vector4d lhs, Vector4d rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.99999944E-11;
        }

        public static bool operator !=(Vector4d lhs, Vector4d rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector4d Lerp(Vector4d a, Vector4d b, double t)
        {
            t = System.Math.Max(0, System.Math.Min(1, t));
            return new Vector4d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        public static Vector4d LerpUnclamped(Vector4d a, Vector4d b, double t)
        {
            return new Vector4d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        public static Vector4d MoveTowards(Vector4d current, Vector4d target, double maxDistanceDelta)
        {
            Vector4d a = target - current;
            double magnitude = a.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }

        public override bool Equals(object? other)
        {
            if (!(other is Vector4d))
            {
                return false;
            }
            Vector4d vector = (Vector4d)other;
            return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
        }

        public override string ToString()
        {
            return $"({x:F1}, {y:F1}, {z:F1}, {w:F1})";
        }
    }
}