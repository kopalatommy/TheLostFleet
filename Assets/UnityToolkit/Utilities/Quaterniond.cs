using System.Diagnostics.CodeAnalysis;

namespace ProjectWorlds
{
    [System.Serializable]
    public struct Quaterniond
    {
        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public Quaterniond inverse
        {
            get
            {
                double lengthSq = 1.0 / sqrMagnitude;
                return new Quaterniond(-x * lengthSq, -y * lengthSq, -z * lengthSq, w * lengthSq);
            }
        }

        public static Quaterniond identity
        {
            get
            {
                return new Quaterniond(0.0, 0.0, 0.0, 1.0);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        public double magnitude
        {
            get
            {
                return Mathd.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public Quaterniond normalized
        {
            get
            {
                double num = 1.0 / magnitude;
                return new Quaterniond(x * num, y * num, z * num, w * num);
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
                    case 3:
                        return this.w;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Quaterniond index!");
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
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("Invalid Quaterniond index!");
                }
            }
        }

        /// <summary>
        ///   <para>X component of the Quaterniond. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public double x;

        /// <summary>
        ///   <para>Y component of the Quaterniond. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public double y;

        /// <summary>
        ///   <para>Z component of the Quaterniond. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public double z;

        /// <summary>
        ///   <para>W component of the Quaterniond. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public double w;

        public Quaterniond(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Set(double new_x, double new_y, double new_z, double new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }

        public static Quaterniond Euler(Vector3d euler)
        {
            return Euler(euler.x, euler.y, euler.z);
        }

        public static Quaterniond Euler(double x, double y, double z)
        {
            x *= Mathd.Deg2Rad;
            y *= Mathd.Deg2Rad;
            z *= Mathd.Deg2Rad;

            double yawOver2 = x * 0.5;
            double cosYawOver2 = System.Math.Cos(yawOver2);
            double sinYawOver2 = System.Math.Sin(yawOver2);
            double pitchOver2 = y * 0.5;
            double cosPitchOver2 = System.Math.Cos(pitchOver2);
            double sinPitchOver2 = System.Math.Sin(pitchOver2);
            double rollOver2 = z * 0.5;
            double cosRollOver2 = System.Math.Cos(rollOver2);
            double sinRollOver2 = System.Math.Sin(rollOver2);
            Quaterniond result;
            result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            return result;
        }

        public static Quaterniond operator *(Quaterniond lhs, Quaterniond rhs)
        {
            return new Quaterniond(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
            );
        }

        public static Vector3d operator *(Quaterniond rotation, Vector3d point)
        {
            double num = rotation.x * 2.0;
            double num2 = rotation.y * 2.0;
            double num3 = rotation.z * 2.0;
            double num4 = rotation.x * num;
            double num5 = rotation.y * num2;
            double num6 = rotation.z * num3;
            double num7 = rotation.x * num2;
            double num8 = rotation.x * num3;
            double num9 = rotation.y * num3;
            double num10 = rotation.w * num;
            double num11 = rotation.w * num2;
            double num12 = rotation.w * num3;
            Vector3d result = new Vector3d();
            result.x = (1.0 - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1.0 - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1.0 - (num4 + num5)) * point.z;
            return result;
        }

        public static bool operator ==(Quaterniond lhs, Quaterniond rhs)
        {
            return Quaterniond.Dot(lhs, rhs) > 0.999999;
        }

        public static bool operator !=(Quaterniond lhs, Quaterniond rhs)
        {
            return Quaterniond.Dot(lhs, rhs) <= 0.999999;
        }

        public static Quaterniond operator /(Quaterniond lhs, double rhs)
        {
            return new Quaterniond(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
        }

        public static double Dot(Quaterniond a, Quaterniond b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static double Angle(Quaterniond a, Quaterniond b)
        {
            double f = Quaterniond.Dot(a, b);
            return Mathd.Acos(Mathd.Min(Mathd.Abs(f), 1.0)) * 2.0 * Mathd.Rad2Deg;
        }

        public static Quaterniond AngleAxis(double angle, Vector3d axis)
        {
            Quaterniond result;
            double num = angle * 0.5;
            double num2 = System.Math.Sin(num);
            double num3 = System.Math.Cos(num);
            result.x = axis.x * num2;
            result.y = axis.y * num2;
            result.z = axis.z * num2;
            result.w = num3;
            return result;
        }

        public static Quaterniond LookRotation(Vector3d forward, Vector3d upwards)
        {
            forward.Normalize();
            Vector3d right = Vector3d.Cross(upwards, forward);
            upwards = Vector3d.Cross(forward, right);
            double m00 = right.x;
            double m01 = right.y;
            double m02 = right.z;
            double m10 = upwards.x;
            double m11 = upwards.y;
            double m12 = upwards.z;
            double m20 = forward.x;
            double m21 = forward.y;
            double m22 = forward.z;
            double num8 = m00 + m11 + m22;
            Quaterniond quaternion = new Quaterniond();
            if (num8 > 0.0)
            {
                double num = Mathd.Sqrt(num8 + 1.0);
                quaternion.w = num * 0.5;
                num = 0.5 / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if (m00 >= m11 && m00 >= m22)
            {
                double num7 = Mathd.Sqrt(1.0 + m00 - m11 - m22);
                double num4 = 0.5 / num7;
                quaternion.x = 0.5 * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                double num6 = Mathd.Sqrt(1.0 + m11 - m00 - m22);
                double num3 = 0.5 / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5 * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            double num5 = Mathd.Sqrt(1.0 + m22 - m00 - m11);
            double num2 = 0.5 / num5;

            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5 * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }

        public static Quaterniond Slerp(Quaterniond a, Quaterniond b, double t)
        {
            double num = Quaterniond.Dot(a, b);
            bool flag = false;
            if (num < 0.0)
            {
                flag = true;
                num = -num;
            }
            double num2;
            double num3;
            if (num > 0.999999)
            {
                num2 = 1.0 - t;
                num3 = (flag ? (-t) : t);
            }
            else
            {
                double num4 = Mathd.Acos(num);
                double num5 = 1.0 / Mathd.Sin(num4);
                num2 = Mathd.Sin((1.0 - t) * num4) * num5;
                num3 = (flag ? (-Mathd.Sin(t * num4) * num5) : (Mathd.Sin(t * num4) * num5));
            }
            Quaterniond result;
            result.x = num2 * a.x + num3 * b.x;
            result.y = num2 * a.y + num3 * b.y;
            result.z = num2 * a.z + num3 * b.z;
            result.w = num2 * a.w + num3 * b.w;
            return result;
        }

        public static Quaterniond Lerp(Quaterniond a, Quaterniond b, double t)
        {
            if (t < 0.0)
            {
                return a;
            }
            if (t > 1.0)
            {
                return b;
            }
            return new Quaterniond(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            ).normalized;
        }

        public static Quaterniond FromToRotation(Vector3d fromDirection, Vector3d toDirection)
        {
            Vector3d normalized = fromDirection.normalized;
            Vector3d normalized2 = toDirection.normalized;
            double d = Vector3d.Dot(normalized, normalized2);
            if (d > -1.0 + 1E-6)
            {
                Vector3d vector = Vector3d.Cross(normalized, normalized2);
                double w = Mathd.Sqrt((1.0 + d) * 2.0);
                double num = 1.0 / w;
                return new Quaterniond(vector.x * num, vector.y * num, vector.z * num, w).normalized;
            }
            Vector3d vector2 = Vector3d.Orthogonal(normalized).normalized;
            return Quaterniond.AngleAxis(180.0, vector2) * Quaterniond.AngleAxis(180.0, Vector3d.Cross(normalized, vector2));
        }

        public static Quaterniond RotateTowards(Quaterniond from, Quaterniond to, double maxDegreesDelta)
        {
            double num = Quaterniond.Angle(from, to);
            if (num == 0.0)
            {
                return to;
            }
            double t = Mathd.Min(1.0, maxDegreesDelta / num);
            return Quaterniond.SlerpUnclamped(from, to, t);
        }

        public static Quaterniond Normalize(Quaterniond q)
        {
            double num = 1.0 / Mathd.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaterniond(q.x * num, q.y * num, q.z * num, q.w * num);
        }

        public static Quaterniond NormalizeSafe(Quaterniond q)
        {
            double num = q.magnitude;
            if (num == 0.0)
            {
                return Quaterniond.identity;
            }
            return q / num;
        }

        public static Quaterniond SlerpUnclamped(Quaterniond a, Quaterniond b, double t)
        {
            double num = Quaterniond.Dot(a, b);
            bool flag = false;
            if (num < 0.0)
            {
                flag = true;
                num = -num;
            }
            double num2;
            double num3;
            if (num > 0.999999)
            {
                num2 = 1.0 - t;
                num3 = (flag ? (-t) : t);
            }
            else
            {
                double num4 = Mathd.Acos(num);
                double num5 = 1.0 / Mathd.Sin(num4);
                num2 = Mathd.Sin((1.0 - t) * num4) * num5;
                num3 = (flag ? (-Mathd.Sin(t * num4) * num5) : (Mathd.Sin(t * num4) * num5));
            }
            Quaterniond result;
            result.x = num2 * a.x + num3 * b.x;
            result.y = num2 * a.y + num3 * b.y;
            result.z = num2 * a.z + num3 * b.z;
            result.w = num2 * a.w + num3 * b.w;
            return result;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Quaterniond))
            {
                return false;
            }
            Quaterniond quaternion = (Quaterniond)obj;
            return x.Equals(quaternion.x) && y.Equals(quaternion.y) && z.Equals(quaternion.z) && w.Equals(quaternion.w);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
        }

        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
        }
    }
}