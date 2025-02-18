using System;

namespace ProjectWorlds.PermutationNoise
{
    public class MersenneTwister : PermutationNoise
    {
        public static MersenneTwister Default
        {
            get
            {
                return new MersenneTwister((uint)DateTime.Now.Ticks);
            }
        }

        protected const uint wordSide = 32;
        protected const uint degreeOfRecurrence = 624;
        protected const uint middleWord = 397;
        protected const int separationPoint = 31;
        protected const ulong a = 0x9908b0dfUL;
        protected const uint u = 11;
        protected const uint s = 7;
        protected const uint t = 15;
        protected const uint l = 18;
        protected const ulong b = 0x9d2c5680UL;
        protected const ulong c = 0xefc60000UL;
        protected const uint f = 1812433253;

        protected const ulong UMASK = (0xFFFFFFFF << separationPoint);
        protected const ulong LMASK = (0xFFFFFFFF >> (int)(wordSide - separationPoint));

        protected uint[] mt;
        protected uint index;

        public MersenneTwister() : this((uint)DateTime.Now.Ticks)
        {

        }

        public MersenneTwister(uint seed)
        {
            mt = new uint[degreeOfRecurrence];
            Reset();
        }

        public override void Reset()
        {
            mt[0] = seed;
            for (uint i = 1; i < degreeOfRecurrence; i++)
            {
                mt[i] = (uint)(f * (mt[i - 1] ^ (mt[i - 1] >> (int)(wordSide - 2))) + i);
            }
            index = degreeOfRecurrence;

            count = 0;
        }

        public override float NextValue()
        {
            if (index >= degreeOfRecurrence)
            {
                Twist();
            }

            uint y = mt[index++];
            y ^= (y >> (int)u) & 0xFFFFFFFF;
            y ^= (y << (int)s) & 0x9d2c5680;
            y ^= (y << (int)t) & 0xefc60000;
            y ^= (y >> (int)l) & 0xFFFFFFFF;

            count++;

            return y / (float)0xFFFFFFFF;
        }

        public override float NextValue(float x)
        {
            return NextValue() * x;
        }

        public override float NextValue(float x, float y)
        {
            return NextValue() * (y - x) + x;
        }

        protected void Twist()
        {
            for (uint i = 0; i < degreeOfRecurrence; i++)
            {
                uint x = (uint)((mt[i] & UMASK) + (mt[(i + 1) % degreeOfRecurrence] & LMASK));
                uint xA = x >> 1;
                if (x % 2 != 0)
                {
                    xA ^= (uint)a;
                }
                mt[i] = mt[(i + middleWord) % degreeOfRecurrence] ^ xA;
            }
            index = 0;
        }

        public override void SetSeed(int seed)
        {
            this.seed = (uint)seed;
            Reset();
        }
    }
}