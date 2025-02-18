using System;

namespace ProjectWorlds.PermutationNoise
{
    public class LinearCongruentGenerator : PermutationNoise
    {
        public static LinearCongruentGenerator Default
        {
            get
            {
                return new LinearCongruentGenerator((uint)DateTime.Now.Ticks);
            }
        }
        
        public static LinearCongruentGenerator GLibC
        {
            get
            {
                return new LinearCongruentGenerator((uint)DateTime.Now.Ticks, 1103515245, 12345, 2147483648);
            }
        }

        public ulong Multiplier
        {
            get
            {
                return multiplier;
            }
            set
            {
                multiplier = value;
                Reset();
            }
        }

        public ulong Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value;
                Reset();
            }
        }

        public ulong Modulus
        {
            get
            {
                return modulus;
            }
            set
            {
                modulus = value;
                Reset();
            }
        }

        protected ulong multiplier;
        protected ulong increment;
        protected ulong modulus;

        public LinearCongruentGenerator() : this((uint)DateTime.Now.Ticks)
        {
            multiplier = 1664525;
            increment = 1013904223;
            modulus = 4294967296;

            Reset();
        }

        public LinearCongruentGenerator(uint seed) : base(seed)
        {
            multiplier = 1664525;
            increment = 1013904223;
            modulus = 4294967296;
        }

        public LinearCongruentGenerator(uint seed, uint multiplier, uint increment, uint modulus) : base(seed)
        {
            this.multiplier = multiplier;
            this.increment = increment;
            this.modulus = modulus;
        }

        public override void SetSeed(int seed)
        {
            this.seed = (uint)seed;
            Reset();
        }

        public override void Reset()
        {
            count = 0;
            CurrentValue = seed;
        }

        public override float NextValue()
        {
            count++;
            CurrentValue = (multiplier * CurrentValue + increment) % modulus;
            return (float)CurrentValue / modulus;
        }

        public override float NextValue(float x)
        {
            return NextValue() * x;
        }

        public override float NextValue(float x, float y)
        {
            return NextValue() * (y - x) + x;
        }

        public override string ToString()
        {
            return $"LCG: Seed: {Seed}, Multiplier: {Multiplier}, Increment: {Increment}, Modulus: {Modulus}";
        }
    }
}