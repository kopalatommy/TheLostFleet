using System.Collections.Generic;
using ProjectWorlds.DataStructures.Trees;
using ProjectWorlds.Testing;

namespace ProjectWorlds.UnitTests
{
    public class TreeTester<T> : TesterBase where T : ITree<int>, new()
    {
        public TreeTester(string testerName, string resultsDir = null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(TreeTester<T>); } }

        #region Count Tests

        [RunTest(true)]
        public bool TestCount_EmptyTree()
        {
            T tree = new T();
            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestCount_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool TestCount_TwoElements()
        {
            T tree = new T();
            tree.Add(1);
            tree.Add(2);
            return tree.Count == 2;
        }

        #endregion // Count Tests

        #region IsEmpty Tests

        [RunTest(true)]
        public bool TestIsEmpty_EmptyTree()
        {
            T tree = new T();
            return tree.IsEmpty;
        }

        [RunTest(true)]
        public bool TestIsEmpty_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            return !tree.IsEmpty;
        }

        #endregion // IsEmpty Tests

        #region MinValue Tests

        [RunTest(true)]
        public bool TestMinValue_EmptyTree()
        {
            T tree = new T();
            return tree.MinValue == default(int);
        }

        [RunTest(true)]
        public bool TestMinValue_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            return tree.MinValue == 1;
        }

        [RunTest(true)]

        public bool TestMinValue_TwoElements()
        {
            T tree = new T();
            tree.Add(2);
            tree.Add(1);
            return tree.MinValue == 1;
        }

        [RunTest(true)]
        public bool TestMinValue_100Elements()
        {
            T tree = new T();
            
            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            return tree.MinValue == 0;
        }

        #endregion // MinValue Tests

        #region MaxValue Tests

        [RunTest(true)]
        public bool TestMaxValue_EmptyTree()
        {
            T tree = new T();
            return tree.MaxValue == default(int);
        }

        [RunTest(true)]
        public bool TestMaxValue_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            return tree.MaxValue == 1;
        }

        [RunTest(true)]
        public bool TestMaxValue_TwoElements()
        {
            T tree = new T();
            tree.Add(1);
            tree.Add(2);
            return tree.MaxValue == 2;
        }

        [RunTest(true)]
        public bool TestMaxValue_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            return tree.MaxValue == 99;
        }

        #endregion // MaxValue Tests

        #region Add Tests

        [RunTest(true)]
        public bool TestAdd_EmptyTree()
        {
            T tree = new T();
            tree.Add(1);
            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool TestAdd_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            tree.Add(2);
            return tree.Count == 2;
        }

        [RunTest(true)]
        public bool TestAdd_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            return tree.Count == 100;
        }

        [RunTest(true)]
        public bool TestAdd_Contains()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);

                for (int j = 0; j <= i; j++)
                {
                    if (!tree.Contains(j))
                    {
                        return false;
                    }
                }
            }

            return tree.Count == 100;
        }

        #endregion // Add Tests

        #region Clear Tests

        [RunTest(true)]
        public bool TestClear_EmptyTree()
        {
            T tree = new T();
            tree.Clear();
            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestClear_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            tree.Clear();
            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestClear_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            tree.Clear();
            return tree.Count == 0;
        }

        #endregion // Clear Tests

        #region Contains Tests

        [RunTest(true)]
        public bool TestContains_EmptyTree()
        {
            T tree = new T();
            return !tree.Contains(1);
        }

        [RunTest(true)]
        public bool TestContains_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            return tree.Contains(1);
        }

        [RunTest(true)]
        public bool TestContains_OneElementNot()
        {
            T tree = new T();
            tree.Add(1);
            return !tree.Contains(2);
        }

        [RunTest(true)]
        public bool TestContains_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            return tree.Contains(99);
        }

        [RunTest(true)]
        public bool TestContains_100ElementsNot()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            return !tree.Contains(100);
        }

        #endregion // Contains Tests

        #region Remove Tests

        [RunTest(true)]
        public bool TestRemove_EmptyTree()
        {
            T tree = new T();
            tree.Remove(1);
            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestRemove_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            tree.Remove(1);

            if (tree.Count != 0)
            {
                Log("Count is not 0: " + tree.Count + " - " + tree);
                return false;
            }

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestRemove_OneElementNot()
        {
            T tree = new T();
            tree.Add(1);
            tree.Remove(2);
            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool TestRemove_TwoElements()
        {
            T tree = new T();
            tree.Add(1);
            tree.Add(2);
            tree.Remove(2);

            if (tree.Count != 1) {
                Log("Count is not 1: " + tree.Count + " - " + tree);
                return false;
            }

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool TestRemove_TwoElementsNot()
        {
            T tree = new T();
            tree.Add(1);
            tree.Add(2);
            tree.Remove(3);
            return tree.Count == 2;
        }

        [RunTest(true)]
        public bool TestRemove_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            tree.Remove(99);
            return tree.Count == 99 && !tree.Contains(99);
        }

        [RunTest(true)]
        public bool TestRemove_100ElementsNot()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            tree.Remove(100);
            return tree.Count == 100;
        }

        [RunTest(true)]
        public bool TestRemove_AddRemoveAdd()
        {
            T tree = new T();
            tree.Add(1);
            tree.Remove(1);
            tree.Add(1);
            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool TestRemove_AddRemoveAddRemove()
        {
            T tree = new T();
            tree.Add(1);
            tree.Remove(1);
            tree.Add(1);
            tree.Remove(1);
            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool TestRemove_AddClearAddRemoveAdd()
        {
            T tree = new T();
            tree.Add(1);
            tree.Clear();
            tree.Remove(1);
            tree.Add(1);
            return tree.Count == 1;
        }

        #endregion // Remove Tests

        #region ToCollection Tests

        [RunTest(true)]
        public bool TestToCollection_EmptyTree()
        {
            T tree = new T();
            System.Collections.Generic.ICollection<int> collection = new System.Collections.Generic.List<int>();
            tree.ToCollection(ref collection);
            return collection.Count == 0;
        }

        [RunTest(true)]
        public bool TestToCollection_OneElement()
        {
            T tree = new T();
            tree.Add(1);
            System.Collections.Generic.ICollection<int> collection = new System.Collections.Generic.List<int>();
            tree.ToCollection(ref collection);
            return collection.Count == 1 && collection.Contains(1);
        }

        [RunTest(true)]
        public bool TestToCollection_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            System.Collections.Generic.ICollection<int> collection = new ProjectWorlds.DataStructures.Lists.ArrayList<int>(100);
            tree.ToCollection(ref collection);

            if (collection.Count != 100)
            {
                Log("Collection count is not 100: " + collection.Count + " - " + collection);
                return false;
            }

            return collection.Count == 100;
        }

        #endregion // ToCollection Tests

        #region GetEnumerator Tests

        [RunTest(true)]
        public bool TestGetEnumerator_EmptyTree()
        {
            T tree = new T();
            System.Collections.Generic.IEnumerator<int> enumerator = tree.GetEnumerator();
            return !enumerator.MoveNext();
        }

        [RunTest(true)]
        public bool TestGetEnumerator_OneElement()
        {
            T tree = new T();
            tree.Add(1);

            System.Collections.Generic.IEnumerator<int> enumerator = tree.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                Log("TestGetEnumerator_OneElement failed: MoveNext returned false: " + tree);
                return false;
            }

            if (enumerator.Current != 1)
            {
                Log("TestGetEnumerator_OneElement failed: Current is not 1");
                return false;
            }

            return !enumerator.MoveNext();
        }

        [RunTest(true)]
        public bool TestGetEnumerator_100Elements()
        {
            T tree = new T();

            for (int i = 0; i < 100; i++)
            {
                tree.Add(i);
            }

            System.Collections.Generic.IEnumerator<int> enumerator = tree.GetEnumerator();

            for (int i = 0; i < 100; i++)
            {
                enumerator.MoveNext();
                if (enumerator.Current != i)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion // GetEnumerator Tests
    }
}
