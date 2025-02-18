using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    public class CircularList<T> : IListExtended<T>, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>
    {
        protected class CircularListEnumerator : IEnumerator<T>
        {
            private CircularList<T> list = null;
            private int index = -1;

            public CircularListEnumerator(CircularList<T> list)
            {
                this.list = list;
            }

            public T Current { get { return list.Get(index); } }

            object IEnumerator.Current { get { return list.Get(index); } }

            public void Dispose()
            {
                list = null;
            }

            public bool MoveNext()
            {
                index++;
                return index < list.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        private T[] buffer = null;
        private int head = 0;
        private int tail = 0;
        private int count = 0;

        public bool IsReadOnly { get { return false; } }
        public int Count { get { return count; } }

        public bool IsEmpty { get { return count == 0; } }
        public bool IsFull { get { return count == buffer.Length; } }

        public T this[int index] { get { return Get(index); } set { Set(index, value); } }

        public CircularList()
        {
            buffer = new T[16];
        }
        public CircularList(int capacity)
        {
            buffer = new T[capacity];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CircularListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CircularListEnumerator(this);
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            buffer[(head + index) % buffer.Length] = value;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            int i = (head + index) % buffer.Length;
            return buffer[i];
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range) {
                Add(item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> range)
        {
            foreach (T item in range) {
                Insert(index++, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> range, int length)
        {
            foreach (T item in range) {
                Insert(index++, item);
                if (--length == 0) {
                    break;
                }
            }
        }

        public T TakeFirst()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            T item = buffer[head];
            head = (head + 1) % buffer.Length;
            count--;

            return item;
        }

        public T TakeLast()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            tail = (tail - 1 + buffer.Length) % buffer.Length;
            T item = buffer[tail];
            count--;

            return item;
        }

        public void RemoveFirst()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            head = (head + 1) % buffer.Length;
            count--;
        }

        public void RemoveLast()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            tail = (tail - 1 + buffer.Length) % buffer.Length;
            count--;
        }

        public T First()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            return buffer[head];
        }

        public T Last()
        {
            if (count == 0) {
                throw new System.InvalidOperationException("List is empty");
            }

            return buffer[(tail - 1 + buffer.Length) % buffer.Length];
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (length < 0 || (index + length) > count) {
                throw new System.ArgumentOutOfRangeException("Length out of range");
            }

            if (index == 0) {
                head = (head + length) % buffer.Length;
            } else if ((index + length) == count) {
                tail = (tail - length + buffer.Length) % buffer.Length;
            } else {
                int i = (head + index) % buffer.Length;
                int j = (i + length) % buffer.Length;

                if (i < j) {
                    System.Array.Copy(buffer, j, buffer, i, buffer.Length - j);
                } else {
                    System.Array.Copy(buffer, j, buffer, i, buffer.Length - j);
                    System.Array.Copy(buffer, 0, buffer, buffer.Length - j, j);
                }
            }

            count -= length;
        }

        public void RemoveAll(T value)
        {
            for (int i = 0; i < count; i++) {
                if (buffer[(head + i) % buffer.Length].Equals(value)) {
                    RemoveAt(i);
                    i--;
                }
            }
        }

        public bool RemoveFirst(T value)
        {
            int i = head;
            int j = 0;

            while (j < count) {
                if (buffer[i].Equals(value)) {
                    RemoveRange(j, 1);
                    return true;
                }

                i = (i + 1) % buffer.Length;
                j++;
            }

            return false;
        }

        public bool RemoveLast(T value)
        {
            int i = (tail - 1 + buffer.Length) % buffer.Length;
            int j = count - 1;

            while (j >= 0) {
                if (buffer[i].Equals(value)) {
                    RemoveRange(j, 1);
                    return true;
                }

                i = (i - 1 + buffer.Length) % buffer.Length;
                j--;
            }

            return false;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < count; i++) {
                if (buffer[(head + i) % buffer.Length].Equals(item)) {
                    return i;
                }
            }

            return -1;
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

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (index == 0) {
                head = (head + 1) % buffer.Length;
            } else if (index == count - 1) {
                tail = (tail - 1 + buffer.Length) % buffer.Length;
            } else {
                for (int i = index; i < count - 1; i++) {
                    buffer[(head + i) % buffer.Length] = buffer[(head + i + 1) % buffer.Length];
                }
                buffer[(tail - 1 + buffer.Length) % buffer.Length] = default(T);
            }
            count--;
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

        public void Clear()
        {
            head = 0;
            tail = 0;
            count = 0;

            // Clear the buffer
            Array.Clear(buffer, 0, buffer.Length);
        }

        public bool Contains(T item)
        {
            foreach (T value in buffer) {
                if (value.Equals(item)) {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) {
                throw new System.ArgumentNullException("Array is null");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length) {
                throw new System.ArgumentOutOfRangeException("Array index out of range");
            }

            if ((array.Length - arrayIndex) < count) {
                throw new System.ArgumentException("Array is too small");
            }

            for (int i = 0; i < count; i++) {
                array[arrayIndex + i] = buffer[(head + i) % buffer.Length];
            }
        }

        public bool Remove(T item)
        {
            int i = head;
            int j = 0;

            while (j < count) {
                if (buffer[i].Equals(item)) {
                    RemoveRange(j, 1);
                    return true;
                }

                i = (i + 1) % buffer.Length;
                j++;
            }

            return false;
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
    }
}