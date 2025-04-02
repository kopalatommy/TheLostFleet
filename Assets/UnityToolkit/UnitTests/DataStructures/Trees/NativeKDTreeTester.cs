using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using GalacticBoundStudios.DataScribes.Unmanaged;
using ProjectWorlds.Testing;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace ProjectWorlds.UnitTests
{
    public class NativeKDTreeTester : TesterBase
    {
        public NativeKDTreeTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(NativeKDTreeTester); } }

        [RunTest(true)]
        public bool AddTest_Add1Item()
        {
            try
            {
                NativeArray<float3> points = new NativeArray<float3>(1, Allocator.TempJob);
                points[0] = new float3(0, 0, 0);

                NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
                kDTree.AddPoints(points);

                bool success = kDTree.count == 1;
                kDTree.Dispose();

                return success;
            }
            catch (Exception e)
            {
                Log($"Test failed with: {e} Stack trace: {Environment.StackTrace}");

                return false;
            }
        }

        [RunTest(true, null, 1000)]
        public bool AddTest_Add100Items()
        {
            NativeArray<float3> points = new NativeArray<float3>(100, Allocator.TempJob);
            for (int i = 0; i < 100; i++)
            {
                points[i] = new float3(i, i, i);
            }

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            bool success = kDTree.count == 100;
            kDTree.Dispose();

            return success;
        }

        [RunTest(false, null, 1000)]
        public bool AddTest_Add1000Items()
        {
            NativeArray<float3> points = new NativeArray<float3>(1000, Allocator.TempJob);
            for (int i = 0; i < 1000; i++)
            {
                points[i] = new float3(i, i, i);
            }

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            bool success = kDTree.count == 1000;
            kDTree.Dispose();

            return success;
        }

        [RunTest(false)]
        public bool AddTest_TooManyItems()
        {
            NativeArray<float3> points = new NativeArray<float3>(100 * 100 * 100, Allocator.TempJob);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < 100; k++)
                    {
                        points[i * 100 * 100 + j * 100 + k] = new float3(i, j, k);
                    }
                }
            }

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            bool success = kDTree.count == 100 * 100 * 100;
            kDTree.Dispose();
            points.Dispose();
        
            return success;
        }

        [RunTest(false)]
        public bool QueryRadiusTest_EmptyTree()
        {
            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            NativeList<int> result = new NativeList<int>(Allocator.TempJob);

            kDTree.QueryRadius(new float3(0, 0, 0), 1f, result);

            
            bool success = result.Length == 0;
            result.Dispose();
            kDTree.Dispose();
        
            return success;
        }

        [RunTest(false)]
        public bool QueryRadiusTest_OneItem()
        {
            NativeArray<float3> points = new NativeArray<float3>(1, Allocator.TempJob);
            points[0] = new float3(0, 0, 0);

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            NativeList<int> result = new NativeList<int>(Allocator.TempJob);
            kDTree.QueryRadius(new float3(0, 0, 0), 1f, result);

            bool success = result.Length == 1;
            result.Dispose();
            kDTree.Dispose();
            points.Dispose();
        
            return success;
        }

        [RunTest(false)]
        public bool QueryRadiusTest_TwoItems()
        {
            NativeArray<float3> points = new NativeArray<float3>(2, Allocator.TempJob);
            points[0] = new float3(0, 0, 0);
            points[1] = new float3(1, 1, 1);

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            NativeList<int> result = new NativeList<int>(Allocator.TempJob);
            kDTree.QueryRadius(new float3(0, 0, 0), 1f, result);

            bool success = result.Length == 1;
            result.Dispose();
            kDTree.Dispose();
            points.Dispose();
        
            return success;
        }

        [RunTest(false)]
        public bool QueryRadiusTest_100Items()
        {
            NativeArray<float3> points = new NativeArray<float3>(100, Allocator.TempJob);
            for (int i = 0; i < 100; i++)
            {
                points[i] = new float3(i, i, i);
            }

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            NativeList<int> result = new NativeList<int>(Allocator.TempJob);
            kDTree.QueryRadius(new float3(0, 0, 0), 1f, result);

            bool success = result.Length == 1;
            result.Dispose();
            kDTree.Dispose();
            points.Dispose();
        
            return success;
        }

        [RunTest(false)]
        public bool QueryRadiusTest_CorrectFromMatrix()
        {
            NativeArray<float3> points = new NativeArray<float3>(100 * 100 * 100, Allocator.TempJob);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < 100; k++)
                    {
                        points[i * 100 * 100 + j * 100 + k] = new float3(i, j, k);
                    }
                }
            }

            NativeKDTree kDTree = new NativeKDTree(Allocator.TempJob);
            kDTree.AddPoints(points);

            NativeList<int> result = new NativeList<int>(Allocator.TempJob);
            kDTree.QueryRadius(new float3(0, 0, 0), 1f, result);

            bool success = result.Length == 1;
            result.Dispose();
            kDTree.Dispose();
            points.Dispose();
        
            return success;
        }
    }
}