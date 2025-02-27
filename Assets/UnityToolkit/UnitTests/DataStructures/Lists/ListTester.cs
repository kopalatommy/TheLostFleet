using System;
using System.Collections;
using System.Collections.Generic;
using GalacticBoundStudios.DataScribes.Managed.Lists;
using ProjectWorlds.Testing;

namespace ProjectWorlds.UnitTests
{
    public class ListTester<T> : TesterBase where T : IListExtended<int>, new()
    {
        public ListTester(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {
            
        }

        public override Type TestType { get { Log("List tester"); return typeof(ListTester<T>); } }

        protected virtual T CreateList()
        {
            return new T();
        }

        #region Get Tests

        [RunTest(true)]
        public bool GetTest_Get1Item()
        {
            T list = CreateList();
            list.Add(1);
            return list.Get(0) == 1;
        }

        [RunTest(true)]
        public bool GetTest_Get2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.Get(1) == 2;
        }

        [RunTest(true)]
        public bool GetTest_EmptyList()
        {
            T list = CreateList();
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
            T list = CreateList();
            list.Add(1);
            try
            {
                list.Get(1);
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
            T list = CreateList();
            list.Add(1);
            try
            {
                list.Get(-1);
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
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.Get(i) != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool GetTest_SetClearSetGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);

            if (list.Get(0) != 2)
            {
                Log("GetTest_SetClearSetGet failed. " + list.Get(0) + " != 2");
                return false;
            }

            return list.Get(0) == 2;
        }

        [RunTest(true)]
        public bool GetTest_SetClearGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
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

        #endregion // Get Tests

        #region Set Tests

        [RunTest(true)]
        public virtual bool SetTest_Set1Item()
        {
            T list = CreateList();
            list.Add(1);
            list.Set(0, 2);
            return list.Get(0) == 2;
        }

        [RunTest(true)]
        public virtual bool SetTest_Set2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.Set(1, 3);
            return list.Get(1) == 3;
        }

        [RunTest(true)]
        public bool SetTest_EmptyList()
        {
            T list = CreateList();
            try
            {
                list.Set(0, 1);
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
            T list = CreateList();
            list.Add(1);
            try
            {
                list.Set(1, 2);
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
            T list = CreateList();
            list.Add(1);
            try
            {
                list.Set(-1, 2);
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
            T list = CreateList();

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

            if (list.Count != 100) {
                Log("SetTest_Range failed. Count is not 100. " + list.Count);
                return false;
            } else {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool SetTest_SetClearSetGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Set(0, 3);
            return list.Get(0) == 3;
        }

        [RunTest(true)]
        public bool SetTest_SetClearGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            try
            {
                list.Set(0, 2);
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
            T list = CreateList();
            list.Add(1);
            return list.Get(0) == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_Add2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.Get(1) == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool AddTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.Get(i) != i)
                {
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool AddTest_SetClearAddGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.Get(0) == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_SetClearAddAddGet()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Add(3);
            return list.Get(1) == 3 && list.Count == 2;
        }

        #endregion // Add Tests
    
        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_Remove1Item()
        {
            T list = CreateList();
            list.Add(1);
            list.Remove(1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_Remove2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            bool removed = list.Remove(2);

            if (list.Count != 1)
            {
                Log("RemoveTest_Remove2Items failed. Count != 1. Count: " + list.Count);
            }
            if (list.Get(0) != 1)
            {
                Log("RemoveTest_Remove2Items failed. Get(0) != 1. Get(0): " + list.Get(0));
            }
            if (!removed)
            {
                Log("RemoveTest_Remove2Items failed. Removed != true. Removed: " + removed);
            }

            return list.Count == 1 && list.Get(0) == 1 && removed;
        }

        [RunTest(true)]
        public virtual bool RemoveTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.Remove(i);
            }

            if (list.Count != 0)
            {
                Log("RemoveTest_Range failed. Count != 0. Count: " + list.Count + " - " + list);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_AddClearAddRemove()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Remove(2);

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
            T list = CreateList();
            list.Add(1);
            list.Clear();
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_Clear2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.Clear();
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            list.Clear();

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_AddClearAddClear()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Clear();
            return list.Count == 0;
        }

        #endregion // Clear Tests

        #region Count Tests

        [RunTest(true)]
        public bool CountTest_Add1Item()
        {
            T list = CreateList();
            list.Add(1);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool CountTest_Add2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool CountTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool CountTest_AddClearAddCount()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool CountTest_AddClearAddAddCount()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Add(3);
            return list.Count == 2;
        }

        #endregion // Count Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_Contains1Item()
        {
            T list = CreateList();
            list.Add(1);
            return list.Contains(1);
        }

        [RunTest(true)]
        public bool ContainsTest_Contains2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.Contains(2);
        }

        [RunTest(true)]
        public virtual bool ContainsTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (!list.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool ContainsTest_AddClearAddContains()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.Contains(2);
        }

        [RunTest(true)]
        public bool ContainsTest_AddClearAddAddContains()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Add(3);
            return list.Contains(3);
        }

        #endregion // Contains Tests

        #region IndexOf Tests

        [RunTest(true)]
        public bool IndexOfTest_IndexOf1Item()
        {
            T list = CreateList();
            list.Add(1);
            return list.IndexOf(1) == 0;
        }

        [RunTest(true)]
        public bool IndexOfTest_IndexOf2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.IndexOf(2) == 1;
        }

        [RunTest(true)]
        public virtual bool IndexOfTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.IndexOf(i) != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool IndexOfTest_AddClearAddIndexOf()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.IndexOf(2) == 0;
        }

        [RunTest(true)]
        public bool IndexOfTest_AddClearAddAddIndexOf()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Add(3);
            return list.IndexOf(3) == 1;
        }

        [RunTest(true)]
        public bool IndexOfTest_NotFound()
        {
            T list = CreateList();
            list.Add(1);
            return list.IndexOf(2) == -1;
        }

        [RunTest(true)]
        public bool IndexOfTest_NotFoundEmptyList()
        {
            T list = CreateList();
            return list.IndexOf(1) == -1;
        }

        #endregion // IndexOf Tests

        #region Insert Tests

        [RunTest(true)]
        public bool InsertTest_Insert1Item()
        {
            T list = CreateList();
            list.Insert(0, 1);
            return list.Get(0) == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertTest_Insert2Items()
        {
            T list = CreateList();
            list.Insert(0, 1);
            list.Insert(1, 2);
            return list.Get(1) == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public virtual bool InsertTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Insert(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list.Get(i) != i)
                {
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public bool InsertTest_AddClearInsert()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Insert(0, 2);
            return list.Get(0) == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertTest_AddClearInsertInsert()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Insert(0, 2);
            list.Insert(1, 3);
            return list.Get(1) == 3 && list.Count == 2;
        }

        #endregion // Insert Tests

        #region RemoveAt Tests

        [RunTest(true)]
        public bool RemoveAtTest_RemoveAt1Item()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAt(0);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAtTest_RemoveAt2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.RemoveAt(1);
            return list.Count == 1 && list.Get(0) == 1;
        }

        [RunTest(true)]
        public virtual bool RemoveAtTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
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
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.RemoveAt(0);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAtTest_OutOfBounds()
        {
            T list = CreateList();
            list.Add(1);
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
            T list = CreateList();
            list.Add(1);
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
            T list = CreateList();
            list.Add(1);
            int[] array = new int[1];
            list.CopyTo(array, 0);
            return array[0] == 1;
        }

        [RunTest(true)]
        public bool CopyToTest_CopyTo2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            int[] array = new int[2];
            list.CopyTo(array, 0);
            return array[0] == 1 && array[1] == 2;
        }

        [RunTest(true)]
        public virtual bool CopyToTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int[] array = new int[100];
            list.CopyTo(array, 0);

            for (int i = 0; i < 100; i++)
            {
                if (array[i] != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool CopyToTest_AddClearAddCopyTo()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            int[] array = new int[1];
            list.CopyTo(array, 0);
            return array[0] == 2;
        }

        [RunTest(true)]
        public bool CopyToTest_EmptyList()
        {
            T list = CreateList();
            int[] array = new int[0];
            list.CopyTo(array, 0);
            return true;
        }

        [RunTest(true)]
        public bool CopyToTest_OutOfBounds()
        {
            T list = CreateList();
            list.Add(1);
            int[] array = new int[1];
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
            T list = CreateList();
            list.Add(1);
            int[] array = new int[1];
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
            T list = CreateList();
            list.Add(1);
            IEnumerator<int> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current == 1;
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_Enumerate2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            IEnumerator<int> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            return enumerator.Current == 2;
        }

        [RunTest(true)]
        public virtual bool GetEnumeratorTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            IEnumerator<int> enumerator = list.GetEnumerator();

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

        [RunTest(true)]
        public bool GetEnumeratorTest_AddClearAddEnumerate()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            IEnumerator<int> enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current == 2;
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_EmptyList()
        {
            T list = CreateList();
            IEnumerator<int> enumerator = list.GetEnumerator();
            return !enumerator.MoveNext();
        }

        [RunTest(true)]
        public bool GetEnumeratorTest_MultipleEnumerators()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            IEnumerator<int> enumerator1 = list.GetEnumerator();
            IEnumerator<int> enumerator2 = list.GetEnumerator();
            enumerator1.MoveNext();
            enumerator2.MoveNext();
            enumerator1.MoveNext();
            return enumerator1.Current == 2 && enumerator2.Current == 1;
        }

        [RunTest(true)]
        public virtual bool GetEnumeratorTest_Foreach()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int index = 0;
            foreach (int item in list)
            {
                if (item != index)
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
            T list = CreateList();
            list.Add(1);
            return list[0] == 1;
        }

        [RunTest(true)]
        public bool AccessorOperatorTest_AccessorOperator2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list[1] == 2;
        }

        [RunTest(true)]
        public virtual bool AccessorOperatorTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool AccessorOperatorTest_SetRange()
        {
            T list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                list[i] = i + 1;
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != (i + 1))
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
            T list = CreateList();
            list.AddRange(new int[] { 1 });
            return list.Get(0) == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddRangeTest_AddRange2Items()
        {
            T list = CreateList();
            list.AddRange(new int[] { 1, 2 });
            return list.Get(1) == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public bool AddRangeTest_Range()
        {
            T list = CreateList();
            list.AddRange(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            for (int i = 0; i < 10; i++)
            {
                if (list.Get(i) != i)
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public bool AddRangeTest_AddClearAddRange()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.AddRange(new int[] { 2 });
            return list.Get(0) == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool AddRangeTest_EmptyRange()
        {
            T list = CreateList();
            list.AddRange(new int[0]);
            return list.Count == 0;
        }

        #endregion // AddRange Tests

        #region InsertRange Tests

        [RunTest(true)]
        public bool InsertRangeTest_InsertRange1Item()
        {
            T list = CreateList();
            list.InsertRange(0, new int[] { 1 });
            return list.Get(0) == 1 && list.Count == 1;
        }

        [RunTest(true)]

        public bool InsertRangeTest_InsertRange2Items()
        {
            T list = CreateList();
            list.InsertRange(0, new int[] { 1, 2 });
            return list.Get(1) == 2 && list.Count == 2;
        }

        [RunTest(true)]
        public bool InsertRangeTest_Range()
        {
            T list = CreateList();
            list.InsertRange(0, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            for (int i = 0; i < 10; i++)
            {
                if (list.Get(i) != i)
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public bool InsertRangeTest_AddClearInsertRange()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.InsertRange(0, new int[] { 2 });
            return list.Get(0) == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool InsertRangeTest_EmptyRange()
        {
            T list = CreateList();
            list.InsertRange(0, new int[0]);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool InsertRangeTest_OutOfBounds()
        {
            T list = CreateList();
            try
            {
                list.InsertRange(1, new int[] { 1 });
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
            T list = CreateList();
            try
            {
                list.InsertRange(-1, new int[] { 1 });
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
            T list = CreateList();
            list.Add(5);
            list.InsertRange(0, new int[] { 1, 4 });
            list.InsertRange(1, new int[] { 2, 3 });

            for (int i = 0; i < 5; i++)
            {
                if (list.Get(i) != (i + 1))
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
            T list = CreateList();
            list.Add(1);
            list.RemoveRange(0, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_RemoveRange2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.RemoveRange(1, 1);
            return list.Count == 1 && list.Get(0) == 1;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.RemoveRange(0, 10);

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_AddClearAddRemoveRange()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.RemoveRange(0, 1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveRangeTest_OutOfBounds()
        {
            T list = CreateList();
            list.Add(1);
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
            T list = CreateList();
            list.Add(1);
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
            T list = CreateList();
            list.Add(1);
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
            T list = CreateList();
            list.Add(1);
            list.RemoveFirst(1);

            if (list.Count != 0)
            {
                Log(this.TesterName + "::RemoveFirstTest_RemoveFirst1Item Failed. Count != 0: " + list.Count + ". " + list);
            }
            
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_RemoveFirst2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.RemoveFirst(2);
            return list.Count == 1 && list.Get(0) == 1;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveFirst(i);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_AddClearAddRemoveFirst()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.RemoveFirst(2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveFirstTest_NotFound()
        {
            T list = CreateList();
            list.Add(1);
            return !list.RemoveFirst(2);
        }

        [RunTest(true)]
        public bool RemoveFirstTest_NotFoundEmptyList()
        {
            T list = CreateList();
            return !list.RemoveFirst(1);
        }

        #endregion // Remove First Tests

        #region Remove Last Tests

        [RunTest(true)]
        public bool RemoveLastTest_RemoveLast1Item()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveLast(1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_RemoveLast2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.RemoveLast(2);
            return list.Count == 1 && list.Get(0) == 1;
        }

        [RunTest(true)]
        public bool RemoveLastTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveLast(i);
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_AddClearAddRemoveLast()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.RemoveLast(2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveLastTest_NotFound()
        {
            T list = CreateList();
            list.Add(1);
            return !list.RemoveLast(2);
        }

        [RunTest(true)]
        public bool RemoveLastTest_NotFoundEmptyList()
        {
            T list = CreateList();
            return !list.RemoveLast(1);
        }

        #endregion // Remove Last Tests

        #region Remove All Tests

        [RunTest(true)]
        public bool RemoveAllTest_RemoveAll1Item()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(1);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAllTest_RemoveAll2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            list.RemoveAll(2);
            return list.Count == 1 && list.Get(0) == 1;
        }

        [RunTest(true)]
        public bool RemoveAllTest_Multiple()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
                list.Add(i);
            }

            for (int i = 0; i < 10; i++)
            {
                list.RemoveAll(i);
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
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.RemoveAll(2);
            return list.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveAllTest_NotFound()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(2);
            return list.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveAllTest_NotFoundEmptyList()
        {
            T list = CreateList();
            list.RemoveAll(1);
            return list.Count == 0;
        }

        #endregion // Remove All Tests

        #region Get First Tests

        [RunTest(true)]
        public bool GetFirstTest_GetFirst1Item()
        {
            T list = CreateList();
            list.Add(1);
            return list.First() == 1;
        }

        [RunTest(true)]
        public bool GetFirstTest_GetFirst2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.First() == 1;
        }

        [RunTest(true)]
        public bool GetFirstTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            return list.First() == 0;
        }

        [RunTest(true)]
        public bool GetFirstTest_AddClearAddGetFirst()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.First() == 2;
        }

        [RunTest(true)]
        public bool GetFirstTest_EmptyList()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(1);
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
            T list = CreateList();
            list.Add(1);
            return list.Last() == 1;
        }

        [RunTest(true)]
        public bool GetLastTest_GetLast2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.Last() == 2;
        }

        [RunTest(true)]
        public bool GetLastTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            return list.Last() == 9;
        }

        [RunTest(true)]
        public bool GetLastTest_AddClearAddGetLast()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.Last() == 2;
        }

        [RunTest(true)]
        public bool GetLastTest_EmptyList()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(1);
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
            T list = CreateList();
            list.Add(1);
            return list.TakeFirst() == 1 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_TakeFirst2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.TakeFirst() == 1 && list.Count == 1;
        }

        [RunTest(true)]
        public bool TakeFirstTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 10; i++)
            {
                if (list.TakeFirst() != i)
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_AddClearAddTakeFirst()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.TakeFirst() == 2 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeFirstTest_EmptyList()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(1);
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
            T list = CreateList();
            list.Add(1);
            return list.TakeLast() == 1 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_TakeLast2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);
            return list.TakeLast() == 2 && list.Count == 1;
        }

        [RunTest(true)]
        public bool TakeLastTest_Range()
        {
            T list = CreateList();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (int i = 9; i >= 0; i--)
            {
                if (list.TakeLast() != i)
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_AddClearAddTakeLast()
        {
            T list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            return list.TakeLast() == 2 && list.Count == 0;
        }

        [RunTest(true)]
        public bool TakeLastTest_EmptyList()
        {
            T list = CreateList();
            list.Add(1);
            list.RemoveAll(1);
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
            T list = CreateList();

            if (list.ToString() != "{ }")
            {
                Log(TesterName + "::ToStringTest_Empty failed. Expected: { } Actual: " + list.ToString());
            }

            return list.ToString() == "{ }";
        }

        [RunTest(true)]
        public bool ToStringTest_1Item()
        {
            T list = CreateList();
            list.Add(1);

            if (list.ToString() != "{ 1 }")
            {
                Log(TesterName + "::ToStringTest_1Item failed. Expected: { 1 } Actual: " + list.ToString());
            }

            return list.ToString() == "{ 1 }";
        }

        [RunTest(true)]
        public bool ToStringTest_2Items()
        {
            T list = CreateList();
            list.Add(1);
            list.Add(2);

            if (list.ToString() != "{ 1, 2 }")
            {
                Log(TesterName + "::ToStringTest_2Items failed. Expected: { 1, 2 } Actual: " + list.ToString());
            }

            return list.ToString() == "{ 1, 2 }";
        }

        #endregion // ToString Tests
    }
}
