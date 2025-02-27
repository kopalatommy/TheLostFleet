using GalacticBoundStudios.DataScribes.Managed.Lists;
using ProjectWorlds.Testing;
using System;
using System.Collections.Generic;

namespace ProjectWorlds.UnitTests
{
    public class CircularListTester : ListTester<CircularList<int>>
    {
        public CircularListTester(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override Type TestType { get { Log("Circular List Tester"); return typeof(CircularListTester); } }

        [RunTest(true)]
        public override bool GetTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                if (list.Get(i) != (i + 84))
                {
                    Log("GetTest_Range failed. " + list[i] + " != " + (i + 84) + " - " + list);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool SetTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                list.Set(i, i + 1);
            }

            for (int i = 0; i < 16; i++)
            {
                if (list.Get(i) != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool CountTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            return list.Count == 16;
        }

        [RunTest(true)]
        public override bool ContainsTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                if (!list.Contains(99 - i))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool IndexOfTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                if (list.IndexOf(i + 84) != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool CopyToTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int[] array = new int[16];
            list.CopyTo(array, 0);

            for (int i = 0; i < 16; i++)
            {
                if (array[i] != (i + 84))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool AddTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                if (list.Get(i) != (84 + i))
                {
                    return false;
                }
            }

            return list.Count == 16;
        }

        [RunTest(true)]
        public override bool GetEnumeratorTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            IEnumerator<int> enumerator = list.GetEnumerator();

            for (int i = 0; i < 16; i++)
            {
                enumerator.MoveNext();
                if (enumerator.Current != (i + 84))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool GetEnumeratorTest_Foreach()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int index = 0;
            foreach (int item in list)
            {
                if (item != (index + 84))
                {
                    return false;
                }
                index++;
            }

            return true;
        }

        [RunTest(true)]
        public override bool AccessorOperatorTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                if (list[i] != (i + 84))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool AccessorOperatorTest_SetRange()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                list[i] = i + 1;
            }

            for (int i = 0; i < 16; i++)
            {
                if (list[i] != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public override bool InsertTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Insert(i % 16, i);
                if (list[i % 16] != i)
                {
                    Log("InsertTest_Range failed. " + list[i % 16] + " != " + i + " - " + list);
                    return false;
                }
            }

            return list.Count == 16;
        }

        [RunTest(true)]
        public override bool RemoveAtTest_Range()
        {
            CircularList<int> list = CreateList();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                list.RemoveAt(0);
                if (list.Count != (16 - i - 1))
                {
                    return false;
                }
            }

            return list.Count == 0;
        }
    }
}