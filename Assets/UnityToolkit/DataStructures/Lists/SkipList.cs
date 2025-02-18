using System;
using Random = System.Random;

namespace ProjectWorlds.DataStructures.Lists
{
    public class SkipList<T> : IListExtended<T> where T : IComparable<T>
    {
        protected class SkipListEnumerator : System.Collections.Generic.IEnumerator<T>
        {
            private SkipList<T> list;
            private SkipList<T>.Node current;

            public SkipListEnumerator(SkipList<T> list)
            {
                this.list = list;
                current = list.head;
                while (current.Down != null) {
                    current = current.Down;
                }
            }

            public T Current => current.Value;

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {
                list = null;
                current = null;
            }

            public bool MoveNext()
            {
                if (current.Next == null) {
                    return false;
                }

                current = current.Next;
                return true;
            }

            public void Reset()
            {
                current = list.head;
                while (current.Down != null) {
                    current = current.Down;
                }
            }
        }

        protected class Node : IComparable<Node>
        {
            public T Value { get; set; }
            public Node Down { get; set; }
            public Node Next { get; set; }
            public int Level { get; set; }
            public bool IsHead { get; set; }

            public Node(int level, T value, Node down = null, Node next = null)
            {
                Level = level;
                Value = value;
                Down = down;
                Next = next;
            }

            public int CompareTo(Node other)
            {
                return Value.CompareTo(other.Value);
            }
        }

        // The first node at the maximum level of the skip list
        private Node head = null;

        // The maximum level of the skip list
        private int maxLevel = 0;

        // The probability of a node having a level of i + 1 is 1 / p
        private int p = 4;

        // The number of elements in the skip list
        private int count = 0;

        // Random number generator used when adding nodes to the skip list
        private Random random = new Random();

        public int Count => count;

        public bool IsReadOnly => false;

        public bool IsEmpty { get { return count == 0; } }

        public SkipList()
        {
            // Create the head node of the skip list
            head = new Node(0, default(T));
            head.IsHead = true;
        }

        public void Add(T value)
        {
            // Determine the level of the new node
            int level = 1;
            while (level < (maxLevel + 1) && random.Next(1, p) == 1) {
                level++;
            }

            // If the new node has a level greater than the maximum level of the skip list
            // then create a new head node with the new level
            if (level > maxLevel) {
                maxLevel = level;

                Node newHead = new Node(level, default(T));
                newHead.IsHead = true;
                newHead.Down = head;
                head = newHead;
            }

            // Create the new node
            Node newNode = new Node(level, value);

            // Insert the new node into the skip list
            Node current = head;
            while (current.Level != level) {
                current = current.Down;
            }

            while (current != null) {
                // Find the correct position to insert the new node in the current level
                while (current.Next != null && current.Next.CompareTo(newNode) < 0) {
                    current = current.Next;
                }

                // Insert the new node
                newNode.Next = current.Next;

                current.Next = newNode;

                // Move down one level
                current = current.Down;
            }

            count++;
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= count) {
                throw new IndexOutOfRangeException();
            }

            // Remove the node at the given index
            RemoveAt(index);
            // Add the new value because the given index is probably wrong
            Add(value);
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count) {
                throw new IndexOutOfRangeException();
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Move to the index-th node
            for (int i = 0; i <= index; i++) {
                current = current.Next;
            }

            // Return the value of the index-th node
            return current.Value;
        }

        public void AddRange(System.Collections.Generic.IEnumerable<T> range)
        {
            foreach (T value in range) {
                Add(value);
            }
        }

        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range)
        {
            if (index < 0 || index > count) {
                throw new IndexOutOfRangeException();
            }

            foreach (T value in range) {
                Add(value);
            }
        }

        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range, int length)
        {
            if (index < 0 || index >= count) {
                throw new IndexOutOfRangeException();
            }

            int i = 0;
            foreach (T value in range) {
                if (i == length) {
                    break;
                }

                Add(value);
                i++;
            }
        }

        public T TakeFirst()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            T value = First();
            RemoveFirst();
            return value;
        }

        public T TakeLast()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            T value = Last();
            RemoveLast();
            return value;
        }

        public void RemoveFirst()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Track the first node
            Node first = current.Next;

            // Remove all instances of the first node
            current = head;
            while (current != null) {
                // If there is a next node and it contains the same value as the first node
                // then if it in the same column, remove it
                if (current.Next != null && 
                    current.Next.CompareTo(first) == 0 &&
                    IsInSameColumn(current.Next, first)) {
                    // Remove node
                    current.Next = current.Next.Next;
                }
                // Move down to the next row
                current = current.Down;
            }

            count--;
        }

        protected bool IsInSameColumn(Node node1, Node node2)
        {
            while (node1.Down != null) {
                node1 = node1.Down;
            }

            while (node2.Down != null) {
                node2 = node2.Down;
            }

            return node1 == node2;
        }

        public void RemoveLast()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Move to the second last node
            while (current.Next.Next != null) {
                current = current.Next;
            }

            // Remove the last node
            current.Next = null;
            count--;
        }

        public T First()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            return current.Next.Value;
        }

        public T Last()
        {
            if (IsEmpty) {
                throw new InvalidOperationException("The skip list is empty");
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Move to the last node
            while (current.Next != null) {
                current = current.Next;
            }

            return current.Value;
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || index >= count) {
                throw new IndexOutOfRangeException();
            }

            if (length < 0) {
                throw new ArgumentException("The length must be greater than or equal to zero");
            }

            if (index + length > count) {
                throw new ArgumentException("The sum of the index and length must be less than or equal to the count");
            }

            // Find the first node in the range to remove
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Find the node before the first to remove
            int i;
            for (i = 0; i < index; i++) {
                current = current.Next;
            }

            // Keep track of the first node to remove
            Node first = current.Next;

            // Clear the range of nodes
            for (i = 0; i < length; i++) {
                Node temp = current.Next;
                current.Next = current.Next.Next;
                temp.Next = null;
            }

            Node last = current.Next;

            current = head;
            while (current != null) {
                // Find the node before the first node to remove
                while (current.Next != null && current.Next.CompareTo(first) < 0) {
                    current = current.Next;
                }

                if (last != null) {
                    // Remove all nodes in the range
                    while (current.Next != null && current.Next.CompareTo(last) <= 0) {
                        current.Next = current.Next.Next;
                    }
                } else {
                    // Remove all nodes after the first node to remove
                    while (current.Next != null) {
                        current.Next = current.Next.Next;
                    }
                }

                current = current.Down;
            }

            count -= length;
        }

        public void RemoveAll(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            // Find the first node with the value
            while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                current = current.Next;
            }

            while (current != null) {
                // If the next node contains the value, remove it
                if (current.Next != null && current.Next.Value.CompareTo(value) == 0) {
                    current.Next = current.Next.Next;

                    if (current.Down == null) {
                        count--;
                    }

                } else if (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                } else {
                    current = current.Down;
                }
            }
        }

        public bool RemoveFirst(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            while (true) {
                // Move down row until the next node is greater than or equal to value
                while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                }

                if (current.Down == null) {
                    break;
                } else {
                    current = current.Down;
                }
            }

/*            // Find the node to remove
            while (current.Down != null) {
                while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                }
                current = current.Down;
            }*/

            if (current.Next == null || current.Next.Value.CompareTo(value) != 0) {
                return false;
            }

            Node toRemove = current.Next;

            // Remove all instances of the node to remove
            current = head;
            while (current != null) {
                // Find the node before the node to remove
                while (current.Next != null && (current.Next.CompareTo(toRemove) < 0 || (current.Next.CompareTo(toRemove) == 0 && !IsInSameColumn(current.Next, toRemove)))) {
                    current = current.Next;
                }

                // If the next node is the node to remove, remove it
                if (current.Next != null && current.Next.CompareTo(toRemove) == 0 && IsInSameColumn(current.Next, toRemove)) {
                    current.Next = current.Next.Next;
                }

                current = current.Down;
            }

            count--;

            return true;
        }

        public bool RemoveLast(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            // Find the first node with the value
            while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                current = current.Next;
            }

            Node last = null;
            while (current != null) {
                // If the next node contains the value, remove it
                if (current.Next != null && current.Next.Value.CompareTo(value) == 0) {
                    last = current;
                }

                if (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                } else {
                    current = current.Down;
                }
            }

            if (last != null) {
                last.Next = last.Next.Next;
                count--;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            head = new Node(0, default(T));
            head.IsHead = true;
            maxLevel = 0;
            count = 0;
        }

        public bool Contains(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            // Find the first node with the value
            while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                current = current.Next;
            }

            while (current != null) {
                // If the next node contains the value, return true
                if (current.Next != null && current.Next.Value.CompareTo(value) == 0) {
                    return true;
                } else if (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                } else {
                    current = current.Down;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) {
                throw new ArgumentNullException("The array cannot be null");
            }

            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("The array index cannot be negative");
            }

            if (array.Length - arrayIndex < count) {
                throw new ArgumentException("The array is not large enough to hold the elements");
            }

            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            for (int i = 0; i < count; i++) {
                array[arrayIndex + i] = current.Next.Value;
                current = current.Next;
            }
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new SkipListEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            // Move to bottom level
            while (current.Down != null) {
                current = current.Down;
            }

            int index = 0;
            while (current != null) {
                // If the next node contains the value, return the index
                if (current.Next != null && current.Next.Value.CompareTo(value) == 0) {
                    return index;
                } else if (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                    index++;
                } else {
                    current = current.Down;
                }
            }

            return -1;
        }

        public void Insert(int index, T value)
        {
            if (index < 0 || index > count) {
                throw new IndexOutOfRangeException();
            }

            Add(value);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) {
                throw new IndexOutOfRangeException();
            }

            // Move to the bottom level of the skip list
            Node current = head;
            while (current.Down != null) {
                current = current.Down;
            }

            // Move to the index-th node
            for (int i = 0; i <= index; i++) {
                current = current.Next;
            }

            Node toRemove = current;

            // Remove all nodes in the same column as the node to remove
            current = head;
            while (current != null) {
                // Find the node before the node to remove
                while (current.Next != null && (current.Next.CompareTo(toRemove) < 0 || (current.Next.CompareTo(toRemove) == 0 && !IsInSameColumn(current.Next, toRemove)))) {
                    current = current.Next;
                }

                // If the next node is the node to remove, remove it
                if (current.Next != null && current.Next.CompareTo(toRemove) == 0 && IsInSameColumn(current.Next, toRemove)) {
                    current.Next = current.Next.Next;
                }

                current = current.Down;
            }
            count--;
        }

        public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public bool Remove(T value)
        {
            // Move to the bottom level of the skip list
            Node current = head;

            // Find the first node with the value
            Node toRemove = null;
            while (current != null) {
                while (current.Next != null && current.Next.Value.CompareTo(value) < 0) {
                    current = current.Next;
                }

                if (current.Next != null && current.Next.Value.CompareTo(value) == 0) {
                    toRemove = current.Next;
                    while (toRemove.Down != null) {
                        toRemove = toRemove.Down;
                    }
                    current = null;
                } else {
                    current = current.Down;
                }
            }

            if (toRemove == null) {
                return false;
            }

            // Remove all nodes in the same column as the node to remove
            current = head;
            while (current != null) {
                // Find the node before the node to remove
                while (current.Next != null && (current.Next.CompareTo(toRemove) < 0 || (current.Next.CompareTo(toRemove) == 0 && !IsInSameColumn(current.Next, toRemove)))) {
                    current = current.Next;
                }

                // If the next node is the node to remove, remove it
                if (current.Next != null && current.Next.CompareTo(toRemove) == 0 && IsInSameColumn(current.Next, toRemove)) {
                    current.Next = current.Next.Next;
                }

                current = current.Down;
            }

            count--;
            return true;
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