using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Stacks
{
    public class Stack<T>
    {
        protected ArrayList<T> list = null;

        protected int maxCapacity = -1;

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return list.Count == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return maxCapacity != -1 && list.Count == maxCapacity;
            }
        }

        public int MaxCapacity
        {
            get
            {
                return maxCapacity;
            }
            set
            {
                if (value < 0)
                {
                    throw new System.Exception("Max capacity must be greater than or equal to 0");
                }

                if (value < list.Count)
                {
                    while (list.Count > value)
                    {
                        Pop();
                    }
                }

                maxCapacity = value;
            }
        }

        public Stack()
        {
            list = new ArrayList<T>(1);
        }

        public Stack(int capacity)
        {
            maxCapacity = capacity;
            list = new ArrayList<T>(capacity);
        }

        public void Push(T item)
        {
            if (maxCapacity != -1 && list.Count == maxCapacity)
            {
                throw new System.Exception("Stack is full");
            }

            list.Add(item);
        }

        public T Pop()
        {
            if (list.Count == 0)
            {
                throw new System.Exception("Stack is empty");
            }

            return list.TakeLast();
        }

        public T Peek()
        {
            if (list.Count == 0)
            {
                throw new System.Exception("Stack is empty");
            }

            return list.Last();
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}
