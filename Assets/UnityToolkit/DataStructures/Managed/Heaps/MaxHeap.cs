namespace GalacticBoundStudios.DataScribes.Managed.Heaps
{
    public class MaxHeap<T> : System.Collections.Generic.IEnumerable<T> where T : System.IComparable<T>
    {
        protected class MaxHeapEnumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator
        {
            private T[] items;
            int index;
            private T current;

            public MaxHeapEnumerator(MaxHeap<T> heap)
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

        public bool IsFull
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

        private T[] buffer;
        private int count;

        public MaxHeap()
        {
            buffer = new T[16];
            count = 0;
        }

        public MaxHeap(int capacity)
        {
            buffer = new T[capacity];
            count = 0;
        }

        public MaxHeap(T[] collection)
        {
            buffer = new T[collection.Length];
            count = 0;

            for (int i = 0; i < collection.Length; i++)
            {
                Add(collection[i]);
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
            int parent = (index - 1) / 2;

            while (index > 0 && buffer[index].CompareTo(buffer[parent]) > 0)
            {
                T temp = buffer[index];
                buffer[index] = buffer[parent];
                buffer[parent] = temp;

                index = parent;
                parent = (index - 1) / 2;
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
            count--;

            if (count > 0)
            {
                buffer[0] = buffer[count];
                HeapifyDown(0);
            }

            return item;
        }

        private void HeapifyDown(int index)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int largest = index;

            if (left < count && buffer[left].CompareTo(buffer[largest]) > 0)
            {
                largest = left;
            }

            if (right < count && buffer[right].CompareTo(buffer[largest]) > 0)
            {
                largest = right;
            }

            if (largest != index)
            {
                T temp = buffer[index];
                buffer[index] = buffer[largest];
                buffer[largest] = temp;

                HeapifyDown(largest);
            }
        }

        public void Clear()
        {
            count = 0;
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new MaxHeapEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MaxHeapEnumerator(this);
        }

        public T MinValue()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            T min = buffer[0];

            for (int i = 1; i < count; i++)
            {
                if (buffer[i].CompareTo(min) < 0)
                {
                    min = buffer[i];
                }
            }

            return min;
        }

        public T TakeMin()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            T min = buffer[0];
            int minIndex = 0;

            for (int i = 1; i < count; i++)
            {
                if (buffer[i].CompareTo(min) < 0)
                {
                    min = buffer[i];
                    minIndex = i;
                }
            }

            buffer[minIndex] = buffer[count - 1];
            count--;

            HeapifyDown(minIndex);

            return min;
        }

        public T MaxValue()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty");
            }

            return buffer[0];
        }

        public T TakeMax()
        {
            return Remove();
        }

        public T[] ToArray()
        {
            T[] array = new T[count];
            System.Array.Copy(buffer, array, count);
            return array;
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
            ProjectWorlds.Algorithms.QuickSort.Sort(collection, 0, count - 1, (T x, T y) => y.CompareTo(x));
        }

        public override string ToString()
        {
            string result = "{ ";
            foreach (T item in this)
            {
                result += item + " ";
            }

            result += "}";
            return result;
        }

        protected void GrowBuffer()
        {
            int newCapacity = buffer.Length < 1024 ? buffer.Length * 2 : buffer.Length + 512;

            T[] newBuffer = new T[newCapacity];
            System.Array.Copy(buffer, newBuffer, buffer.Length);
            buffer = newBuffer;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class MaxHeap<TKey, TValue> where TKey : System.IComparable<TKey>
    {
        protected MaxHeap<KeyValuePair> heap;

        public struct KeyValuePair : System.IComparable<KeyValuePair>
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public int CompareTo(KeyValuePair other)
            {
                return Key.CompareTo(other.Key);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return heap.IsEmpty;
            }
        }

        public bool IsFull
        {
            get
            {
                return heap.IsFull;
            }
        }

        public int Count
        {
            get
            {
                return heap.Count;
            }
        }

        public MaxHeap()
        {
            heap = new MaxHeap<KeyValuePair>();
        }

        public MaxHeap(int capacity)
        {
            heap = new MaxHeap<KeyValuePair>(capacity);
        }

        public MaxHeap(KeyValuePair[] elements)
        {
            heap = new MaxHeap<KeyValuePair>(elements);
        }

        public void Add(TKey key, TValue value)
        {
            heap.Add(new KeyValuePair(key, value));
        }

        public KeyValuePair Peek()
        {
            return heap.Peek();
        }

        public KeyValuePair Remove()
        {
            return heap.Remove();
        }

        public void Clear()
        {
            heap.Clear();
        }

        public KeyValuePair[] ToArray()
        {
            return heap.ToArray();
        }

        public System.Collections.Generic.IEnumerator<KeyValuePair> GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        // System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        // {
        //     return null;
        // }

        public KeyValuePair MinValue()
        {
            return heap.MinValue();
        }

        public KeyValuePair TakeMin()
        {
            return heap.TakeMin();
        }

        public KeyValuePair MaxValue()
        {
            return heap.MaxValue();
        }

        public KeyValuePair TakeMax()
        {
            return heap.TakeMax();
        }

        public void ToCollection(KeyValuePair[] collection)
        {
            heap.ToCollection(collection);
        }
    }
}