using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Heaps;
using ProjectWorlds.Testing;

namespace ProjectWorlds.UnitTests
{
    public class MinHeapTester : TesterBase
    {
        public MinHeapTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(MinHeapTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_AddToEmpty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty()
        {
            MinHeap<int> heap = new MinHeap<int>();

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
            MinHeap<int> heap = new MinHeap<int>();

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
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.Count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            return heap.Count == 1000;
        }

        #endregion // Add Tests

        #region Peek Tests

        [RunTest(true)]
        public bool PeekTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.Peek();
                Log("No exception thrown for empty heap");
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool PeekTest_OneItem()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.Peek() == 5;
        }

        [RunTest(true)]
        public bool PeekTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return heap.Peek() == 5;
        }

        [RunTest(true)]
        public bool PeekTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            return heap.Peek() == 5;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
                if (heap.Peek() != 0)
                {
                    Log("Peek is not 0: " + heap.Peek());
                    return false;
                }
            }

            return heap.Peek() == 0;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
                if (heap.Peek() != i)
                {
                    Log("Peek is not " + i + ": " + heap.Peek());
                    return false;
                }
            }

            return heap.Peek() == 0;
        }

        #endregion // Peek Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.Remove();
                Log("No exception thrown for empty heap");
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveTest_OneItem()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.Remove() == 5 && heap.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            if (heap.Remove() != 5)
            {
                Log("First remove is not 5: " + heap.Remove());
                return false;
            }

            return heap.Remove() == 6;
        }

        [RunTest(true)]
        public bool RemoveTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            if (heap.Remove() != 5)
            {
                Log("First remove is not 5: " + heap.Remove());
                return false;
            }

            return heap.Remove() == 6;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (heap.Remove() != i)
                {
                    Log("Remove is not " + i + ": " + heap.Remove());
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (heap.Remove() != i)
                {
                    Log("Remove is not " + i + ": " + heap.Remove());
                    return false;
                }
            }

            return true;
        }

        #endregion // Remove Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Clear();

            return heap.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1000Items()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

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
            MinHeap<int> heap = new MinHeap<int>();

            foreach (int item in heap)
            {
                Log("Found item in empty heap: " + item);
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_OneItem()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            foreach (int item in heap)
            {
                if (item != 5)
                {
                    Log("Item is not 5: " + item);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            int[] items = new int[2];
            int i = 0;
            foreach (int item in heap)
            {
                items[i] = item;
                i++;
            }

            if (items[0] != 5)
            {
                Log("First item is not 5: " + items[0]);
                return false;
            }

            if (items[1] != 6)
            {
                Log("Second item is not 6: " + items[1]);
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            int[] items = new int[2];
            int i = 0;
            foreach (int item in heap)
            {
                items[i] = item;
                i++;
            }

            if (items[0] != 5)
            {
                Log("First item is not 5: " + items[0]);
                return false;
            }

            if (items[1] != 6)
            {
                Log("Second item is not 6: " + items[1]);
                return false;
            }

            return true;
        }

        #endregion // Enumerator Tests

        #region Min Value Tests

        [RunTest(true)]
        public bool MinValueTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.MinValue();
                Log("No exception thrown for empty heap");
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool MinValueTest_OneItem()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.MinValue() == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return heap.MinValue() == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            return heap.MinValue() == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.MinValue() == 0;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            return heap.MinValue() == 0;
        }

        #endregion // Min Value Tests

        #region Max Value Tests

        [RunTest(true)]
        public bool MaxValueTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.MaxValue();
                Log("No exception thrown for empty heap");
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool MaxValueTest_OneItem()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.MaxValue() == 5;
        }

        [RunTest(true)]
        public bool MaxValueTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            return heap.MaxValue() == 6;
        }

        [RunTest(true)]
        public bool MaxValueTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            return heap.MaxValue() == 6;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            return heap.MaxValue() == 999;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            return heap.MaxValue() == 999;
        }

        #endregion // Max Value Tests

        #region Take Min Tests

        [RunTest(true)]
        public bool TakeMinTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.TakeMin();
                Log("No exception thrown for empty heap");
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
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.TakeMin() == 5 && heap.Count == 0;
        }

        [RunTest(true)]
        public bool TakeMinTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            if (heap.TakeMin() != 5)
            {
                Log("First take min is not 5: " + heap.TakeMin());
                return false;
            }

            return heap.TakeMin() == 6;
        }

        [RunTest(true)]
        public bool TakeMinTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            if (heap.TakeMin() != 5)
            {
                Log("First take min is not 5: " + heap.TakeMin());
                return false;
            }

            return heap.TakeMin() == 6;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (heap.TakeMin() != i)
                {
                    Log("Take min is not " + i + ": " + heap.TakeMin());
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (heap.TakeMin() != i)
                {
                    Log("Take min is not " + i + ": " + heap.TakeMin());
                    return false;
                }
            }

            return true;
        }

        #endregion // Take Min Tests

        #region Take Max Tests

        [RunTest(true)]
        public bool TakeMaxTest_Empty()
        {
            MinHeap<int> heap = new MinHeap<int>();

            try
            {
                int value = heap.TakeMax();
                Log("No exception thrown for empty heap");
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
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);

            return heap.TakeMax() == 5;
        }

        [RunTest(true)]
        public bool TakeMaxTest_TwoItems()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(6);

            if (heap.TakeMax() != 6)
            {
                Log("First take max is not 6: " + heap.TakeMax());
                return false;
            }

            return heap.TakeMax() == 5;
        }

        [RunTest(true)]
        public bool TakeMaxTest_TwoItems2()
        {
            MinHeap<int> heap = new MinHeap<int>();

            heap.Add(6);
            heap.Add(5);

            if (heap.TakeMax() != 6)
            {
                Log("First take max is not 6: " + heap.TakeMax());
                return false;
            }

            return heap.TakeMax() == 5;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInOrder()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.TakeMax() != i)
                {
                    Log("Take max is not " + i + ": " + heap.TakeMax());
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsReverse()
        {
            MinHeap<int> heap = new MinHeap<int>(1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.TakeMax() != i)
                {
                    Log("Take max is not " + i + ": " + heap.TakeMax());
                    return false;
                }
            }

            return true;
        }

        #endregion // Take Max Tests
    }
}