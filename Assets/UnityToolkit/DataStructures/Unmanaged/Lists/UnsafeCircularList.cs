using System.Collections;
using System.Runtime.InteropServices;
using GalacticBoundStudios.DataScribes.Managed.Lists;
using System;

namespace ProjectWorlds.DataStructures.Unsafe.Lists
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UnsafeCircularList<T> : IDisposable, IListExtended<T>, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T> where T : unmanaged
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : System.Collections.Generic.IEnumerator<T>
        {
            private UnsafeCircularList<T> list;
            private int index;
            private T current;

            public Enumerator(UnsafeCircularList<T> list)
            {
                this.list = list;
                index = -1;
                current = default;
            }

            public T Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {
                list = default;
                current = default;
            }

            public bool MoveNext()
            {
                if (index < list.count - 1)
                {
                    index++;
                    current = list.buffer[(list.head + index) % list.buffer.Length];
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default;
            }
        }

        private UnsafeArray<T> buffer;
        private int head;
        private int tail;
        private int count;

        public bool IsFull => count == buffer.Length;
        public T this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }
        public int Count => count;
        public bool IsReadOnly => false;
        public bool IsEmpty => count == 0;

        public UnsafeCircularList(int capacity)
        {
            buffer = new UnsafeArray<T>(capacity);
            head = 0;
            tail = 0;
            count = 0;
        }

        public void Add(T item)
        {
            buffer[tail] = item;
            tail = (tail + 1) % buffer.Length;

            if (!IsFull) {
                count++;
            } else {
                head = (head + 1) % buffer.Length;
            }
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            buffer[(head + index) % buffer.Length] = value;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            return buffer[(head + index) % buffer.Length];
        }

        public void AddRange(System.Collections.Generic.IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Add(item);
            }
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (!IsFull)
            {
                for (int i = Count; i > index; i--)
                {
                    buffer[(i + head) % buffer.Length] = buffer[(i + head - 1 + buffer.Length) % buffer.Length];
                }

                buffer[(head + index) % buffer.Length] = item;
                count++;
            }
            else
            {
                for (int i = Count - 1; i > index; i--)
                {
                    buffer[(i + head) % buffer.Length] = buffer[(i + head - 1 + buffer.Length) % buffer.Length];
                }

                buffer[(head + index) % buffer.Length] = item;
            }
        }

        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                Insert(index++, item);
            }
        }

        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range, int rangeCount)
        {
            foreach (T item in range)
            {
                Insert(index++, item);
                if (--rangeCount == 0)
                {
                    break;
                }
            }
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (length < 0 || index + length > count)
            {
                throw new System.ArgumentOutOfRangeException("Length out of range");
            }

            if (index == 0 && length == count)
            {
                count = 0;
                head = 0;
                tail = 0;
                return;
            }

            int i = index;
            int j = index + length;

            while (j < count)
            {
                buffer[(head + i) % buffer.Length] = buffer[(head + j) % buffer.Length];
                i++;
                j++;
            }

            count -= length;
            tail = (tail - length + buffer.Length) % buffer.Length;
        }

        public T First()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            return buffer[head];
        }

        public T TakeFirst()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            T item = buffer[head];
            head = (head + 1) % buffer.Length;
            count--;

            return item;
        }

        public void RemoveFirst()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            head = (head + 1) % buffer.Length;
            count--;
        }

        public T Last()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            return buffer[(tail - 1 + buffer.Length) % buffer.Length];
        }

        public T TakeLast()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            tail = (tail - 1 + buffer.Length) % buffer.Length;
            T item = buffer[tail];
            count--;

            return item;
        }

        public void RemoveLast()
        {
            if (count == 0)
            {
                throw new System.InvalidOperationException("List is empty");
            }

            tail = (tail - 1 + buffer.Length) % buffer.Length;
            count--;
        }

        public bool Remove(T item)
        {
            return RemoveFirst(item);
        }

        public bool RemoveFirst(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
            }
            return index != -1;
        }

        public bool RemoveLast(T item)
        {
            int index = LastIndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
            }
            return index != -1;
        }

        public void RemoveAll(T item)
        {
            int i = 0;
            int j = 0;

            while (j < count)
            {
                if (!buffer[(head + j) % buffer.Length].Equals(item))
                {
                    buffer[(head + i) % buffer.Length] = buffer[(head + j) % buffer.Length];
                    i++;
                }

                j++;
            }

            count = i;
            tail = (head + count) % buffer.Length;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (index == 0)
            {
                RemoveFirst();
                return;
            }

            if (index == count - 1)
            {
                RemoveLast();
                return;
            }

            int i = index;
            int j = index + 1;

            while (j < count)
            {
                buffer[(head + i) % buffer.Length] = buffer[(head + j) % buffer.Length];
                i++;
                j++;
            }

            count--;
            tail = (tail - 1 + buffer.Length) % buffer.Length;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[(head + i) % buffer.Length].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(T item)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (buffer[(head + i) % buffer.Length].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Clear()
        {
            count = 0;
            head = 0;
            tail = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override string ToString()
        {
            string result = "{ ";

            foreach (T item in this) {
                result += item.ToString() + ", ";
            }

            if (IsEmpty) {
                result += "}";
            } else {
                result = result.Remove(result.Length - 2);
                result += " }";
            }

            return result;
        }

        public void Dispose()
        {
            buffer.Dispose();
            head = 0;
            tail = 0;
            count = 0;
        }
    }
}