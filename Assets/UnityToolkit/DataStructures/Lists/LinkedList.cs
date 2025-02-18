using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    // A doubly linked list
    public class LinkedList<T> : IListExtended<T>, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>
    {
        // Node class for the linked list
        protected class Node
        {
            // The value of the node
            public T Value { get; set; }
            // Reference to the next node in the list
            public Node Next { get; set; }
            // Reference to the previous node in the list
            public Node Previous { get; set; }

            public Node(T value, Node next = null, Node previous = null)
            {
                Value = value;
                Next = next;
                Previous = previous;
            }
        }

        // Enumerator for the linked list
        protected class LinkedListEnumerator : System.Collections.Generic.IEnumerator<T>
        {
            // Current node in the list
            protected Node current = null;

            public T Current { get { return current.Value; } }

            object System.Collections.IEnumerator.Current { get { return current.Value; } }

            public LinkedListEnumerator(Node head)
            {
                current = new Node(default(T), head);
            }

            public void Dispose()
            {
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
                throw new System.NotImplementedException();
            }
        }

        // Head of the linked list
        protected Node head = null;
        // Tail of the linked list
        protected Node tail = null;
        // Number of items in the list
        protected int count = 0;

        public T this[int index] { get { return Get(index); } set { Set(index, value); } }

        public int Count { get { return count; } }

        public bool IsReadOnly { get { return false; } }

        public bool IsEmpty { get { return count == 0; } }


        public LinkedList()
        {
            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LinkedListEnumerator(head);
        }

        public void Add(T item)
        {
            // If the list is empty, create a new node and set it as the head and tail
            if (head == null) {
                head = new Node(item);
                tail = head;
            } else {
                // Create a new node with the given value
                Node newNode = new Node(item, null, tail);
                // Set the tail's next node to the new node
                tail.Next = newNode;
                // Set the tail to the new node
                tail = newNode;
            }
            count++;
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range) {
                Add(item);
            }
        }

        public void Clear()
        {
            // For each node in the list, set the next and previous nodes to null to allow for garbage collection
            Node current = head;
            while (current != null) {
                Node next = current.Next;
                current.Next = null;
                current.Previous = null;
                current = next;
            }
            count = 0;
            head = null;
            tail = null;
        }

        public bool Contains(T item)
        {
            Node current = head;
            while (current != null) {
                if (current.Value.Equals(item)) {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Node current = head;
            while (current != null) {
                array[arrayIndex++] = current.Value;
                current = current.Next;
            }
        }

        public T First()
        {
            if (head == null) {
                throw new System.InvalidOperationException("List is empty");
            }
            return head.Value;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            Node current = head;
            for (int i = 0; i < index; i++) {
                current = current.Next;
            }
            return current.Value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedListEnumerator(head);
        }

        public int IndexOf(T item)
        {
            Node current = head;
            int index = 0;
            while (current != null) {
                if (current.Value.Equals(item)) {
                    return index;
                }
                current = current.Next;
                index++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (index == 0) {
                Node newNode = new Node(item, head);
                if (head != null) {
                    head.Previous = newNode;
                } else {
                    // head is null, so tail is also null
                    // set tail to the new node
                    tail = newNode;
                }
                head = newNode;
            } else if (index == count) {
                Node newNode = new Node(item, null, tail);
                tail.Next = newNode;
                tail = newNode;
            } else {
                Node current = head;
                for (int i = 0; i < index; i++) {
                    current = current.Next;
                }
                Node newNode = new Node(item, current, current.Previous);
                current.Previous.Next = newNode;
                current.Previous = newNode;
            }
            count++;
        }

        public void InsertRange(int index, IEnumerable<T> range)
        {
            if (index < 0 || index > count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            if (head == null) {
                AddRange(range);
                return;
            }
            else
            {
                // Find the node that is right before the index
                Node current = head;
                for (int i = 0; i < index; i++) {
                    current = current.Next;
                }

                // Add the range of items
                foreach (T item in range) {
                    Node newNode = new Node(item, current, current.Previous);
                    if (current.Previous != null) {
                        current.Previous.Next = newNode;
                    } else {
                        head = newNode;
                    }
                    current.Previous = newNode;
                    count++;
                }
            }
        }

        public void InsertRange(int index, IEnumerable<T> range, int length)
        {
            if (index < 0 || index > count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            Node current = head;
            for (; index > 0; index--) {
                current = current.Next;
            }
            foreach (T item in range) {
                Node newNode = new Node(item, current, current.Previous);
                current.Previous.Next = newNode;
                current.Previous = newNode;
                current = newNode;

                length--;
                if (length == 0) {
                    break;
                }
            }
        }

        public T Last()
        {
            return tail.Value;
        }

        public bool Remove(T item)
        {
            Node current = head;
            while (current != null) {
                if (current.Value.Equals(item)) {
                    if (current.Previous != null) {
                        current.Previous.Next = current.Next;
                    } else {
                        head = current.Next;
                    }
                    if (current.Next != null) {
                        current.Next.Previous = current.Previous;
                    } else {
                        tail = current.Previous;
                    }
                    count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public void RemoveAll(T value)
        {
            Node current = head;
            while (current != null) {
                if (current.Value.Equals(value)) {
                    if (current.Previous != null) {
                        current.Previous.Next = current.Next;
                    } else {
                        head = current.Next;
                    }
                    if (current.Next != null) {
                        current.Next.Previous = current.Previous;
                    } else {
                        tail = current.Previous;
                    }
                    count--;
                }
                current = current.Next;
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            // Get the node at the given index
            Node current = head;
            for (int i = 0; i < index; i++) {
                current = current.Next;
            }
            // Check if the current node is the head. This is true if the
            // current node's previous node is null
            if (current.Previous != null) {
                current.Previous.Next = current.Next;
            } else {
                head = current.Next;
            }
            // Check if the current node is the tail. This is true if the
            // current node's next node is null
            if (current.Next != null) {
                current.Next.Previous = current.Previous;
            } else {
                tail = current.Previous;
            }
            count--;
        }

        public void RemoveFirst()
        {
            if (head == null) {
                throw new System.InvalidOperationException("List is empty");
            }

            head = head.Next;
            if (head != null) {
                head.Previous = null;
            } else {
                tail = null;
            }
            count--;
        }

        public bool RemoveFirst(T value)
        {
            Node current = head;
            while (current != null) {
                if (current.Value.Equals(value)) {
                    if (current.Previous != null) {
                        current.Previous.Next = current.Next;
                    } else {
                        head = current.Next;
                    }
                    if (current.Next != null) {
                        current.Next.Previous = current.Previous;
                    } else {
                        tail = current.Previous;
                    }
                    count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public void RemoveLast()
        {
            if (tail == null) {
                throw new System.InvalidOperationException("List is empty");
            }

            tail = tail.Previous;
            if (tail != null) {
                tail.Next = null;
            } else {
                head = null;
            }
            count--;
        }

        public bool RemoveLast(T value)
        {
            Node current = tail;
            while (current != null) {
                if (current.Value.Equals(value)) {
                    if (current.Previous != null) {
                        current.Previous.Next = current.Next;
                    } else {
                        head = current.Next;
                    }
                    if (current.Next != null) {
                        current.Next.Previous = current.Previous;
                    } else {
                        tail = current.Previous;
                    }
                    count--;
                    return true;
                }
                current = current.Previous;
            }
            return false;
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            Node current = head;
            for (; index > 0; index--) {
                current = current.Next;
            }
            for (; length > 0; length--) {
                if (current.Previous != null) {
                    current.Previous.Next = current.Next;
                } else {
                    head = current.Next;
                }
                if (current.Next != null) {
                    current.Next.Previous = current.Previous;
                } else {
                    tail = current.Previous;
                }
                count--;
                current = current.Next;
            }
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= count) {
                throw new System.ArgumentOutOfRangeException("Index out of range");
            }

            Node current = head;
            for (int i = 0; i < index; i++) {
                current = current.Next;
            }
            current.Value = value;
        }

        public T TakeFirst()
        {
            if (head == null) {
                throw new System.InvalidOperationException("List is empty");
            }

            T value = head.Value;
            head = head.Next;
            if (head != null) {
                head.Previous = null;
            } else {
                tail = null;
            }
            count--;
            return value;
        }

        public T TakeLast()
        {
            if (tail == null) {
                throw new System.InvalidOperationException("List is empty");
            }

            T value = tail.Value;
            tail = tail.Previous;
            if (tail != null) {
                tail.Next = null;
            } else {
                head = null;
            }
            count--;
            return value;
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