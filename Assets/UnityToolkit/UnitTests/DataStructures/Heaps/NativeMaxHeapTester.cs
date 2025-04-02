using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Unmanaged;
using ProjectWorlds.Testing;
using Unity.Collections;
using Unity.VisualScripting;

namespace ProjectWorlds.UnitTests
{
    public class NativeMaxHeapTester : TesterBase
    {
        public NativeMaxHeapTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(NativeMaxHeapTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_AddToEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            int count = heap.Count;
            heap.Dispose();

            return count == 1;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            if (heap.Count != 2)
            {
                Log("Count is not 2: " + heap.Count);
                heap.Dispose();
                return false;
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_AddToNonEmpty2()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(4, 4);

            if (heap.Count != 2)
            {
                Log("Count is not 2: " + heap.Count);
                heap.Dispose();
                return false;
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 1000;
        }

        [RunTest(true)]
        public bool AddTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
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
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(5, 5);

            int count = heap.Count;
            heap.Dispose();

            return count == 2;
        }

        [RunTest(true)]
        public bool AddTest_CannotGrow_CompletelyFill()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, false, 10);

            for (int i = 0; i < 10; i++)
            {
                heap.Add(i, i);
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 10;
        }

        [RunTest(true)]
        public bool AddTest_CannotGrow_WillThrowAwaySmallest()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, false, 10);

            for (int i = 0; i < 11; i++)
            {
                heap.Add(i, i);
            }

            int expected = 9;
            for (int i = 0; i < 10; i++)
            {
                (int key, int value) = heap.PopMax();
                if (key != expected)
                {
                    Log("Did not find the expected value. Expected: " + expected + " Found: " + key);
                    heap.Dispose();
                    return false;
                }
                expected--;
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool AddTest_CannotGrow_WillThrowAwaySmallest_Reverse()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, false, 10);

            for (int i = 10; i >= 0; i--)
            {
                heap.Add(i, i);
            }

            int expected = 9;
            for (int i = 0; i < 10; i++)
            {
                (int key, int value) = heap.PopMax();
                if (key != expected)
                {
                    Log("Did not find the expected value. Expected: " + expected + " Found: " + key);
                    heap.Dispose();
                    return false;
                }
                expected--;
            }

            heap.Dispose();
            return true;
        }

        #endregion // Add Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_RemoveFromEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                heap.PopMax();
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
        public bool RemoveTest_RemoveFromNonEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int item) = heap.PopMax();
            int count = heap.Count;
            heap.Dispose();

            return item == 6 && count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_RemoveFromNonEmpty2()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int item) = heap.PopMax();
            int count = heap.Count;
            heap.Dispose();

            return item == 6 && count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.PopMax().Item2 != i)
                {
                    Log("Remove is not " + i);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            for (int i = 1000; i > 0; i--)
            {
                if (heap.PopMax().Item2 != i)
                {
                    Log("Remove is not " + i);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();

            return count == 0;
        }

        #endregion

        #region Peek Tests

        [RunTest(true)]
        public bool PeekTest_PeekEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                heap.PeekMax();
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
        public bool PeekTest_PeekNonEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            (int key, int item) = heap.PeekMax();
            heap.Dispose();

            return item == 6;
        }

        [RunTest(true)]
        public bool PeekTest_PeekNonEmpty2()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(6, 6);
            heap.Add(5, 5);

            (int key, int item) = heap.PeekMax();
            heap.Dispose();

            return item == 6;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);

                if (heap.PeekMax().Item2 != i)
                {
                    Log("Peek is not " + i + ": " + heap.PeekMax().Item2);
                    heap.Dispose();
                    return false;
                }
            }

            (int key, int item) = heap.PeekMax();
            heap.Dispose();

            return item == 999;
        }

        [RunTest(true)]
        public bool PeekTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);

                if (heap.PeekMax().Item2 != 1000)
                {
                    Log("Peek is not 1000: " + heap.PeekMax().Item2);
                    heap.Dispose();
                    return false;
                }
            }

            (int key, int item) = heap.PeekMax();
            heap.Dispose();

            return item == 1000;
        }

        #endregion // Peek Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_ClearEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Clear();

            int count = heap.Count;
            heap.Dispose();
            return count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_ClearNonEmpty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            heap.Clear();

            int count = heap.Count;
            heap.Dispose();
            return count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1000Items()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

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
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            foreach ((int key, int item) in heap)
            {
                Log("There should be no items in the heap.");
                heap.Dispose();
                return false;
            }

            heap.Dispose();
            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            int count = 999;
            foreach ((int key, int item) in heap)
            {
                if (item != count)
                {
                    Log("Item is not " + count + ": " + item);
                    heap.Dispose();
                    return false;
                }

                count--;
            }

            int finalCount = heap.Count;
            heap.Dispose();
            return finalCount == 1000;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            int count = 1000;
            foreach ((int key, int item) in heap)
            {
                if (item != count)
                {
                    Log("Item is not " + count + ": " + item);
                    heap.Dispose();
                    return false;
                }

                count--;
            }

            heap.Dispose();
            return count == 0;
        }

        #endregion // Enumerator Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_Empty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            bool contains = heap.Contains(5);
            heap.Dispose();
            
            return !contains;
        }

        [RunTest(true)]
        public bool ContainsTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            bool contains = heap.Contains(5);
            heap.Dispose();

            return contains;
        }

        [RunTest(true)]
        public bool ContainsTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            bool contains = heap.Contains(5);
            heap.Dispose();

            return contains;
        }

        [RunTest(true)]
        public bool ContainsTest_NotInHeap()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);
            heap.Add(6, 6);

            bool contains = heap.Contains(7);
            heap.Dispose();
            return !contains;
        }

        [RunTest(true)]
        public bool ContainsTest_1000ItemsInOrderAll()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 0; i < 1000; i++)
            {
                if (!heap.Contains(i))
                {
                    Log("Contains is not " + i);
                    heap.Dispose();
                    return false;
                }
            }
            heap.Dispose();

            return true;
        }

        #endregion // Contains Tests

        #region Min Value Tests

        [RunTest(true)]
        public bool MinValueTest_Empty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                heap.MinItem();
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
        public bool MinValueTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            (int key, int item) = heap.MinItem();
            heap.Dispose();
            return item == 0;
        }

        [RunTest(true)]
        public bool MinValueTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            (int key, int item) = heap.MinItem();
            heap.Dispose();
            return item == 1;
        }

        #endregion // Min Value Tests

        #region Max Value Tests

        [RunTest(true)]
        public bool MaxValueTest_Empty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                heap.PopMax();
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
        public bool MaxValueTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            (int key, int item) = heap.PopMax();
            heap.Dispose();

            return item == 999;
        }

        [RunTest(true)]
        public bool MaxValueTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            (int key, int item) = heap.PopMax();
            heap.Dispose();
            return item == 1000;
        }

        #endregion // Max Value Tests
        
        #region Take Max Tests

        [RunTest(true)]
        public bool TakeMaxTest_Empty()
        {
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            try
            {
                heap.PopMax();
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
            NativeMaxHeap<int,int> heap = new NativeMaxHeap<int,int>(Allocator.Persistent, true, 1000);

            heap.Add(5, 5);

            (int key, int item) = heap.PopMax();
            int count = heap.Count;
            heap.Dispose();

            return item == 5 && count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 0; i < 1000; i++)
            {
                heap.Add(i, i);
            }

            for (int i = 999; i >= 0; i--)
            {
                if (heap.PopMax().Item2 != i)
                {
                    Log("TakeMax is not " + i);
                    heap.Dispose();
                    return false;
                }
            }

            int count = heap.Count;
            heap.Dispose();
            return count == 0;
        }

        [RunTest(true)]
        public bool TakeMaxTest_1000ItemsInReverseOrder()
        {
            NativeMaxHeap<int, int> heap = new NativeMaxHeap<int, int>(Allocator.Persistent, true, 1000);

            for (int i = 1000; i > 0; i--)
            {
                heap.Add(i, i);
            }

            for (int i = 1000; i > 0; i--)
            {
                if (heap.PopMax().Item2 != i)
                {
                    Log("TakeMax is not " + i);
                    heap.Dispose();
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