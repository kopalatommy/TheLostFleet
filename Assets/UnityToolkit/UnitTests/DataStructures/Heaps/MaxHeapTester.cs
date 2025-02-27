using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Heaps;
using ProjectWorlds.Testing;

namespace ProjectWorlds.UnitTests
{
    public class MaxHeapTester : TesterBase
    {
        public MaxHeapTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(MaxHeapTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_AddToEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);

            return heap.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(6);

            if (heap.Count != 2)
            {
                Log("Count is not 2: " + heap.Count);
                return false;
            }

            return heap.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty2()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(4);

            if (heap.Count != 2)
            {
                Log("Count is not 2: " + heap.Count);
                return false;
            }

            return heap.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.Count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            return heap.Count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_Duplicates()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(5);

            return heap.Count == 2;
        }

        #endregion // Add Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_RemoveFromEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.Remove();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveTest_RemoveFromNonEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return heap.Remove() == 6 && heap.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_RemoveFromNonEmpty2()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(6);
            heap.Add(5);

            return heap.Remove() == 6 && heap.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.Remove() != i)
                {
                    Log("Remove is not " + i);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 1000; i > 0; i--)
            {
                if (heap.Remove() != i)
                {
                    Log("Remove is not " + i);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        #endregion

        #region Peek Tests

        [RunTest(true)]
        public bool PeekTest_PeekEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.Peek();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool PeekTest_PeekNonEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return heap.Peek() == 6;
        }

        [RunTest(true)]
        public bool PeekTest_PeekNonEmpty2()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(6);
            heap.Add(5);

            return heap.Peek() == 6;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);

                if (heap.Peek() != i)
                {
                    Log("Peek is not " + i + ": " + heap.Peek());
                    return false;
                }
            }

            return heap.Peek() == 999;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);

                if (heap.Peek() != 1000)
                {
                    Log("Peek is not 1000: " + heap.Peek());
                    return false;
                }
            }

            return heap.Peek() == 1000;
        }

        #endregion // Peek Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_ClearEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Clear();

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_ClearNonEmpty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(6);

            heap.Clear();

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1000Items()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            heap.Clear();

            return heap.Count == 0;
        }

        #endregion // Clear Tests

        #region Enumerator Tests

        [RunTest(true)]
        public bool EnumeratorTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            foreach (int item in heap)
            {
                Log("There should be no items in the heap.");
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            int count = 999;
            foreach (int item in heap)
            {
                if (item != count)
                {
                    Log("Item is not " + count + ": " + item);
                    return false;
                }

                count--;
            }

            return heap.Count == 1000;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            int count = 1000;
            foreach (int item in heap)
            {
                if (item != count)
                {
                    Log("Item is not " + count + ": " + item);
                    return false;
                }

                count--;
            }

            return count == 0;
        }

        #endregion // Enumerator Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            return !heap.Contains(5);
        }

        [RunTest(true)]
        public bool ContainsTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.Contains(5);
        }

        [RunTest(true)]
        public bool ContainsTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            return heap.Contains(5);
        }

        [RunTest(true)]
        public bool ContainsTest_NotInHeap()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return !heap.Contains(7);
        }

        #endregion // Contains Tests

        #region Min Value Tests

        [RunTest(true)]
        public bool MinValueTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.MinValue();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.MinValue() == 0;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            return heap.MinValue() == 1;
        }

        #endregion // Min Value Tests

        #region Max Value Tests

        [RunTest(true)]
        public bool MaxValueTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.MaxValue();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.MaxValue() == 999;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            return heap.MaxValue() == 1000;
        }

        #endregion // Max Value Tests

        #region Take Min Tests

        [RunTest(true)]
        public bool TakeMinTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.TakeMin();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool TakeMinTest_OneItem()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);

            return heap.TakeMin() == 5 && heap.Count == 0;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (heap.TakeMin() != i)
                {
                    Log("TakeMin is not " + i);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 1; i <= 1000; i++)
            {
                int t = heap.TakeMin();
                if (t != i)
                {
                    Log("TakeMin is not " + i + ": " + t + " - " + heap);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        #endregion // Take Min Tests

        #region Take Max Tests

        [RunTest(true)]
        public bool TakeMaxTest_Empty()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            try
            {
                heap.TakeMax();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool TakeMaxTest_OneItem()
        {
            MaxHeap<int> heap = new MaxHeap<int>();

            heap.Add(5);

            return heap.TakeMax() == 5 && heap.Count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.TakeMax() != i)
                {
                    Log("TakeMax is not " + i);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInReverseOrder()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 1000; i > 0; i--)
            {
                if (heap.TakeMax() != i)
                {
                    Log("TakeMax is not " + i);
                    return false;
                }
            }

            return heap.Count == 0;
        }

        #endregion // Take Max Tests
    }
}