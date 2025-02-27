using System.Collections.Generic;

namespace GalacticBoundStudios.DataScribes.Managed.Lists
{
    public class OrderedList<T> : ArrayList<T> where T : System.IComparable<T>
    {
        // Inserts an item into the list using a binary search to find
        // the correct position for the item
        public override void Add(T item)
        {
            // Make sure there is room for the new item
            EnsureCapacity(count + 1);

            // Find the index where the item should be inserted
            int index = FindIndex(0, count - 1, item);

            // Make sure this is the last instance of the item
            while (index < count && buffer[index].CompareTo(item) == 0) {
                index++;
            }

            // Shift all items from the insertion point to the end of the list
            for (int i = count; i > index; i--) {
                buffer[i] = buffer[i - 1];
            }

            // Add the item to the end of the list
            buffer[index] = item;
            count++;
        }

        public override void Set(int index, T value)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Remove the item at the index
            RemoveAt(index);
            // Ignore the index because it might not be correct
            Add(value);
        }

        public override void Insert(int index, T item)
        {
            if (index < 0 || index > count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Ignore the index because it might not be correct
            Add(item);
        }

        public override void RemoveAll(T value)
        {
            // Start by finding the first instance of the value
            int index = FindIndex(0, count - 1, value);

            // If index == count, then the value is not in the list
            if (index == count || buffer[index].CompareTo(value) != 0) {
                return;
            }

            // Make sure we have the start index
            while (index > 0 && buffer[index - 1].CompareTo(value) == 0) {
                index--;
            }

            // Find the total number of instances of the value
            int total = 1;
            for (int i = index + 1; i < count; i++) {
                if (buffer[i].CompareTo(value) == 0) {
                    total++;
                }
            }

            // Remove the range of values
            RemoveRange(index, total);
        }

        public override bool RemoveFirst(T value)
        {
            // Find the first instance of the value
            for (int i = 0; i < count; i++) {
                int comp = buffer[i].CompareTo(value);

                // If 0, then we found the value
                if (comp == 0) {
                    RemoveAt(i);
                    return true;
                }
                // If greater than 0, then the value is not in the list
                else if (comp > 0) {
                    return false;
                }
            }

            return false;
        }

        public override bool RemoveLast(T value)
        {
            for (int i = count - 1; i >= 0; i--) {
                int comp = buffer[i].CompareTo(value);

                // If 0, then we found the value
                if (comp == 0) {
                    RemoveAt(i);
                    return true;
                }
                // If less than 0, then the value is not in the list
                else if (comp < 0) {
                    return false;
                }
            }

            return false;
        }

        // Find the first instance of the value
        protected int FindIndex(int low, int high, T value)
        {
            while (low <= high) {
                int mid = (low + high) / 2;
                int comp = buffer[mid].CompareTo(value);

                if (comp == 0) {
                    return mid;
                } else if (comp < 0) {
                    low = mid + 1;
                } else {
                    high = mid - 1;
                }
            }
            return low;
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

    public struct ComparablePair<TKey, TValue> : System.IComparable<ComparablePair<TKey,TValue>> where TKey : System.IComparable<TKey>
    {
        public TKey Key;
        public TValue Value;

        public ComparablePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public int CompareTo(ComparablePair<TKey,TValue> other)
        {
            return Key.CompareTo(other.Key);
        }

        public KeyValuePair<TKey, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(Key, Value);
        }

        public override string ToString()
        {
            return "<" + Key.ToString() + "," + Value.ToString() + ">";
        }
    }

    public class OrderedList<TKey, TValue> : OrderedList<ComparablePair<TKey,TValue>> where TKey : System.IComparable<TKey>
    {
        public KeyValuePair<TKey,TValue> GetAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            return buffer[index].ToKeyValuePair();
        }

        public void Add(TKey key, TValue value)
        {
            Add(new ComparablePair<TKey, TValue>(key, value));
        }

        public void Set(int index, TKey key, TValue value)
        {
            Set(index, new ComparablePair<TKey, TValue>(key, value));
        }

        public void Insert(int index, TKey key, TValue value)
        {
            Insert(index, new ComparablePair<TKey, TValue>(key, value));
        }

        public void RemoveAll(TKey key, TValue value)
        {
            RemoveAll(new ComparablePair<TKey, TValue>(key, value));
        }

        public bool RemoveFirst(TKey key, TValue value)
        {
            return RemoveFirst(new ComparablePair<TKey, TValue>(key, value));
        }

        public bool RemoveLast(TKey key, TValue value)
        {
            return RemoveLast(new ComparablePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key, TValue value)
        {
            return Remove(new ComparablePair<TKey, TValue>(key, value));
        }

        public bool Contains(TKey key, TValue value)
        {
            return Contains(new ComparablePair<TKey, TValue>(key, value));
        }

        public TValue Get(TKey key)
        {
            for (int i = 0; i < count; i++) {
                int comp = buffer[i].Key.CompareTo(key);
                // If less than, continue
                if (comp < 0) {
                    continue;
                }
                // If equal to, return value
                else if (comp == 0) {
                    return buffer[i].Value;
                } 
                // If greater than, value not in list
                else {
                    break;
                }
            }

            throw new System.ArgumentException("Key not found");
        }

        public int IndexOf(TKey key, TValue value)
        {
            return IndexOf(new ComparablePair<TKey, TValue>(key, value));
        }

        public override string ToString()
        {
            string result = "{ ";

            foreach (ComparablePair<TKey, TValue> item in this) {
                result += "<" + item.Key.ToString() + "," + item.Value.ToString() + ">, ";
            }

            if (IsEmpty) {
                result += "}";
            } else {
                result = result.Remove(result.Length - 2);
                result += " }";
            }

            return result;
        }

        public bool ContainsKey(TKey key)
        {
            // Use a binary search to find the key
            int start = 0;
            int end = count - 1;
            while (start <= end) {
                int mid = (start + end) / 2;
                int comp = buffer[mid].Key.CompareTo(key);

                if (comp == 0) {
                    return true;
                } else if (comp < 0) {
                    start = mid + 1;
                } else {
                    end = mid - 1;
                }
            }
            // Failed to find key
            return false;
        }

        public bool ContainsValue(TValue value)
        {
            // Use a linear search to find the value
            for (int i = 0; i < count; i++) {
                if (buffer[i].Value.Equals(value)) {
                    return true;
                }
            }
            // Failed to find value
            return false;
        }

        public bool RemoveKey(TKey key)
        {
            // Use a binary search to find the key
            int start = 0;
            int end = count - 1;
            while (start <= end) {
                int mid = (start + end) / 2;
                int comp = buffer[mid].Key.CompareTo(key);

                if (comp == 0) {
                    // Make sure this is the first instance of the key
                    while (mid > 0 && buffer[mid - 1].Key.CompareTo(key) == 0) {
                        mid--;
                    }
                    RemoveAt(mid);
                    return true;
                } else if (comp < 0) {
                    start = mid + 1;
                } else {
                    end = mid - 1;
                }
            }
            // Failed to find key
            return false;
        }

        // Returns a list of all items that have the given key
        public ArrayList<TValue> GetValues(TKey key)
        {
            ArrayList<TValue> values = new ArrayList<TValue>();

            // Use a binary search to find the key
            int start = 0;
            int end = count - 1;
            while (start <= end) {
                int mid = (start + end) / 2;
                int comp = buffer[mid].Key.CompareTo(key);

                if (comp == 0) {
                    // Make sure this is the first instance of the key
                    while (mid > 0 && buffer[mid - 1].Key.CompareTo(key) == 0) {
                        mid--;
                    }
                    // Add all items with the key
                    while (mid < count && buffer[mid].Key.CompareTo(key) == 0) {
                        values.Add(buffer[mid].Value);
                        mid++;
                    }
                    return values;
                } else if (comp < 0) {
                    start = mid + 1;
                } else {
                    end = mid - 1;
                }
            }
            // Failed to find key
            return values;
        }
    }
}