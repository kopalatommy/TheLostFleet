using ProjectWorlds.Testing;
using ProjectWorlds.DataStructures.Stacks;
using System;

namespace ProjectWorlds.UnitTests
{
    public class StackTester<T> : TesterBase where T : ProjectWorlds.DataStructures.Stacks.Stack<int>, new()
    {
        public StackTester(string testerName, string resultsDir=null, bool verbose = false) : base(testerName, resultsDir, verbose)
        {

        }

        public override Type TestType { get { return typeof(StackTester<T>); } }

        #region Count Tests

        [RunTest(true)]
        public bool TestCount_EmptyList()
        {
            T stack = new T();
            return stack.Count == 0;
        }

        [RunTest(true)]
        public bool TestCount_OneElement()
        {
            T stack = new T();
            stack.Push(1);
            return stack.Count == 1;
        }

        [RunTest(true)]
        public bool TestCount_TwoElements()
        {
            T stack = new T();
            stack.Push(1);
            stack.Push(2);
            return stack.Count == 2;
        }

        #endregion // Count Tests

        #region IsEmpty Tests

        [RunTest(true)]
        public bool TestIsEmpty_EmptyList()
        {
            T stack = new T();
            return stack.IsEmpty;
        }

        [RunTest(true)]
        public bool TestIsEmpty_OneElement()
        {
            T stack = new T();
            stack.Push(1);
            return !stack.IsEmpty;
        }

        #endregion // IsEmpty Tests

        #region IsFull Tests

        [RunTest(true)]
        public bool TestIsFull_EmptyList()
        {
            T stack = new T();
            return !stack.IsFull;
        }

        [RunTest(true)]
        public bool TestIsFull_CapacityNotSet()
        {
            T stack = new T();
            stack.Push(1);
            return !stack.IsFull;
        }

        [RunTest(true)]
        public bool TestIsFull_CapacitySetNotFull()
        {
            T stack = new T();
            stack.MaxCapacity = 5;
            stack.Push(1);
            return !stack.IsFull;
        }

        [RunTest(true)]
        public bool TestIsFull_CapacitySetFull()
        {
            T stack = new T();
            stack.MaxCapacity = 1;
            stack.Push(1);
            return stack.IsFull;
        }

        #endregion // IsFull Tests

        #region MaxCapacity Tests

        [RunTest(true)]
        public bool TestMaxCapacity_EmptyList()
        {
            T stack = new T();
            return stack.MaxCapacity == -1;
        }

        [RunTest(true)]
        public bool TestMaxCapacity_CapacitySet()
        {
            T stack = new T();
            stack.MaxCapacity = 5;
            return stack.MaxCapacity == 5;
        }

        #endregion // MaxCapacity Tests

        #region Push Tests
        
        [RunTest(true)]
        public bool TestPush_EmptyList()
        {
            T stack = new T();
            stack.Push(1);
            return stack.Count == 1;
        }

        [RunTest(true)]
        public bool TestPush_CapacityNotSet()
        {
            T stack = new T();
            stack.Push(1);
            stack.Push(2);
            return stack.Count == 2;
        }

        [RunTest(true)]
        public bool TestPush_CapacitySetNotFull()
        {
            T stack = new T();
            stack.MaxCapacity = 5;
            stack.Push(1);
            stack.Push(2);
            return stack.Count == 2;
        }

        [RunTest(true)]
        public bool TestPush_CapacitySetFull()
        {
            T stack = new T();
            stack.MaxCapacity = 1;
            stack.Push(1);
            return stack.Count == 1;
        }

        [RunTest(true)]
        public bool TestPush_CapacitySetFullException()
        {
            T stack = new T();
            stack.MaxCapacity = 1;
            stack.Push(1);
            try
            {
                stack.Push(2);
                return false;
            }
            catch (Exception e)
            {
                return e.Message == "Stack is full";
            }
        }

        #endregion // Push Tests

        #region Pop Tests

        [RunTest(true)]
        public bool TestPop_EmptyList()
        {
            T stack = new T();
            try
            {
                stack.Pop();
                return false;
            }
            catch (Exception e)
            {
                return e.Message == "Stack is empty";
            }
        }

        [RunTest(true)]
        public bool TestPop_OneElement()
        {
            T stack = new T();
            stack.Push(1);
            return stack.Pop() == 1;
        }

        [RunTest(true)]
        public bool TestPop_TwoElements()
        {
            T stack = new T();
            stack.Push(1);
            stack.Push(2);
            return stack.Pop() == 2;
        }

        [RunTest(true)]
        public bool TestPop_Range()
        {
            T stack = new T();

            for (int i = 0; i < 100; i++) {
                stack.Push(i);
            }

            for (int i = 99; i >= 0; i--)
            {
                if (stack.Pop() != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public bool TestPop_PopFromEmptyList()
        {
            T stack = new T();
            stack.Push(1);
            stack.Pop();
            try
            {
                stack.Pop();
                return false;
            }
            catch (Exception e)
            {
                return e.Message == "Stack is empty";
            }
        }

        [RunTest(true)]
        public bool TestPop_PopTooMany()
        {
            T stack = new T();
            stack.Push(1);
            stack.Pop();
            try
            {
                stack.Pop();
                return false;
            }
            catch (Exception e)
            {
                return e.Message == "Stack is empty";
            }
        }

        #endregion // Pop Tests

        #region Peek Tests

        [RunTest(true)]
        public bool TestPeek_EmptyList()
        {
            T stack = new T();
            try
            {
                stack.Peek();
                return false;
            }
            catch (Exception e)
            {
                return e.Message == "Stack is empty";
            }
        }

        [RunTest(true)]
        public bool TestPeek_OneElement()
        {
            T stack = new T();
            stack.Push(1);
            return stack.Peek() == 1;
        }

        [RunTest(true)]
        public bool TestPeek_TwoElements()
        {
            T stack = new T();
            stack.Push(1);
            stack.Push(2);
            return stack.Peek() == 2;
        }

        #endregion // Peek Tests

        #region Clear Tests

        [RunTest(true)]
        public bool TestClear_EmptyList()
        {
            T stack = new T();
            stack.Clear();
            return stack.Count == 0;
        }

        [RunTest(true)]
        public bool TestClear_OneElement()
        {
            T stack = new T();
            stack.Push(1);
            stack.Clear();
            return stack.Count == 0;
        }

        [RunTest(true)]
        public bool TestClear_TwoElements()
        {
            T stack = new T();
            stack.Push(1);
            stack.Push(2);
            stack.Clear();
            return stack.Count == 0;
        }

        #endregion // Clear Tests
    }
}