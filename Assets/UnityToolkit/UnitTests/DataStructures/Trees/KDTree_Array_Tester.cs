using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using ProjectWorlds.Testing;
using System.Collections.Generic;

#if UNITY_STANDALONE
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
#else
using GameDevToolkitConsole.UnityToolkit.Utilities;
using Vector3 = GameDevToolkitConsole.UnityToolkit.Utilities.Vector3;
#endif

namespace ProjectWorlds.UnitTests
{
    public class KDTree_Array_Tester : TesterBase
    {
        public KDTree_Array_Tester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(KDTree_Array_Tester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_EmptyTree()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            tree.AddPoint(new Vector3(1, 2, 3));

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_NonEmptyTree()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            tree.AddPoint(new Vector3(1, 2, 3));
            tree.AddPoint(new Vector3(4, 5, 6));

            return tree.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_1000Items()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            for (int i = 0; i < 1000; i++)
            {
                tree.AddPoint(new Vector3(i, i, i));
            }

            return tree.Count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_Array_1000Items()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            Vector3[] points = new Vector3[1000];
            for (int i = 0; i < 1000; i++)
            {
                points[i] = new Vector3(i, i, i);
            }
            tree.AddPoints(points);

            return tree.Count == 1000;
        }

        [RunTest(true)]
        public bool SetTest_Array_1000Items_Empty()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            Vector3[] points = new Vector3[1000];
            for (int i = 0; i < 1000; i++)
            {
                points[i] = new Vector3(i, i, i);
            }
            tree.SetPoints(points);

            return tree.Count == 1000;
        }

        [RunTest(true)]
        public bool SetTest_Array_1000Items_Full()
        {
            KDTree_Arrays tree = new KDTree_Arrays();
            Vector3[] points = new Vector3[1000];
            for (int i = 0; i < 1000; i++)
            {
                points[i] = new Vector3(i, i, i);
            }
            tree.SetPoints(points);

            for (int i = 0; i < 1000; i++)
            {
                points[i] = new Vector3(i + 1000, i+ 1000, i + 1000);
            }
            tree.SetPoints(points);

            return tree.Count == 1000;
        }

        #endregion // Add Tests

        
    }
}