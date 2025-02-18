using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.Testing;
using System;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class OrderedListTester : ListTester<OrderedList<int>>
    {
        public OrderedListTester(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override Type TestType { get { Log("OrderedList Tester"); return typeof(OrderedListTester); } }

        [RunTest(true)]
        public bool InverseAddTest()
        {
            OrderedList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(99 - i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.Get(i) != i)
                {
                    Log("InverseAddTest failed. " + list.Get(i) + " != " + (i) + " - " + list);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool InsertTest_WrongOrder()
        {
            OrderedList<int> list = CreateList();

            int[] values = new int[] { 6, 8, 2, 1, 4, 3, 5, 7, 9, 0 };
            list.Add(10);

            for (int i = 0; i < values.Length; i++)
            {
                list.Insert(i, values[i]);
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (list.Get(i) != i)
                {
                    Log("InsertTest_WrongOrder failed. " + list.Get(i) + " != " + i + " - " + list);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool SetTest_Set1Item()
        {
            OrderedList<int> list = CreateList();
            list.Add(1);
            list.Set(0, 2);
            return list.Get(0) == 2;
        }

        [RunTest(true)]
        public override bool SetTest_Set2Items()
        {
            OrderedList<int> list = CreateList();
            list.Add(1);
            list.Add(2);
            list.Set(1, 3);
            return list.Get(1) == 3;
        }

        [RunTest(true)]
        public override bool SetTest_SetClearSetGet()
        {
            OrderedList<int> list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Set(0, 3);
            return list.Get(0) == 3;
        }

        [RunTest(true)]
        public override bool SetTest_Range()
        {
            OrderedList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.Set(i, i + 1);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.Get(i) != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class OrderedListTester_Comparable : TesterBase
    {
        public OrderedListTester_Comparable(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override Type TestType { get { Log("OrderedList Comparable Tester"); return typeof(OrderedListTester_Comparable); } }

        protected virtual OrderedList<int,int> CreateList()
        {
            return new OrderedList<int, int>();
        }

        #region Get Tests

        [RunTest(true)]
        public bool GetTest_Get1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Get(1) == 1;
        }

        [RunTest(true)]
        public bool GetTest_Get2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.GetAt(1).Value == 2;
        }

        [RunTest(true)]
        public bool GetTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            try
            {
                list.Get(0);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool GetTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.GetAt(1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool GetTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.GetAt(-1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool GetTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.GetAt(i).Value != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool GetTest_SetClearSetGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.GetAt(0).Value == 2;
        }

        [RunTest(true)]
        public bool GetTest_SetClearGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            try
            {
                list.GetAt(0);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Get Tests

        #region Set Tests

        [RunTest(true)]
        public virtual bool SetTest_Set1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Set(0, 2, 2);
            return list.GetAt(0).Value == 2;
        }

        [RunTest(true)]
        public virtual bool SetTest_Set2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.Set(1, 3, 3);
            return list.GetAt(1).Value == 3;
        }

        [RunTest(true)]
        public bool SetTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            try
            {
                list.Set(0, 1, 1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool SetTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.Set(1, 2, 2);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool SetTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.Set(-1, 2, 2);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool SetTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.Set(i, i + 1, i + 1);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.GetAt(i).Value != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool SetTest_SetClearSetGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Set(0, 3, 3);
            return list.GetAt(0).Value == 3;
        }

        [RunTest(true)]
        public bool SetTest_SetClearGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            try
            {
                list.Set(0, 2, 2);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Set Tests

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_Add1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Get(1) == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_Add2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.GetAt(0).Value == 1 && list.GetAt(1).Value == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool AddTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.GetAt(i).Value != i)
                {
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool AddTest_SetClearAddGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.GetAt(0).Value == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_SetClearAddAddGet()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Add(3, 3);
            return list.GetAt(1).Value == 3 && list.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_FloatKey()
        {
            OrderedList<float, int> list = new OrderedList<float, int>();
            
            list.Add(5.5f, 1);
            list.Add(0, 0);
            list.Add(5.5f, 2);

            if (list.GetAt(0).Value != 0 || list.GetAt(1).Value != 1 || list.GetAt(2).Value != 2) {
                Log("List in wrong order. " + list);
            }

            return list.GetAt(0).Value == 0 && list.GetAt(1).Value == 1 && list.GetAt(2).Value == 2;
        }

        #endregion // Add Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_Remove1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Remove(1, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_Remove2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.Remove(2, 2);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public virtual bool RemoveTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.Remove(i, i);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_AddClearAddRemove()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Remove(2, 2);

            if (list.Count != 0)
            {
                Log("RemoveTest_AddClearAddRemove failed. Count != 0. Count: " + list.Count);
            }

            return list.Count == 0;
        }

        #endregion // Remove Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_Clear1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_Clear2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.Clear();
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            list.Clear();

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_AddClearAddClear()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Clear();
            return list.Count == 0;
        }

        #endregion // Clear Tests

        #region Count Tests

        [RunTest(true)]
        public bool CountTest_Add1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool CountTest_Add2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool CountTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool CountTest_AddClearAddCount()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool CountTest_AddClearAddAddCount()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Add(3, 3);
            return list.Count == 2;
        }

        #endregion // Count Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_Contains1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Contains(1, 1);
        }

        [RunTest(true)]
        public bool ContainsTest_Contains2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.Contains(2, 2);
        }

        [RunTest(true)]
        public virtual bool ContainsTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (!list.Contains(i, i))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool ContainsTest_AddClearAddContains()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.Contains(2, 2);
        }

        [RunTest(true)]
        public bool ContainsTest_AddClearAddAddContains()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Add(3, 3);
            return list.Contains(3, 3);
        }

        #endregion // Contains Tests

        #region IndexOf Tests

        [RunTest(true)]
        public bool IndexOfTest_IndexOf1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.IndexOf(1, 1) == 0;
        }

        [RunTest(true)]
        public bool IndexOfTest_IndexOf2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.IndexOf(2 ,2) == 1;
        }

        [RunTest(true)]
        public virtual bool IndexOfTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.IndexOf(i, i) != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool IndexOfTest_AddClearAddIndexOf()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.IndexOf(2, 2) == 0;
        }

        [RunTest(true)]
        public bool IndexOfTest_AddClearAddAddIndexOf()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.Add(3, 3);
            return list.IndexOf(3, 3) == 1;
        }

        [RunTest(true)]
        public bool IndexOfTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.IndexOf(2, 2) == -1;
        }

        [RunTest(true)]
        public bool IndexOfTest_NotFoundEmptyList()
        {
            OrderedList<int, int> list = CreateList();
            return list.IndexOf(1, 1) == -1;
        }

        #endregion // IndexOf Tests

        #region Insert Tests

        [RunTest(true)]
        public bool InsertTest_Insert1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Insert(0, 1, 1);
            return list.GetAt(0).Value == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertTest_Insert2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Insert(0, 1, 1);
            list.Insert(1, 2, 2);
            return list.GetAt(1).Value == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool InsertTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Insert(i, i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.GetAt(i).Value != i)
                {
                    Log("InsertTest_Range failed. " + list.Get(i) + " != " + i + " - " + list);
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool InsertTest_AddClearInsert()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Insert(0, 2, 2);
            return list.GetAt(0).Value == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertTest_AddClearInsertInsert()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Insert(0, 2, 2);
            list.Insert(1, 3, 3);
            return list.GetAt(1).Value == 3 && list.Count == 2;
        }

        #endregion // Insert Tests

        #region RemoveAt Tests

        [RunTest(true)]
        public bool RemoveAtTest_RemoveAt1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAt(0);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAtTest_RemoveAt2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveAt(1);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public virtual bool RemoveAtTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.RemoveAt(0);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAtTest_AddClearAddRemoveAt()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.RemoveAt(0);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAtTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.RemoveAt(1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveAtTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.RemoveAt(-1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // RemoveAt Tests

        #region CopyTo Tests

        [RunTest(true)]
        public bool CopyToTest_CopyTo1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[1] { new ComparablePair<int, int>(0, 0) };
            list.CopyTo(array, 0);

            if (!array[0].Equals(new ComparablePair<int,int>(1, 1)))
            {
                Log("CopyToTest_CopyTo1Item failed. " + array[0] + " != " + new ComparablePair<int,int>(1, 1));
                return false;
            }

            return array[0].Equals(new ComparablePair<int,int>(1, 1));
        }

        [RunTest(true)]
        public bool CopyToTest_CopyTo2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[2];
            list.CopyTo(array, 0);
            return array[0].Equals(new ComparablePair<int,int>(1,1)) && array[1].Equals(new ComparablePair<int,int>(2,2));
        }

        [RunTest(true)]
        public virtual bool CopyToTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            ComparablePair<int,int>[] array = new ComparablePair<int, int>[100];
            list.CopyTo(array, 0);

            for (int i = 0; i < 100; i++)
            {
                if (!array[i].Equals(new ComparablePair<int,int>(i, i)))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool CopyToTest_AddClearAddCopyTo()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[1];
            list.CopyTo(array, 0);
            return array[0].Equals(new ComparablePair<int,int>(2, 2));
        }

        [RunTest(true)]
        public bool CopyToTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[0];
            list.CopyTo(array, 0);
            return true;
        }

        [RunTest(true)]
        public bool CopyToTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[1];
            try
            {
                list.CopyTo(array, 1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool CopyToTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            ComparablePair<int,int>[] array = new ComparablePair<int, int>[1];
            try
            {
                list.CopyTo(array, -1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // CopyTo Tests

        #region GetEnumerator Tests

        [RunTest(true)]
        public bool GetEnumeratorTest_Enumerate1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            IEnumerator<ComparablePair<int,int>> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current.Equals(new ComparablePair<int,int>(1, 1));
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_Enumerate2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            IEnumerator<ComparablePair<int,int>> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            return enumerator.Current.Equals(new ComparablePair<int,int>(2, 2));
        }

        [RunTest(true)]
        public virtual bool GetEnumeratorTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            IEnumerator<ComparablePair<int,int>> enumerator = list.GetEnumerator();

            for (int i = 0; i < 100; i++)
            {
                enumerator.MoveNext();
                if (!enumerator.Current.Equals(new ComparablePair<int,int>(i, i)))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_AddClearAddEnumerate()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            IEnumerator<ComparablePair<int,int>> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current.Equals(new ComparablePair<int,int>(2, 2));
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            IEnumerator<ComparablePair<int,int>> enumerator = list.GetEnumerator();
            return !enumerator.MoveNext();
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_MultipleEnumerators()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            IEnumerator<ComparablePair<int,int>> enumerator1 = list.GetEnumerator();
            IEnumerator<ComparablePair<int,int>> enumerator2 = list.GetEnumerator();
            enumerator1.MoveNext();
            enumerator2.MoveNext();
            enumerator1.MoveNext();
            return enumerator1.Current.Equals(new ComparablePair<int,int>(2,2)) && enumerator2.Current.Equals(new ComparablePair<int,int>(1,1));
        }

        [RunTest(true)]
        public virtual bool GetEnumeratorTest_Foreach()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            int index = 0;
            foreach (ComparablePair<int,int> item in list)
            {
                if (item.Value != index)
                {
                    return false;
                }
                index++;
            }

            return true;
        }

        #endregion // GetEnumerator Tests

        #region Accessor Operator Tests

        [RunTest(true)]
        public bool AccessorOperatorTest_AccessorOperator1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 2);
            return list[0].Value == 2;
        }

        [RunTest(true)]
        public bool AccessorOperatorTest_AccessorOperator2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list[1].Value == 2;
        }

        [RunTest(true)]
        public virtual bool AccessorOperatorTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i].Value != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool AccessorOperatorTest_SetRange()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                list[i] = new ComparablePair<int,int>(i + 1, i + 1);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i].Value != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion // Accessor Operator Tests

        #region AddRange Tests

        [RunTest(true)]
        public bool AddRangeTest_AddRange1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.AddRange(new ComparablePair<int,int>[] { new ComparablePair<int,int>(1, 1)});
            return list.GetAt(0).Value == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddRangeTest_AddRange2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.AddRange(new ComparablePair<int,int>[] { new ComparablePair<int,int>(1, 1), new ComparablePair<int,int>(2, 2) });
            return list.GetAt(0).Value == 1 && list.GetAt(1).Value == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public bool AddRangeTest_Range()
        {
            OrderedList<int, int> list = CreateList();
            list.AddRange(new ComparablePair<int,int>[] { new ComparablePair<int,int>(0,0), new ComparablePair<int,int>(1,1), new ComparablePair<int,int>(2,2), new ComparablePair<int,int>(3,3), new ComparablePair<int,int>(4,4), new ComparablePair<int,int>(5,5), new ComparablePair<int,int>(6,6), new ComparablePair<int,int>(7,7), new ComparablePair<int,int>(8,8), new ComparablePair<int,int>(9,9)});

            for (int i = 0; i < 10; i++)
            {
                if (list.GetAt(i).Value != i)
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public bool AddRangeTest_AddClearAddRange()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.AddRange(new ComparablePair<int,int>[] { new ComparablePair<int,int>(2, 2)});
            return list.GetAt(0).Value == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddRangeTest_EmptyRange()
        {
            OrderedList<int, int> list = CreateList();
            list.AddRange(new ComparablePair<int,int>[0]);
            return list.Count == 0;
        }

        #endregion // AddRange Tests

        #region InsertRange Tests

        [RunTest(true)]
        public bool InsertRangeTest_InsertRange1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.InsertRange(0, new ComparablePair<int,int>[] { new ComparablePair<int,int>(1, 1) });
            return list.GetAt(0).Value == 1 && list.Count == 1;
        }

        [RunTest(true)]

        public bool InsertRangeTest_InsertRange2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.InsertRange(0, new ComparablePair<int,int>[] { new ComparablePair<int,int>(1,1), new ComparablePair<int,int>(2,2) });
            return list.GetAt(0).Value == 1 && list.GetAt(1).Value == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public bool InsertRangeTest_Range()
        {
            OrderedList<int, int> list = CreateList();
            list.InsertRange(0, new ComparablePair<int,int>[] { new ComparablePair<int,int>(0,0), new ComparablePair<int,int>(1,1), new ComparablePair<int,int>(2,2), new ComparablePair<int,int>(3,3), new ComparablePair<int,int>(4,4), new ComparablePair<int,int>(5,5), new ComparablePair<int,int>(6,6), new ComparablePair<int,int>(7,7), new ComparablePair<int,int>(8,8), new ComparablePair<int,int>(9,9)});

            for (int i = 0; i < 10; i++)
            {
                if (list.GetAt(i).Value != i)
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public bool InsertRangeTest_AddClearInsertRange()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.InsertRange(0, new ComparablePair<int,int>[] { new ComparablePair<int,int>(2, 2)});
            return list.GetAt(0).Value == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertRangeTest_EmptyRange()
        {
            OrderedList<int, int> list = CreateList();
            list.InsertRange(0, new ComparablePair<int,int>[0]);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool InsertRangeTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            try
            {
                list.InsertRange(1, new ComparablePair<int,int>[] { new ComparablePair<int,int>(1, 1) });
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool InsertRangeTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            try
            {
                list.InsertRange(-1, new ComparablePair<int,int>[] { new ComparablePair<int,int>(1,1) });
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool InsertRangeTest_InsertRangeRange()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(5, 5);
            list.InsertRange(0, new ComparablePair<int,int>[] { new ComparablePair<int,int>(1,1), new ComparablePair<int,int>(4,4) });
            list.InsertRange(1, new ComparablePair<int,int>[] { new ComparablePair<int,int>(2,2), new ComparablePair<int,int>(3,3) });

            for (int i = 0; i < 5; i++)
            {
                if (list.GetAt(i).Value != (i + 1))
                {
                    Log("InsertRangeTest_InsertRangeRange failed. Expected: " + (i + 1) + " Actual: " + list.Get(i) + " - " + list.ToString());
                    return false;
                }
            }

            if (list.Count != 5)
            {
                Log("InsertRangeTest_InsertRangeRange failed. Count != 5. Count: " + list.Count + " - " + list.ToString());
                return false;
            }

            return true;
        }

        #endregion // InsertRange Tests

        #region RemoveRange Tests

        [RunTest(true)]
        public bool RemoveRangeTest_RemoveRange1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveRange(0, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_RemoveRange2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveRange(1, 1);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i, i);
            }

            list.RemoveRange(0, 10);

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_AddClearAddRemoveRange()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.RemoveRange(0, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_OutOfBounds()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.RemoveRange(1, 1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveRangeTest_OutOfBoundsNegative()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.RemoveRange(-1, 1);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        [RunTest(true)]
        public bool RemoveRangeTest_TooLarge()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.RemoveRange(0, 2);
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // RemoveRange Tests

        #region Remove First Tests

        [RunTest(true)]
        public bool RemoveFirstTest_RemoveFirst1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveFirst(1, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_RemoveFirst2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveFirst(2, 2);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveFirst(i, i);
            }

            if (list.Count != 0)
            {
                Log(this.TesterName + "::RemoveFirstTest_Range Failed. Count != 0: " + list.Count + ". " + list);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_AddClearAddRemoveFirst()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.RemoveFirst(2, 2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return !list.RemoveFirst(2, 2);
        }

        [RunTest(true)]
        public bool RemoveFirstTest_NotFoundEmptyList()
        {
            OrderedList<int, int> list = CreateList();
            return !list.RemoveFirst(1, 1);
        }

        #endregion // Remove First Tests

        #region Remove Last Tests

        [RunTest(true)]
        public bool RemoveLastTest_RemoveLast1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveLast(1, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_RemoveLast2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveLast(2, 2);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public bool RemoveLastTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i, i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveLast(i, i);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_AddClearAddRemoveLast()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.RemoveLast(2, 2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return !list.RemoveLast(2, 2);
        }

        [RunTest(true)]
        public bool RemoveLastTest_NotFoundEmptyList()
        {
            OrderedList<int, int> list = CreateList();
            return !list.RemoveLast(1, 1);
        }

        #endregion // Remove Last Tests

        #region Remove All Tests

        [RunTest(true)]
        public bool RemoveAllTest_RemoveAll1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(1, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAllTest_RemoveAll2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveAll(2, 2);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public bool RemoveAllTest_Multiple()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i, i);
                list.Add(i, i);
                list.Add(i, i);
                list.Add(i, i);
                list.Add(i, i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveAll(i, i);
            }

            if (list.Count != 0)
            {
                Log(this.TesterName + "::RemoveAllTest_Multiple Failed. Count != 0: " + list.Count + ". " + list);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAllTest_AddClearAddRemoveAll()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            list.RemoveAll(2, 2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAllTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(2, 2);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveAllTest_NotFoundEmptyList()
        {
            OrderedList<int, int> list = CreateList();
            list.RemoveAll(1, 1);
            return list.Count == 0;
        }

        #endregion // Remove All Tests

        #region Get First Tests

        [RunTest(true)]
        public bool GetFirstTest_GetFirst1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.First().Value == 1;
        }

        [RunTest(true)]
        public bool GetFirstTest_GetFirst2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.First().Value == 1;
        }

        [RunTest(true)]
        public bool GetFirstTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++) {
                list.Add(i, i);
            }

            return list.First().Value == 0;
        }

        [RunTest(true)]
        public bool GetFirstTest_AddClearAddGetFirst()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.First().Value == 2;
        }

        [RunTest(true)]
        public bool GetFirstTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(1, 1);
            try
            {
                list.First();
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Get First Tests

        #region Get Last Tests

        [RunTest(true)]
        public bool GetLastTest_GetLast1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Last().Value == 1;
        }

        [RunTest(true)]
        public bool GetLastTest_GetLast2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.Last().Value == 2;
        }

        [RunTest(true)]
        public bool GetLastTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++) {
                list.Add(i, i);
            }

            return list.Last().Value == 9;
        }

        [RunTest(true)]
        public bool GetLastTest_AddClearAddGetLast()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.Last().Value == 2;
        }

        [RunTest(true)]
        public bool GetLastTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(1, 1);
            try
            {
                list.Last();
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Get Last Tests

        #region Take First Tests

        [RunTest(true)]
        public bool TakeFirstTest_TakeFirst1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.TakeFirst().Value == 1 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_TakeFirst2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.TakeFirst().Value == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool TakeFirstTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++) {
                list.Add(i, i);
            }

            for (int i = 0; i < 10; i++)
            {
                if (list.TakeFirst().Value != i)
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_AddClearAddTakeFirst()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.TakeFirst().Value == 2 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(1, 1);
            try
            {
                list.TakeFirst();
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Take First Tests

        #region Take Last Tests

        [RunTest(true)]
        public bool TakeLastTest_TakeLast1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.TakeLast().Value == 1 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_TakeLast2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.TakeLast().Value == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool TakeLastTest_Range()
        {
            OrderedList<int, int> list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i, i);
            }

            for (int i = 9; i >= 0; i--)
            {
                if (list.TakeLast().Value != i)
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_AddClearAddTakeLast()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Clear();
            list.Add(2, 2);
            return list.TakeLast().Value == 2 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveAll(1, 1);
            try
            {
                list.TakeLast();
                return false;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        #endregion // Take Last Tests

        #region ToString Tests

        [RunTest(true)]
        public bool ToStringTest_Empty()
        {
            OrderedList<int, int> list = CreateList();

            if (list.ToString() != "{ }")
            {
                Log(TesterName + "::ToStringTest_Empty failed. Expected: { } Actual: " + list.ToString());
            }

            return list.ToString() == "{ }";
        }

        [RunTest(true)]
        public bool ToStringTest_1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);

            if (list.ToString() != "{ <1,1> }")
            {
                Log(TesterName + "::ToStringTest_1Item failed. Expected: { 1 } Actual: " + list.ToString());
            }

            return list.ToString() == "{ <1,1> }";
        }

        [RunTest(true)]
        public bool ToStringTest_2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);

            if (list.ToString() != "{ <1,1>, <2,2> }")
            {
                Log(TesterName + "::ToStringTest_2Items failed. Expected: { <1,1>, <2,2> } Actual: " + list.ToString());
            }

            return list.ToString() == "{ <1,1>, <2,2> }";
        }

        #endregion // ToString Tests

        #region Key and value Tests

        [RunTest(true)]
        public bool KeyValueTest_Key1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 2);
            return list[0].Key == 1 && list[0].Value == 2;
        }

        [RunTest(true)]
        public bool KeyValueTest_Key1Item2()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(2, 1);
            return list[0].Key == 2 && list[0].Value == 1;
        }

        [RunTest(true)]
        public bool KeyValueTest_MultipleItems()
        {
            OrderedList<int, int> list = CreateList();
            for (int i = 0; i < 100; i++) {
                list.Add(i, i + 1);
            }
            
            for (int i = 0; i < 100; i++) {
                if (list[i].Key != i || list[i].Value != i + 1) {
                    return false;
                }
            }

            return true;
        }

        #endregion // Key Tests

        #region Contains Key Tests

        [RunTest(true)]
        public bool ContainsKeyTest_ContainsKey1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.ContainsKey(1);
        }

        [RunTest(true)]
        public bool ContainsKeyTest_ContainsKey2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.ContainsKey(2);
        }

        [RunTest(true)]
        public bool ContainsKeyTest_Range()
        {
            OrderedList<int, int> list = CreateList();
            for (int i = 0; i < 100; i++) {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++) {
                if (!list.ContainsKey(i)) {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool ContainsKeyTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return !list.ContainsKey(2);
        }

        #endregion // Contains Key Tests

        #region Contains Value Tests

        [RunTest(true)]
        public bool ContainsValueTest_ContainsValue1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.ContainsValue(1);
        }

        [RunTest(true)]
        public bool ContainsValueTest_ContainsValue2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            return list.ContainsValue(2);
        }

        [RunTest(true)]
        public bool ContainsValueTest_Range()
        {
            OrderedList<int, int> list = CreateList();
            for (int i = 0; i < 100; i++) {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++) {
                if (!list.ContainsValue(i)) {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool ContainsValueTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return !list.ContainsValue(2);
        }

        #endregion // Contains Value Tests

        #region Remove Key Tests

        [RunTest(true)]
        public bool RemoveKeyTest_RemoveKey1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveKey(1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveKeyTest_RemoveKey2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            list.RemoveKey(2);
            return list.Count == 1 && list.GetAt(0).Value == 1;
        }

        [RunTest(true)]
        public bool RemoveKeyTest_Range()
        {
            OrderedList<int, int> list = CreateList();
            for (int i = 0; i < 100; i++) {
                list.Add(i, i);
            }

            for (int i = 0; i < 100; i++) {
                list.RemoveKey(i);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveKeyTest_RemovesFirstInstance()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(1, 2);
            list.RemoveKey(1);
            return list.Count == 1 && list.GetAt(0).Value == 2;
        }

        [RunTest(true)]
        public bool RemoveKeyTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.RemoveKey(2);
            return list.Count == 1;
        }

        #endregion // Remove Key Tests

        #region GetValues Test

        [RunTest(true)]
        public bool GetValuesTest_1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            ArrayList<int> values = list.GetValues(1);
            return values.Count == 1 && values[0] == 1;
        }

        [RunTest(true)]
        public bool GetValuesTest_2Items()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(2, 2);
            ArrayList<int> values = list.GetValues(1);
            return values.Count == 1 && values[0] == 1;
        }

        [RunTest(true)]
        public bool GetValuesTest_EmptyList()
        {
            OrderedList<int, int> list = CreateList();
            ArrayList<int> values = list.GetValues(1);
            return values.Count == 0;
        }

        [RunTest(true)]
        public bool GetValuesTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            ArrayList<int> values = list.GetValues(2);
            return values.Count == 0;
        }

        [RunTest(true)]
        public bool GetValuesTest_Multiple()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            list.Add(1, 2);
            list.Add(1, 3);
            list.Add(1, 4);
            list.Add(1, 5);
            ArrayList<int> values = list.GetValues(1);
            return values.Count == 5 && values[0] == 1 && values[1] == 2 && values[2] == 3 && values[3] == 4 && values[4] == 5;
        }

        #endregion // GetValues Test

        #region Get Test

        [RunTest(true)] 
        public bool GetKeysTest_1Item()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            return list.Get(1) == 1;
        }

        [RunTest(true, new object[0], 1000)] 
        public bool GetKeysTest_1000Items()
        {
            OrderedList<int, int> list = CreateList();
            for (int i = 0; i < 1000; i++) {
                list.Add(i, i);

                for (int j = 0; j <= i; j++) {
                    if (list.Get(j) != j) {
                        Log("Expected: " + j + " Actual: " + list.Get(j));
                        return false;
                    }
                }
            }
            return true;
        }

        [RunTest(true)]
        public bool GetKeysTest_NotFound()
        {
            OrderedList<int, int> list = CreateList();
            list.Add(1, 1);
            try
            {
                list.Get(2);
                return false;
            }
            catch (System.ArgumentException)
            {
                return true;
            }
        }

        #endregion // Get Test
    }
}