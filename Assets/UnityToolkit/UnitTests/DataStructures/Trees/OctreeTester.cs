using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using ProjectWorlds.Testing;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class OctreeTester : TesterBase
    {
        public OctreeTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(OctreeTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_AddToEmptyInRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_AddToEmptyOutOfRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 15, 15, 15 }, 15);

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmptyInRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Add(new double[] { 6, 6, 6 }, 6);

            if (tree.Count != 2)
            {
                Log("Count is not 2: " + tree.Count);
                return false;
            }

            return tree.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmptyOutOfRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Add(new double[] { 15, 15, 15 }, 15);

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            return tree.Count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_Duplicates()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Add(new double[] { 5, 5, 5 }, 5);

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_MultipleDuplicates()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 1000, 1000, 1000 });

            for (int i = 0; i < 10; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            for (int i = 0; i < 10; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            return tree.Count == 10;
        }

        [RunTest(false, new object[0], 1000000)]
        public bool AddTest_1000Duplicates()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
                tree.Add(new double[] { i, i, i }, i);
            }

            return tree.Count == 1000;
        }

        #endregion // Add Tests

        #region Contains Tests
        
        [RunTest(true)]
        public bool ContainsTest_EmptyTree()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] {10, 10, 10});

            return !tree.Contains(new double[] { 5, 5, 5 });
        }

        [RunTest(true)]
        public bool ContainsTest_ItemInRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);

            return tree.Contains(new double[] { 5, 5, 5 });
        }

        [RunTest(true)]
        public bool ContainsTest_ItemOutOfRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);

            return !tree.Contains(new double[] { 15, 15, 15 });
        }

        [RunTest(true)]
        public bool ContainsTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] {  0, 0, 0 }, new double[] {  1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (!tree.Contains(new double[] { i, i, i }))
                {
                    Log("Tree does not contain " + i);
                    return false;
                }
            }

            return true;
        }

        #endregion // Contains Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_EmptyTree()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Remove(new double[] { 5, 5, 5 });

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_ItemInRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Remove(new double[] { 5, 5, 5 });

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_ItemOutOfRange()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Remove(new double[] { 15, 15, 15 });

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] {  0, 0, 0 }, new double[] {  1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                tree.Remove(new double[] { i, i, i });
            }

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_Duplicates()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Add(new double[] { 5, 5, 5 }, 5);
            tree.Remove(new double[] { 5, 5, 5 });

            return tree.Count == 0;
        }

        #endregion // Remove Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_EmptyTree()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            tree.Clear();

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] {  0, 0, 0 }, new double[] {  1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            tree.Clear();

            return tree.Count == 0;
        }

        #endregion // Clear Tests

        #region ToCollection Tests

        [RunTest(true)]
        public bool ToCollectionTest_EmptyTree()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });
            List<int> collection = new List<int>();

            tree.ToCollection(collection);

            return collection.Count == 0;
        }

        [RunTest(true)]
        public bool ToCollectionTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] {  0, 0, 0 }, new double[] {  1000, 1000, 1000 });
            List<int> collection = new List<int>();

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            tree.ToCollection(collection);

            return collection.Count == 1000;
        }

        #endregion // ToCollection Tests

        #region Enumerator Tests

        [RunTest(true)]
        public bool EnumeratorTest_EmptyTree()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 10, 10, 10 });

            foreach (int item in tree)
            {
                Log("Tree is not empty");
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000Items()
        {
            Octree<int> tree = new Octree<int>(3, new double[] { 0, 0, 0 }, new double[] { 1000, 1000, 1000 });

            for (int i = 0; i < 1000; i++)
            {
                tree.Add(new double[] { i, i, i }, i);
            }

            int count = 0;
            foreach (int item in tree)
            {
                count++;
            }

            return count == 1000;
        }

        #endregion // Enumerator Tests
    }
}