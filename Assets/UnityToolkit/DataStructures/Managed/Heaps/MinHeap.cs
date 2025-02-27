namespace GalacticBoundStudios.DataScribes.Managed.Heaps
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


    public class MinHeap<TKey, TValue> where TKey : System.IComparable<TKey>
    {
        protected MinHeap<KeyValuePair> heap;

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

        public MinHeap()
        {
            heap = new MinHeap<KeyValuePair>();
        }

        public MinHeap(int capacity)
        {
            heap = new MinHeap<KeyValuePair>(capacity);
        }

        public MinHeap(KeyValuePair[] elements)
        {
            heap = new MinHeap<KeyValuePair>(elements);
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