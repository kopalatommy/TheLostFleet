using System;
using GalacticBoundStudios.DataScribes.Managed.Stacks;

namespace ProjectWorlds.MemoryManagement
{
    public class ObjectPool<T> where T : new()
    {
        public bool IsEmpty
        {
            get
            {
                return pool.Count == 0;
            }
        }

        public int Count
        {
            get
            {
                return pool.Count;
            }
        }

        public bool IsFull
        {
            get
            {
                return isFixedSize && pool.Count == capacity;
            }
        }

        public CreateObjectDelegate CreateObjectFunction
        {
            get
            {
                return createObjectFunct;
            }
            set
            {
                createObjectFunct = value;
            }
        }

        private CreateObjectDelegate createObjectFunct = null;

        private readonly Stack<T> pool;

        private bool isFixedSize;

        public delegate T CreateObjectDelegate();

        private int capacity = -1;

        public ObjectPool()
        {
            pool = new Stack<T>();
            isFixedSize = false;
            this.createObjectFunct = DefaultCreateObject;
        }

        public ObjectPool(int size)
        {
            pool = new Stack<T>(size);
            isFixedSize = true;
            this.createObjectFunct = DefaultCreateObject;
            capacity = size;
        }

        public T GetObject()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            return new T();
        }

        public void PutObject(T item)
        {
            if (isFixedSize && pool.Count == capacity)
            {
                throw new InvalidOperationException("The pool is full.");
            }

            pool.Push(item);
        }

        protected T DefaultCreateObject()
        {
            return default(T);
        }
    }
}