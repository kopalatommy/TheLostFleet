namespace ProjectWorlds
{
    /// <summary>
    /// Same as the unity Vector3, but uses doubles instead of floats
    /// </summary>
    [System.Serializable]
    public class Vector3d
    {
        public static Vector3d zero
        {
            get
            {
                return new Vector3d(0.0, 0.0, 0.0);
            }
        }

        public static Vector3d one
        {
            get
            {
                return new Vector3d(1.0, 1.0, 1.0);
            }
        }

        public static Vector3d forward
        {
            get
            {
                return new Vector3d(0.0, 0.0, 1.0);
            }
        }

        public static Vector3d back
        {
            get
            {
                return new Vector3d(0.0, 0.0, -1.0);
            }
        }

        public static Vector3d up
        {
            get
            {
                return new Vector3d(0.0, 1.0, 0.0);
            }
        }

        public static Vector3d down
        {
            get
            {
                return new Vector3d(0.0, -1.0, 0.0);
            }
        }

        public static Vector3d left
        {
            get
            {
                return new Vector3d(-1.0, 0.0, 0.0);
            }
        }

        public static Vector3d right
        {
            get
            {
                return new Vector3d(1.0, 0.0, 0.0);
            }
        }

        public static Vector3d Infinity
        {
            get
            {
                return new Vector3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            }
        }

        public static Vector3d NegativeInfinity
        {
            get
            {
                return new Vector3d(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public double magnitude
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
        }

        public Vector3d normalized
        {
            get
            {
                return Normalize(this);
            }
        }

        public double x;
        public double y;
        public double z;

        public Vector3d()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.z = 0.0;
        }
        public Vector3d(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0;
        }
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///   <para>Set x, y and z components of an existing Vector3d.</para>
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="newZ"></param>
        public void Set(double newX, double newY, double newZ)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
        }

        /// <summary>
        ///   <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3d Lerp(Vector3d a, Vector3d b, double t)
        {
            t = Mathd.Clamp01(t);
            return new Vector3d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Vector3d LerpUnclamped(Vector3d a, Vector3d b, double t)
        {
            return new Vector3d(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3d Slerp(Vector3d a, Vector3d b, double t)
        {
            return Slerp(a, b, t, 0.0);
        }

        public static Vector3d Slerp(Vector3d a, Vector3d b, double t, double r)
        {
            double num = Mathd.Clamp(Vector3d.Dot(a, b), -1f, 1f);
            double num2 = Mathd.Acos(num);
            double num3 = Mathd.Sin(num2);
            Vector3d result;
            if (num3 < 1E-05f)
            {
                result = Vector3d.Lerp(a, b, t);
            }
            else
            {
                double num4 = Mathd.Sin((1f - t) * num2) / num3;
                double num5 = Mathd.Sin(t * num2) / num3;
                result = new Vector3d(a.x * num4 + b.x * num5, a.y * num4 + b.y * num5, a.z * num4 + b.z * num5);
            }
            return result;
        }

        /// <summary>
        ///   <para>Moves a point current in a straight line towards a target point.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
        {
            Vector3d a = target - current;
            double magnitude = a.magnitude;
            Vector3d result;
            if (magnitude <= maxDistanceDelta || magnitude < 1.401298E-45f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        /// <summary>
        ///   <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector3d Scale(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3d scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        /// <summary>
        ///   <para>Cross Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }

        /// <summary>
        ///   <para>Returns true if the given vector is exactly equal to this vector.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object? other)
        {
            bool result;
            if (other == null)
            {
                result = false;
            }
            else if (!(other is Vector3d))
            {
                result = false;
            }
            else
            {
                Vector3d vector = (Vector3d)other;
                result = (this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z));
            }
            return result;
        }

        /// <summary>
        ///   <para>Reflects a vector off the plane defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal)
        {
            return -2f * Vector3d.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        /// <summary>
        ///   <para></para>
        /// </summary>
        /// <param name="value"></param>
        public static Vector3d Normalize(Vector3d value)
        {
            double num = Vector3d.Magnitude(value);
            Vector3d result;
            if (num > 1E-05f)
            {
                result = value / num;
            }
            else
            {
                result = Vector3d.zero;
            }
            return result;
        }

        /// <summary>
        ///   <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        public void Normalize()
        {
            double num = Vector3d.Magnitude(this);
            if (num > 1E-05f)
            {
                this.x /= num;
                this.y /= num;
                this.z /= num;
            }
            else
            {
                this.x = this.y = this.z = 0;
            }
        }

        /// <summary>
        ///   <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static double Dot(Vector3d lhs, Vector3d rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        /// <summary>
        ///   <para>Projects a vector onto another vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="onNormal"></param>
        public static Vector3d Project(Vector3d vector, Vector3d onNormal)
        {
            double num = Vector3d.Dot(onNormal, onNormal);
            Vector3d result;
            if (num < Mathd.Epsilon)
            {
                result = Vector3d.zero;
            }
            else
            {
                result = onNormal * Vector3d.Dot(vector, onNormal) / num;
            }
            return result;
        }

        /// <summary>
        ///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        public static Vector3d ProjectOnPlane(Vector3d vector, Vector3d planeNormal)
        {
            return vector - Vector3d.Project(vector, planeNormal);
        }

        /// <summary>
        ///   <para>Returns the angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        public static double Angle(Vector3d from, Vector3d to)
        {
            return Mathd.Acos(Mathd.Clamp(Vector3d.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
        }

        /// <summary>
        ///   <para>Returns the signed angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        /// <param name="axis">A vector around which the other vectors are rotated.</param>
        public static double SignedAngle(Vector3d from, Vector3d to, Vector3d axis)
        {
            Vector3d normalized = from.normalized;
            Vector3d normalized2 = to.normalized;
            double num = Mathd.Acos(Mathd.Clamp(Vector3d.Dot(normalized, normalized2), -1f, 1f)) * 57.29578f;
            double num2 = Mathd.Sign(Vector3d.Dot(axis, Vector3d.Cross(normalized, normalized2)));
            return num * num2;
        }

        /// <summary>
        ///   <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static double Distance(Vector3d a, Vector3d b)
        {
            Vector3d vector = new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
            return Mathd.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        /// <summary>
        ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
        {
            Vector3d result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }

        public static double Magnitude(Vector3d vector)
        {
            return Mathd.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static double SqrMagnitude(Vector3d vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

#if UNITY_STANDALONE
        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed)
        {
            double deltaTime = UnityEngine.Time.deltaTime;
            return Vector3d.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime)
        {
            double deltaTime = UnityEngine.Time.deltaTime;
            double maxSpeed = double.PositiveInfinity;
            return Vector3d.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }
#endif // UNITY_STANDALONE

        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed = double.MaxValue, double deltaTime=0.1)
        {
            smoothTime = Mathd.Max(0.0001f, smoothTime);
            double num = 2f / smoothTime;
            double num2 = num * deltaTime;
            double d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            Vector3d vector = current - target;
            Vector3d vector2 = target;
            double maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector3d Vector3d = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * Vector3d) * d;
            Vector3d vector4 = target + (vector + Vector3d) * d;
            if (Vector3d.Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        public static Vector3d Min(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y), Mathd.Min(lhs.z, rhs.z));
        }

        public static Vector3d Max(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y), Mathd.Max(lhs.z, rhs.z));
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator -(Vector3d a)
        {
            return new Vector3d(-a.x, -a.y, -a.z);
        }

        public static Vector3d operator *(Vector3d a, double d)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator *(double d, Vector3d a)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator /(Vector3d a, double d)
        {
            return new Vector3d(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return Vector3d.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return !(lhs == rhs);
        }

#if UNITY_STANDALONE
        public static explicit operator UnityEngine.Vector3(Vector3d a)
        {
            return new UnityEngine.Vector3((float)a.x, (float)a.y, (float)a.z);
        }
        public static explicit operator Vector3d(UnityEngine.Vector3 a)
        {
            return new Vector3d(a.x, a.y, a.z);
        }
#endif // UNITY_STANDALONE

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
            {
                this.x,
                this.y,
                this.z
            });
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", new object[]
            {
                this.x.ToString(format),
                this.y.ToString(format),
                this.z.ToString(format)
            });
        }

        public static Vector3d Orthogonal(Vector3d v)
        {
            if (v.x == 0)
            {
                return new Vector3d(1, 0, 0);
            }
            else
            {
                return new Vector3d(-v.y, v.x, 0);
            }
        }
    }
}