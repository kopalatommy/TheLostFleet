namespace ProjectWorlds.DataStructures.Heaps
{
    public class MinHeap<T> : System.Collections.Generic.IEnumerable<T> where T : System.IComparable<T>
    {
        protected class MinHeapEnumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator
        {
            private T[] items;
            int index;
            private T current;

            public MinHeapEnumerator(MinHeap<T> heap)
            {
                items = new T[heap.Count];
                heap.ToCollection(items);
                index = -1;
                current = default(T);
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return current;
                }
            }

            public void Dispose()
            {
                items = null;
                current = default(T);
            }

            public bool MoveNext()
            {
                if (index < items.Length - 1)
                {
                    index++;
                    current = items[index];
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default(T);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return count == 0;
            }
        }

        protected bool IsFull
        {
            get
            {
                return count == buffer.Length;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        protected T[] buffer;
        protected int count;

        public MinHeap()
        {
            buffer = new T[16];
            count = 0;
        }

        public MinHeap(int capacity)
        {
            buffer = new T[capacity];
            count = 0;
        }

        public MinHeap(T[] elements)
        {
            buffer = new T[elements.Length];
            count = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                Add(elements[i]);
            }
        }

        public void Add(T item)
        {
            if (IsFull)
            {
                GrowBuffer();
            }

            buffer[count] = item;
            count++;

            HeapifyUp(count - 1);
        }

        protected void HeapifyUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            int parent = (index - 1) / 2;

            if (buffer[index].CompareTo(buffer[parent]) < 0)
            {
                T temp = buffer[index];
                buffer[index] = buffer[parent];
                buffer[parent] = temp;

                HeapifyUp(parent);
            }
        }

        public T Peek()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            return buffer[0];
        }

        public T Remove()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            T item = buffer[0];
            buffer[0] = buffer[count - 1];
            count--;

            HeapifyDown(0);

            return item;
        }

        protected void HeapifyDown(int index)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left < count && buffer[left].CompareTo(buffer[smallest]) < 0)
            {
                smallest = left;
            }

            if (right < count && buffer[right].CompareTo(buffer[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest != index)
            {
                T temp = buffer[index];
                buffer[index] = buffer[smallest];
                buffer[smallest] = temp;

                HeapifyDown(smallest);
            }
        }

        public void Clear()
        {
            count = 0;
        }

        public T[] ToArray()
        {
            T[] array = new T[count];
            System.Array.Copy(buffer, array, count);
            return array;
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new MinHeapEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MinHeapEnumerator(this);
        }

        public T MinValue()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            return buffer[0];
        }

        public T TakeMin()
        {
            return Remove();
        }

        public T MaxValue()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            T max = buffer[0];

            for (int i = 1; i < count; i++)
            {
                if (buffer[i].CompareTo(max) > 0)
                {
                    max = buffer[i];
                }
            }

            return max;
        }

        public T TakeMax()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            T max = buffer[0];
            int maxIndex = 0;

            for (int i = 1; i < count; i++)
            {
                if (buffer[i].CompareTo(max) > 0)
                {
                    max = buffer[i];
                    maxIndex = i;
                }
            }

            T item = buffer[maxIndex];
            buffer[maxIndex] = buffer[count - 1];
            count--;

            HeapifyDown(maxIndex);

            return item;
        }

        public void ToCollection(T[] collection)
        {
            if (collection == null)
            {
                throw new System.ArgumentNullException("collection");
            }

            if (collection.Length < count)
            {
                throw new System.ArgumentException("Collection is too small");
            }

            System.Array.Copy(buffer, collection, count);

            // Sort the collection
            ProjectWorlds.Algorithms.QuickSort.Sort(collection, 0, count - 1);
        }

        protected void GrowBuffer()
        {
            int newCapacity = buffer.Length < 1024 ? buffer.Length * 2 : buffer.Length + 512;

            T[] newBuffer = new T[newCapacity];
            System.Array.Copy(buffer, newBuffer, buffer.Length);
            buffer = newBuffer;
        }
    }
}