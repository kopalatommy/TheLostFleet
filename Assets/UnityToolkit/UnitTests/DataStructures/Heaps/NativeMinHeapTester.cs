using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Heaps;
using ProjectWorlds.Testing;
using GalacticBoundStudios.DataScribes.Unmanaged;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class NativeMinHeapTester : TesterBase
    {
        public NativeMinHeapTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(NativeMinHeapTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_AddToEmpty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            int count = heap.Count;
            heap.Dispose();
            
            return count == 1;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            if (heap.Count != 2)
            {
                heap.Dispose();
                Log("Count is not 2: " + heap.Count);
                return false;
            }

            int count = heap.Count;
            heap.Dispose();
            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(4, 4);

            if (heap.Count != 2)
            {
                heap.Dispose();
                Log("Count is not 2: " + heap.Count);
                return false;
            }

            int count = heap.Count;
            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            int count = heap.Count;
            heap.Dispose();
            return count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            int count = heap.Count;
            heap.Dispose();
            return count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_Duplicates()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(5, 5);

            int count = heap.Count;
            heap.Dispose();

            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_CannotGrow_CompletelyFill()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, false, 10);

            for (int i = 0; i < 10; i++)
            {
                heap.Add(i, i);
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 10;
        }

        [RunTest(true)]
        public bool AddTest_CannotGrow_WillThrowAwayLargest()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, false, 10);

            for (int i = 0; i < 11; i++)
            {
                heap.Add(i, i);
            }

            int expected = 1;
            for (int i = 0; i < 10; i++)
            {
                (int key, int value) = heap.PopMin();
                if (key != expected)
                {
                    Log("Did not find the expected value. Expected: " + expected + " Found: " + key);
                    heap.Dispose();
                    return false;
                }
                expected++;
            }

            heap.Dispose();
            return true;
        }

        #endregion // Add Tests

        #region Peek Tests

        [RunTest(true)]
        public bool PeekTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.PeekMin();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool PeekTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();
            return key == 5;
        }

        [RunTest(true)]
        public bool PeekTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();
            return key == 5;
        }

        [RunTest(true)]
        public bool PeekTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();
            return key == 5;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
                (int key, int value) = heap.PeekMin();
                if (key != 0)
                {
                    Log("Peek is not 0: " + key);
                    heap.Dispose();
                    return false;
                }
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
                (int key, int value) = heap.PeekMin();
                if (key != i)
                {
                    Log("Peek is not " + i + ": " + key);
                    heap.Dispose();
                    return false;
                }
            }

            heap.Dispose();
            return true;
        }

        #endregion // Peek Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.PopMin();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.PopMin();
            int count = heap.Count;
            heap.Dispose();

            return key == 5 && count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.PopMin();

            if (key != 5)
            {
                Log("First remove is not 5: " + key);
                heap.Dispose();
                return false;
            }

            (int key2, int value2) = heap.PopMin();
            int count = heap.Count;

            heap.Dispose();

            return key2 == 6 && count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.PopMin();

            if (key != 5)
            {
                Log("First remove is not 5: " + key);
                heap.Dispose();
                return false;
            }

            (int key2, int value2) = heap.PopMin();
            int count = heap.Count;

            heap.Dispose();

            return key2 == 6 && count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                (int key, int value) = heap.PopMin();
                if (key != i)
                {
                    Log("Remove is not " + i + ": " + key);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                (int key, int value) = heap.PopMin();
                if (key != i)
                {
                    Log("Remove is not " + i + ": " + key);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();
            return count == 0;
        }

        #endregion // Remove Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Clear();

            int count = 0;
            heap.Dispose();

            return count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1000Items()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            heap.Clear();

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        #endregion // Clear Tests

        #region Enumerator Tests

        [RunTest(true)]
        public bool EnumeratorTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            foreach (KeyValuePair<int, int> item in heap)
            {
                Log("Found item in empty heap: " + item);
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            foreach (KeyValuePair<int,int> item in heap)
            {
                if (item.Key != 5)
                {
                    Log("Item is not 5: " + item.Key);
                    heap.Dispose();
                    return false;
                }
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            int[] items = new int[2];
            int i = 0;
            foreach (KeyValuePair<int,int> item in heap)
            {
                items[i] = item.Key;
                i++;
            }

            if (items[0] != 5)
            {
                Log("First item is not 5: " + items[0]);
                heap.Dispose();
                return false;
            }

            if (items[1] != 6)
            {
                Log("Second item is not 6: " + items[1]);
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            int[] items = new int[2];
            int i = 0;
            foreach (KeyValuePair<int,int> item in heap)
            {
                items[i] = item.Key;
                i++;
            }

            if (items[0] != 5)
            {
                Log("First item is not 5: " + items[0]);
                heap.Dispose();
                return false;
            }

            if (items[1] != 6)
            {
                Log("Second item is not 6: " + items[1]);
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return true;
        }

        #endregion // Enumerator Tests

        #region Max Value Tests

        [RunTest(true)]
        public bool MinValueTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.PeekMin();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool MinValueTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();

            return key == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();

            return key == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.PeekMin();
            heap.Dispose();

            return key == 5;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            (int key, int value) = heap.PeekMin();
            heap.Dispose();

            return key == 0;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            (int key, int value) = heap.PeekMin();
            heap.Dispose();

            return key == 0;
        }

        #endregion // Min Value Tests

        #region Max Value Tests

        [RunTest(true)]
        public bool MaxValueTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.MaxItem();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool MaxValueTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.MaxItem();
            heap.Dispose();

            return key == 5;
        }

        [RunTest(true)]
        public bool MaxValueTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.MaxItem();
            heap.Dispose();

            return key == 6;
        }

        [RunTest(true)]
        public bool MaxValueTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.MaxItem();
            heap.Dispose();

            return key == 6;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            (int key, int value) = heap.MaxItem();
            heap.Dispose();

            return key == 999;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            (int key, int value) = heap.MaxItem();
            heap.Dispose();

            return key == 999;
        }

        #endregion // Max Value Tests

        #region Take Min Tests

        [RunTest(true)]
        public bool TakeMinTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.PopMin();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool TakeMinTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.PopMin();
            int count = heap.Count;
            heap.Dispose();

            return key == 5 && count == 0;
        }

        [RunTest(true)]
        public bool TakeMinTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.PopMin();
            (int key2, int value2) = heap.PopMin();

            if (key != 5)
            {
                Log("First take min is not 5: " + key);
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return key2 == 6;
        }

        [RunTest(true)]
        public bool TakeMinTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.PopMin();
            (int key2, int value2) = heap.PopMin();

            if (key != 5)
            {
                Log("First take min is not 5: " + heap.PopMin());
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return key2 == 6;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                (int key, int value) = heap.PopMin();
                if (key != i)
                {
                    Log("Take min is not " + i + ": " + key);
                    heap.Dispose();
                    return false;
                }
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool TakeMinTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                (int key, int value) = heap.PopMin();
                if (key != i)
                {
                    Log("Take min is not " + i + ": " + key);
                    heap.Dispose();
                    return false;
                }
            }

            heap.Dispose();
            return true;
        }

        #endregion // Take Min Tests

        #region Take Max Tests

        [RunTest(true)]
        public bool TakeMaxTest_Empty()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                (int key, int value) = heap.TakeMax();
                Log("No exception thrown for empty heap");
                heap.Dispose();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                heap.Dispose();
                return true;
            }
        }

        [RunTest(true)]
        public bool TakeMaxTest_OneItem()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int value) = heap.TakeMax();
            int count = heap.Count;
            heap.Dispose();

            return key == 5 && count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_TwoItems()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int value) = heap.TakeMax();

            if (key != 6)
            {
                Log("First take max is not 6: " + key);
                heap.Dispose();
                return false;
            }

            (int key2, int value2) = heap.TakeMax();
            int count = heap.Count;
            heap.Dispose();

            return key2 == 5 && count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_TwoItems2()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int value) = heap.TakeMax();

            if (key != 6)
            {
                Log("First take max is not 6: " + key);
                heap.Dispose();
                return false;
            }

            (int key2, int value2) = heap.TakeMax();
            int count = heap.Count;
            heap.Dispose();

            return key2 == 5 && count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInOrder()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 999; i >= 0; i--)
            {
                (int key, int value) = heap.TakeMax();
                if (key != i)
                {
                    Log("Take max is not " + i + ": " + value);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsReverse()
        {
            NativeMinHeap<int,int> heap = new NativeMinHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 999; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            for (int i = 999; i >= 0; i--)
            {
                (int key, int value) = heap.TakeMax();
                if (key != i)
                {
                    Log("Take max is not " + i + ": " + key);
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        #endregion // Take Max Tests
    }
}