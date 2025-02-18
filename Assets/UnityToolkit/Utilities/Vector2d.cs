namespace ProjectWorlds
{
    [System.Serializable]
    public struct Vector2d
    {
        public static Vector2d zero
        {
            get
            {
                return new Vector2d(0, 0);
            }
        }

        public static Vector2d one
        {
            get
            {
                return new Vector2d(1, 1);
            }
        }

        public static Vector2d right
        {
            get
            {
                return new Vector2d(1, 0);
            }
        }

        public static Vector2d left
        {
            get
            {
                return new Vector2d(-1, 0);
            }
        }

        public static Vector2d up
        {
            get
            {
                return new Vector2d(0, 1);
            }
        }

        public static Vector2d down
        {
            get
            {
                return new Vector2d(0, -1);
            }
        }

        public static Vector2d infinity
        {
            get
            {
                return new Vector2d(double.PositiveInfinity, double.PositiveInfinity);
            }
        }

        public static Vector2d negativeInfinity
        {
            get
            {
                return new Vector2d(double.NegativeInfinity, double.NegativeInfinity);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return x;
                }
                else if (index == 1)
                {
                    return y;
                }
                else
                {
                    throw new System.IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
            set
            {
                if (index == 0)
                {
                    x = value;
                }
                else if (index == 1)
                {
                    y = value;
                }
                else
                {
                    throw new System.IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
        }

        public Vector2d normalized
        {
            get
            {
                return Vector2d.Normalize(this);
            }
        }

        public double magnitude
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public double x;
        public double y;

#if NETCOREAPP3_1
        public Vector2d()
        {
            // Code to be executed if C# version is 9 or higher
            x = 0;
            y = 0;

        }
#endif
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x + b.x, a.y + b.y);
        }

        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x - b.x, a.y - b.y);
        }

        public static Vector2d operator *(Vector2d a, double d)
        {
            return new Vector2d(a.x * d, a.y * d);
        }

        public static Vector2d operator /(Vector2d a, double d)
        {
            return new Vector2d(a.x / d, a.y / d);
        }

        public static Vector2d operator *(double d, Vector2d a)
        {
            return new Vector2d(a.x * d, a.y * d);
        }

        public static Vector2d operator /(double d, Vector2d a)
        {
            return new Vector2d(a.x / d, a.y / d);
        }

        public static Vector2d operator -(Vector2d a)
        {
            return new Vector2d(-a.x, -a.y);
        }

        public static bool operator ==(Vector2d lhs, Vector2d rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2d lhs, Vector2d rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        public void Set(double newX, double newY)
        {
            x = newX;
            y = newY;
        }

        public static Vector2d Lerp(Vector2d a, Vector2d b, double t)
        {
            t = System.Math.Max(0, System.Math.Min(1, t));
            return new Vector2d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static Vector2d LerpUnclamped(Vector2d a, Vector2d b, double t)
        {
            return new Vector2d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static Vector2d MoveTowards(Vector2d current, Vector2d target, double maxDistanceDelta)
        {
            Vector2d a = target - current;
            double magnitude = a.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }

        public static Vector2d Scale(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x * b.x, a.y * b.y);
        }

        public void Scale(Vector2d scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        public static Vector2d Reflect(Vector2d inDirection, Vector2d inNormal)
        {
            return -2 * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static Vector2d Perpendicular(Vector2d inDirection)
        {
            return new Vector2d(-inDirection.y, inDirection.x);
        }

        public static double Dot(Vector2d lhs, Vector2d rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static double Angle(Vector2d from, Vector2d to)
        {
            return System.Math.Acos(System.Math.Clamp(Dot(from.normalized, to.normalized), -1, 1)) * 57.29578;
        }

        public static double SignedAngle(Vector2d from, Vector2d to)
        {
            double unsigned_angle = Angle(from, to);
            double sign = System.Math.Sign(from.x * to.y - from.y * to.x);
            return unsigned_angle * sign;
        }

        public static double Distance(Vector2d a, Vector2d b)
        {
            Vector2d vector2 = new Vector2d(a.x - b.x, a.y - b.y);
            return vector2.magnitude;
        }

        public static Vector2d ClampMagnitude(Vector2d vector, double maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }
            return vector;
        }

        public static double SqrMagnitude(Vector2d a)
        {
            return a.x * a.x + a.y * a.y;
        }

        public static double Magnitude(Vector2d a)
        {
            return System.Math.Sqrt(a.x * a.x + a.y * a.y);
        }

        public static Vector2d Project(Vector2d vector, Vector2d onNormal)
        {
            double num = Dot(onNormal, onNormal);
            if (num < double.Epsilon)
            {
                return zero;
            }
            return onNormal * Dot(vector, onNormal) / num;
        }

        public static Vector2d ProjectOnPlane(Vector2d vector, Vector2d planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        public static Vector2d Normalize(Vector2d value)
        {
            double mag = value.magnitude;
            if (mag > double.Epsilon)
            {
                return value / mag;
            }
            return zero;
        }

        public Vector2d Normalized()
        {
            return Vector2d.Normalize(this);
        }

        public static Vector2d Min(Vector2d lhs, Vector2d rhs)
        {
            return new Vector2d(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y));
        }

        public static Vector2d Max(Vector2d lhs, Vector2d rhs)
        {
            return new Vector2d(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y));
        }

        public double Magnitude()
        {
            return System.Math.Sqrt(x * x + y * y);
        }

        public double SqrMagnitude()
        {
            return x * x + y * y;
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2d))
            {
                return false;
            }

            Vector2d rhs = (Vector2d)other;
            return x == rhs.x && y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}