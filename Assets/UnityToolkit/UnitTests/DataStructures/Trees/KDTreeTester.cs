using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Trees;
using ProjectWorlds.Testing;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class KDTreeTester : TesterBase
    {
        public KDTreeTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(KDTreeTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_NonEmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            return tree.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_WrongDimension()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            try
            {
                tree.Add(1, new double[] { 4, 5 });
            }
            catch (System.ArgumentException)
            {
                return true;
            }

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_NullPoint()
        {
            KDTree<int> tree = new KDTree<int>();
            
            try
            {
                tree.Add(0, null);
            }
            catch (System.ArgumentNullException)
            {
                return true;
            }

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool AddTest_1000Items()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            return tree.Count == 1000;
        }

        #endregion // Add Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();

            return !tree.ContainsPoint(new double[] { 1, 2, 3 });
        }

        [RunTest(true)]
        public bool ContainsTest_NonEmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return tree.ContainsPoint(new double[] { 1, 2, 3 });
        }

        [RunTest(true)]
        public bool ContainsTest_NonEmptyTree_False()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return !tree.ContainsPoint(new double[] { 4, 5, 6 });
        }

        [RunTest(true)]
        public bool ContainsTest_WrongDimension()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            try
            {
                tree.ContainsPoint(new double[] { 1, 2 });
            }
            catch (System.ArgumentException)
            {
                return true;
            }

            return false;
        }

        [RunTest(true)]
        public bool ContainsTest_FindsPoint()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            return tree.ContainsPoint(new double[] { 4, 5, 6 });
        }

        [RunTest(true)]
        public bool ContainsTest_FindsPoint2()
        {
            KDTree<int> tree = new KDTree<int>();
            
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { (double)i, (double)i, (double)i });
            }

            if (!tree.ContainsPoint(new double[] { 4.0, 4.0, 4.0 }))
            {
                foreach (KeyValuePair<double[], int> i in tree)
                {
                    if (i.Key[0] == 4.0 && i.Key[1] == 4.0 && i.Key[2] == 4.0)
                    {
                        Log("Contains point, but failed to find: " + i.Value);
                        return false;
                    }
                }
                Log("Failed to find point");
                return false;
            }

            return tree.ContainsPoint(new double[] { 4, 4, 4 });
        }

        [RunTest(true)]
        public bool ContainsTest_AsBuild()
        {
            KDTree<int> tree = new KDTree<int>();
            
            for (double i = 0; i < 1000.0; i += 1.0) {
                tree.Add((int)i, new double[] { i, i, i });
                if (!tree.ContainsPoint(new double[] { i, i, i }))
                {
                    Log("Failed to find point: " + i);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool ContainsTest_AsBuildInDepth()
        {
            KDTree<int> tree = new KDTree<int>();
            
            for (double i = 0; i < 1000.0; i += 1.0) {
                tree.Add((int)i, new double[] { i, i, i });

                for (double j = 0; j < i; j += 1.0) {
                    if (!tree.ContainsPoint(new double[] { j, j, j }))
                    {
                        Log("Failed to find point: " + j + " - " + i);
                        foreach (KeyValuePair<double[], int> k in tree)
                        {
                            Log("Point: " + k.Key[0] + " - " + k.Key[1] + " - " + k.Key[2]);
                        }
                        return false;
                    }
                }


                if (!tree.ContainsPoint(new double[] { i, i, i }))
                {
                    Log("Failed to find point: " + i);
                    return false;
                }
            }

            return true;
        }

        #endregion // Contains Tests

        #region Get Nearest Neighbors Tests

        [RunTest(true)]
        public bool GetNearestNeighborTests_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();

            return tree.GetNearestNeighbors(new double[] { 1, 2, 3 }, 1).IsEmpty;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_NonEmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return tree.GetNearestNeighbors(new double[] { 1, 2, 3 }, 1).Count == 1;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_MultipleItems()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            return tree.GetNearestNeighbors(new double[] { 4, 5, 6 }, 1).Count == 1;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_MultipleItems2()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            OrderedList<float, int> neighbors = tree.GetNearestNeighbors(new double[] { 4, 5, 6 }, 2);

            if (neighbors.Count != 2)
            {
                Log("Incorrect number of neighbors found: " + neighbors.Count);
                return false;
            }

            if (neighbors[0].Value != 1 || neighbors[1].Value != 0)
            {
                Log("Incorrect neighbor found: " + neighbors[0].Value);
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_CorrectItem()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            OrderedList<float, int> neighbors = tree.GetNearestNeighbors(new double[] { 4, 5, 6 }, 1);

            return neighbors[0].Value == 1;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_CorrectItem2()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1.0, 2.0, 3.0 });
            tree.Add(1, new double[] { 4.0, 5.0, 6.0 });

            OrderedList<float, int> neighbors = tree.GetNearestNeighbors(new double[] { 4.0, 5.0, 6.0 }, 6);

            if (neighbors.Count != 2)
            {
                Log("Incorrect number of neighbors found: " + neighbors.Count + " - " + neighbors);
                return false;
            }

            if (neighbors[0].Value != 1 || neighbors[1].Value != 0)
            {
                Log("Incorrect neighbors found: " + neighbors);
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_FindsAllInCorrectOrder()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });
            tree.Add(2, new double[] { 7, 8, 9 });

            OrderedList<float, int> neighbors = tree.GetNearestNeighbors(new double[] { 4, 5, 6 }, 3);

            if (neighbors.Count != 3)
            {
                Log("Incorrect number of neighbors found: " + neighbors.Count + " - " + neighbors);
                return false;
            }

            if (neighbors[0].Value != 1 || neighbors[1].Value != 0 || neighbors[2].Value != 2)
            {
                Log("Incorrect neighbors found: " + neighbors);
                return false;
            }

            return neighbors[0].Value == 1 && neighbors[1].Value == 0 && neighbors[2].Value == 2;
        }

        [RunTest(true)]
        public bool GetNearestNeighborsTest_1000Items()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            OrderedList<float, int> neighbors = tree.GetNearestNeighbors(new double[] { 4, 4, 4 }, 3);
            return neighbors[0].Value == 4 && neighbors[1].Value == 3 && neighbors[2].Value == 5;
        }

        #endregion // Get Nearest Neighbor Tests

        #region Radial Search Tests

        [RunTest(true)]
        public bool RadialSearchTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();

            return tree.RadialSearch(new double[] { 1, 2, 3 }, 1).IsEmpty;
        }

        [RunTest(true)]
        public bool RadialSearchTest_NonEmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return tree.RadialSearch(new double[] { 1, 2, 3 }, 1).Count == 1;
        }

        [RunTest(true)]
        public bool RadialSearchTest_MultipleItems()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            OrderedList<float, int> neighbors = tree.RadialSearch(new double[] { 4, 5, 6 }, 1);

            if (neighbors.Count != 1)
            {
                Log("Incorrect number of neighbors found: " + neighbors.Count + " - " + neighbors);
                return false;
            }

            return neighbors.Count == 1;
        }

        [RunTest(true)]
        public bool RadialSearchTest_MultipleItems2()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            return tree.RadialSearch(new double[] { 4, 5, 6 }, 2).Count == 1;
        }

        [RunTest(true)]
        public bool RadialSearchTest_MultipleItemsNoResults()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            return tree.RadialSearch(new double[] { 10, 10, 10 }, 0.5f).IsEmpty;
        }

        [RunTest(true)]
        public bool RadialSearchTest_CorrectItem()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });

            OrderedList<float, int> points = tree.RadialSearch(new double[] { 4, 5, 6 }, 0.5f);

            if (points.Count != 1)
            {
                Log("Incorrect number of points found: " + points.Count + " - " + points);
                return false;
            }

            return points[0].Value == 1;
        }

        [RunTest(true, new object[0], 5000)]
        public bool RadialSearchTest_1000Items()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });

                if (!tree.ContainsPoint(new double[] { i, i, i }))
                {
                    Log("Failed to find point: " + i);
                    return false;
                }

                OrderedList<float, int> test = tree.RadialSearch(new double[] { i, i, i }, 10000);
                if (test.Count != (i + 1))
                {
                    Log("Incorrect number of points found: " + test.Count + " - " + (i + 1));
                    return false;
                }
            }

            OrderedList<float, int> neighbors = tree.RadialSearch(new double[] { 4, 4, 4 }, 10000);

            if (neighbors.Count != 1000)
            {
                Log("Incorrect number of neighbors found: " + neighbors.Count + " - " + neighbors);
                return false;
            }

            return neighbors.Count == 1000;
        }

        #endregion // Radial Search Tests

        #region Enumerator Tests

        [RunTest(true)]
        public bool EnumeratorTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            foreach (KeyValuePair<double[], int> i in tree)
            {
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1Item()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            foreach (KeyValuePair<double[], int> i in tree)
            {
                return i.Value == 0;
            }

            return false;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000Items()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            int count = 0;
            foreach (KeyValuePair<double[], int> i in tree)
            {
                count++;
            }

            return count == 1000;
        }

        #endregion // Enumerator Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Clear();

            return tree.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_2Items()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });
            tree.Add(1, new double[] { 4, 5, 6 });
            tree.Clear();

            return tree.Count == 0;
        }

        #endregion // Clear Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_EmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();

            return !tree.Remove(new double[] { 1, 2, 3 }) && tree.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_NonEmptyTree()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return tree.Remove(new double[] { 1, 2, 3 }) && tree.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_NonEmptyTree_False()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            return !tree.Remove(new double[] { 4, 5, 6 }) && tree.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_WrongDimension()
        {
            KDTree<int> tree = new KDTree<int>();
            tree.Add(0, new double[] { 1, 2, 3 });

            try
            {
                tree.Remove(new double[] { 1, 2 });
            }
            catch (System.ArgumentException)
            {
                return true;
            }

            return tree.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_1000Items()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            return tree.Remove(new double[] { 4, 4, 4 }) && tree.Count == 999;
        }

        [RunTest(true)]
        public bool RemoveTest_1000Items_False()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            return !tree.Remove(new double[] { 1000, 1000, 1000 }) && tree.Count == 1000;
        }

        [RunTest(true)]
        public bool RemoveTest_1000Items_All()
        {
            KDTree<int> tree = new KDTree<int>();
            for (int i = 0; i < 1000; i++)
            {
                tree.Add(i, new double[] { i, i, i });
            }

            for (int i = 0; i < 1000; i++)
            {
                tree.Remove(new double[] { i, i, i });
            }

            return tree.Count == 0;
        }

        #endregion // Remove Tests
    }
}