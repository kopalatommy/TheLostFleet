using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Unsafe.Lists
{
    // [BurstCompile]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnsafeArrayList<T> : IDisposable, IListExtended<T>, ICollection<T>, IEnumerable<T> where T : unmanaged
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Enumerator : IEnumerator<T>
        {
            private UnsafeArrayList<T> list;
            private int index;
            private T current;

            public Enumerator(UnsafeArrayList<T> list)
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
                    current = list.array[index];
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

        private UnsafeArray<T> array;
        private int count;

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

        public UnsafeArrayList(int capacity = 1)
        {
            array = new UnsafeArray<T>(capacity);
            count = 0;
        }

        public void Add(T item)
        {
            EnsureCapacity(count + 1);
            
            array[count++] = item;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (array[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = this.array[i];
            }
        }

        public T First()
        {
            return this[0];
        }

        public void RemoveFirst()
        {
            RemoveAt(0);
        }

        public T TakeFirst()
        {
            T item = First();
            RemoveFirst();
            return item;
        }

        public T Last()
        {
            return this[count - 1];
        }

        public void RemoveLast()
        {
            RemoveAt(count - 1);
        }

        public T TakeLast()
        {
            T item = Last();
            RemoveLast();
            return item;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            return array[index];
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            array[index] = value;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (array[i].Equals(item))
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
                if (array[i].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            EnsureCapacity(count + 1);

            // Shift all items after the index to the right
            for (int i = count; i > index; i--)
            {
                array[i] = array[i - 1];
            }

            // Insert the new item
            array[index] = item;
            count++;
        }

        public void InsertRange(int index, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                Insert(index++, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> range, int rangeCount)
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

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        // Removes all occurrences of the item from the list. Returns the number
        // of items removed.
        public void RemoveAll(T value)
        {
            int index = 0;

            while (index < count)
            {
                if (array[index].Equals(value))
                {
                    RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Shift all items after the index to the left
            for (int i = index; i < count - 1; i++)
            {
                array[i] = array[i + 1];
            }

            // Clear the last item
            array[count - 1] = default;
            count--;
        }

        public  bool RemoveFirst(T value)
        {
            int index = IndexOf(value);

            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public  bool RemoveLast(T value)
        {
            int index = LastIndexOf(value);

            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
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

            // Shift all items after the range to the left
            for (int i = index; i < count - length; i++)
            {
                array[i] = array[i + length];
            }

            // Clear the last items
            for (int i = count - length; i < count; i++)
            {
                array[i] = default;
            }

            count -= length;
        }


        private void EnsureCapacity(int min)
        {
            if (array.Length < min)
            {
                // Determine the new capacity. Ensure that the new capacity does not grow too much at the top end but also ensure that the new capacity is at least the minimum required
                int newCapacity = array.Length;
                if (newCapacity < 1) {
                    newCapacity = 1;
                }
                do {
                    // Determine the new capacity. Ensure that the new capacity does not grow too much at the top end
                    newCapacity = newCapacity < 512 ? newCapacity << 2 : array.Length + 512;
                } while (newCapacity < min);

                // Resize the buffer
                UnsafeArray<T>.Resize(ref array, newCapacity);
            }
        }

        public IEnumerator<T> GetEnumerator()
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
            array.Dispose();
            count = 0;
        }
    }
}