using System;
using System.Collections;
using System.Collections.Generic;

namespace GalacticBoundStudios.DataScribes.Managed.Lists
{
    public class ArrayList<T> : IListExtended<T>, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>
    {
        // Enumerator for the list
        protected class ArrayListEnumerator : IEnumerator<T>
        {
            // Reference to the list
            protected ArrayList<T> list = null;
            // Current index of the enumerator
            protected int currentIndex = -1;

            public ArrayListEnumerator(ArrayList<T> list)
            {
                if (list == null) {
                    throw new ArgumentNullException("List cannot be null");
                }

                this.list = list;
            }

            public T Current { get { return list.Get(currentIndex); } }

            object IEnumerator.Current { get { return list.Get(currentIndex); } }

            public void Dispose()
            {
                list = null;
            }

            public bool MoveNext()
            {
                currentIndex++;
                return currentIndex < list.Count;
            }

            public void Reset()
            {
                currentIndex = -1;
            }
        }

        // Buffer used to hold all of the list items
        protected T[] buffer = null;
        // Tracks the number of items in the list
        protected int count = 0;

        // Operator overload for accessing the list
        public T this[int index] { get { return Get(index); } set { Set(index, value); } }

        public int Count {  get { return count; } }

        public bool IsReadOnly { get { return false; } }

        public bool IsEmpty { get { return count == 0; } }

        public ArrayList()
        {
            buffer = new T[1];
        }

        public ArrayList(int capacity)
        {
            buffer = new T[capacity];
        }

        public ArrayList(IEnumerable<T> collection)
        {
            buffer = new T[1];
            AddRange(collection);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ArrayListEnumerator(this);
        }

        public virtual void Add(T item)
        {
            // Make sure there is room for the new item
            EnsureCapacity(count + 1);

            // Add the item to the end of the list
            buffer[count++] = item;
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            // Clear the buffer
            buffer = new T[1];
            count = 0;
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

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = buffer[i];
            }
        }

        public T First()
        {
            return Get(0);
        }

        public void RemoveFirst()
        {
            RemoveAt(0);
        }

        public T TakeFirst()
        {
            T item = Get(0);
            RemoveAt(0);
            return item;
        }

        public T Last()
        {
            return Get(count - 1);
        }

        public void RemoveLast()
        {
            RemoveAt(count - 1);
        }

        public T TakeLast()
        {
            T item = Get(count - 1);
            RemoveAt(count - 1);
            return item;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            return buffer[index];
        }

        public virtual void Set(int index, T value)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            buffer[index] = value;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual void Insert(int index, T item)
        {
            // If the index is out of range, throw an exception
            if (index < 0 || index > count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Make sure there is room for the new item
            EnsureCapacity(count + 1);

            // Shift all items after the index to the right
            for (int i = count; i > index; i--)
            {
                buffer[i] = buffer[i - 1];
            }

            // Insert the new item
            buffer[index] = item;
            count++;
        }

        public void InsertRange(int index, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                Insert(index++, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> range, int length)
        {
            // Make sure there is room for the new items
            EnsureCapacity(count + length);

            // Insert the items
            foreach (T item in range)
            {
                Insert(index++, item);

                if (--length == 0)
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

        public virtual void RemoveAll(T value)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(value))
                {
                    RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveAt(int index)
        {
            // If the index is out of range, throw an exception
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Shift all items after the index to the left
            for (int i = index; i < count - 1; i++)
            {
                buffer[i] = buffer[i + 1];
            }

            // Clear the last item
            buffer[--count] = default;
        }

        public virtual bool RemoveFirst(T value)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(value))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public virtual bool RemoveLast(T value)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (buffer[i].Equals(value))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveRange(int index, int length)
        {
            // If the index is out of range, throw an exception
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // If the length is out of range, throw an exception
            if (length < 0 || (index + length) > count)
            {
                throw new System.ArgumentOutOfRangeException("Length out of range");
            }

            // Shift all items after the range to the left
            for (int i = index; i < count - length; i++)
            {
                buffer[i] = buffer[i + length];
            }

            // Clear the last items
            for (int i = count - length; i < count; i++)
            {
                buffer[i] = default;
            }

            count -= length;
        }

        protected void EnsureCapacity(int minCapacity)
        {
            if (buffer.Length < minCapacity)
            {
                // Determine the new capacity. Ensure that the new capacity does not grow too much at the top end but also ensure that the new capacity is at least the minimum required
                int newCapacity;
                do {
                    // Determine the new capacity. Ensure that the new capacity does not grow too much at the top end
                    newCapacity = buffer.Length < 512 ? buffer.Length * 2 : buffer.Length + 512;
                } while (newCapacity < minCapacity);
                
                // Create a new buffer with the new capacity
                T[] newBuffer = new T[newCapacity];
                // Copy the old buffer to the new buffer
                Array.Copy(buffer, newBuffer, count);
                // Set the new buffer as the buffer
                buffer = newBuffer;
            }
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