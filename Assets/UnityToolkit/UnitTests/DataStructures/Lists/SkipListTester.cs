using GalacticBoundStudios.DataScribes.Managed.Lists;
using ProjectWorlds.Testing;
using System;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class SkipListTester  : ListTester<SkipList<int>>
    {
        public SkipListTester(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override Type TestType { get { Log("SkipList Tester"); return typeof(SkipListTester); } }

        [RunTest(true)]
        public bool InverseAddTest()
        {
            SkipList<int> list = CreateList();

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
            SkipList<int> list = CreateList();

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
            SkipList<int> list = CreateList();
            list.Add(1);
            list.Set(0, 2);
            return list.Get(0) == 2;
        }

        [RunTest(true)]
        public override bool SetTest_Set2Items()
        {
            SkipList<int> list = CreateList();
            list.Add(1);
            list.Add(2);
            list.Set(1, 3);
            return list.Get(1) == 3;
        }

        [RunTest(true)]
        public override bool SetTest_SetClearSetGet()
        {
            SkipList<int> list = CreateList();
            list.Add(1);
            list.Clear();
            list.Add(2);
            list.Set(0, 3);
            return list.Get(0) == 3;
        }

        [RunTest(true)]
        public override bool SetTest_Range()
        {
            SkipList<int> list = CreateList();

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
                    Log("SetTest_Range failed. " + list.Get(i) + " != " + (i + 1) + " - " + list + " - " + list.Count);
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
    }
}