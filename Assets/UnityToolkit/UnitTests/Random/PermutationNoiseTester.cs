using ProjectWorlds.Testing;
using ProjectWorlds.PermutationNoise;

namespace ProjectWorlds.UnitTests
{
    public class PermutationNoiseTester<T> : TesterBase where T : PermutationNoise.PermutationNoise, new()
    {
        public PermutationNoiseTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(PermutationNoiseTester<T>); } }

        public virtual T CreatePermutationNoise()
        {
            return new T();
        }

        [RunTest(true)]
        public bool TestPermutationNoise()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            noise.NextValue();
            noise.NextValue(0);
            noise.NextValue(0, 0);
            noise.SetSeed(0);
            return true;
        }

        #region Next Value Tests

        [RunTest(true)]
        public bool TestNextValue()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            float t = noise.NextValue();
            return t >= 0 && t <= 1;
        }

        [RunTest(true)]
        public bool TestNextValue100Items()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            
            for (int i = 0; i < 100; i++)
            {
                float t = noise.NextValue();
                if (t < 0 || t > 1)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion // NextValueTests

        #region NextValueX Tests

        [RunTest(true)]
        public bool TestNextValueX()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            float t = noise.NextValue(100);
            return t >= 0 && t <= 100;
        }

        [RunTest(true)]
        public bool TestNextValueX100Items()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            
            for (int i = 0; i < 100; i++)
            {
                float t = noise.NextValue(100);
                if (t < 0 || t > 100)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion // NextValueX Tests

        #region NextValueXY Tests

        [RunTest(true)]
        public bool TestNextValueXY()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            float t = noise.NextValue(0, 1);
            return t >= 0 && t <= 1;
        }

        [RunTest(true)]
        public bool TestNextValueXY100Items()
        {
            T noise = CreatePermutationNoise();
            noise.Reset();
            
            for (int i = 0; i < 100; i++)
            {
                float t = noise.NextValue(0, 1);
                if (t < 0 || t > 1)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion // NextValueXY Tests

        [RunTest(true)]
        public bool Test_DifferentValues()
        {
            float[] values = new float[100];
            T noise = CreatePermutationNoise();

            noise.Reset();

            for (int i = 0; i < 100; i++)
            {
                values[i] = noise.NextValue();
            }

            for (int i = 0; i < 99; i++)
            {
                if (values[i] == values[i + 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}