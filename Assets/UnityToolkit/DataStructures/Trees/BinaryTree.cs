

namespace ProjectWorlds.DataStructures.Trees
{
    public class BinaryTree<T> : ITree<T> where T : System.IComparable<T>
    {
        protected class BinaryTreeEnumerator : System.Collections.IEnumerator, System.Collections.Generic.IEnumerator<T>
        {
            protected ProjectWorlds.DataStructures.Queues.Queue<T> queue = null;
            T value = default(T);

            public BinaryTreeEnumerator(BinaryTree<T> tree)
            {
                queue = new ProjectWorlds.DataStructures.Queues.Queue<T>();
                System.Collections.Generic.ICollection<T> collection = queue;
                tree.ToCollection(ref collection);
            }
            
            public T Current
            {
                get
                {
                    return value;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return value;
                }
            }

            public void Dispose()
            {
                queue.Clear();
            }

            public bool MoveNext()
            {
                if (queue.IsEmpty)
                {
                    return false;
                }
                value = queue.Dequeue();
                return true;
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }
        }

        protected class Node
        {
            public T Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public Node(T value)
            {
                Value = value;
                Left = null;
                Right = null;
            }
        }

        public int Count { get; private set; } = 0;

        public bool IsEmpty { get { return Count == 0; } }

        public T MinValue { get { return Min(); } }

        public T MaxValue { get { return Max(); } }

        protected Node root = null;

        public BinaryTree()
        {
            root = null;
        }

        public BinaryTree(System.Collections.Generic.IEnumerable<T> collection)
        {
            root = null;
            foreach (T value in collection)
            {
                Add(value);
            }
        }

        public void Add(T value)
        {
            if (root == null)
            {
                root = new Node(value);
            }
            else
            {
                Node current = root;
                while (true)
                {
                    bool isLessThanOrEqual = value.CompareTo(current.Value) <= 0;

                    if (isLessThanOrEqual)
                    {
                        if (current.Left == null)
                        {
                            current.Left = new Node(value);
                            break;
                        }
                        else
                        {
                            current = current.Left;
                        }
                    }
                    else
                    {
                        if (current.Right == null)
                        {
                            current.Right = new Node(value);
                            break;
                        }
                        else
                        {
                            current = current.Right;
                        }
                    }
                }
            }
            Count++;
        }

        public void Clear()
        {
            root = null;
            Count = 0;
        }

        public bool Contains(T value)
        {
            Node current = root;
            while (current != null)
            {
                bool isLessThanOrEqual = value.CompareTo(current.Value) <= 0;

                if (isLessThanOrEqual)
                {
                    if (value.CompareTo(current.Value) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }
                else
                {
                    current = current.Right;
                }
            }
            return false;
        }

        public void Remove(T value)
        {
            Node parent = null;
            Node current = root;
            while (current != null)
            {
                bool isLessThanOrEqual = value.CompareTo(current.Value) <= 0;

                if (isLessThanOrEqual)
                {
                    if (value.CompareTo(current.Value) == 0)
                    {
                        if (current.Left == null && current.Right == null)
                        {
                            if (parent == null)
                            {
                                root = null;
                            }
                            else
                            {
                                if (parent.Left == current)
                                {
                                    parent.Left = null;
                                }
                                else
                                {
                                    parent.Right = null;
                                }
                            }
                        }
                        else if (current.Left == null)
                        {
                            if (parent == null)
                            {
                                root = current.Right;
                            }
                            else
                            {
                                if (parent.Left == current)
                                {
                                    parent.Left = current.Right;
                                }
                                else
                                {
                                    parent.Right = current.Right;
                                }
                            }
                        }
                        else if (current.Right == null)
                        {
                            if (parent == null)
                            {
                                root = current.Left;
                            }
                            else
                            {
                                if (parent.Left == current)
                                {
                                    parent.Left = current.Left;
                                }
                                else
                                {
                                    parent.Right = current.Left;
                                }
                            }
                        }
                        else
                        {
                            Node successor = current.Right;
                            Node successorParent = current;
                            while (successor.Left != null)
                            {
                                successorParent = successor;
                                successor = successor.Left;
                            }

                            if (successorParent.Left == successor)
                            {
                                successorParent.Left = successor.Right;
                            }
                            else
                            {
                                successorParent.Right = successor.Right;
                            }

                            if (parent == null)
                            {
                                root = successor;
                            }
                            else
                            {
                                if (parent.Left == current)
                                {
                                    parent.Left = successor;
                                }
                                else
                                {
                                    parent.Right = successor;
                                }
                            }

                            successor.Left = current.Left;
                            successor.Right = current.Right;
                        }
                        Count--;
                        return;
                    }
                    else
                    {
                        parent = current;
                        current = current.Left;
                    }
                }
                else
                {
                    parent = current;
                    current = current.Right;
                }
            }
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            collection.Clear();
            ToCollection(root, ref collection);
        }
        
        protected void ToCollection(Node current, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (current == null)
            {
                return;
            }

            ToCollection(current.Left, ref collection);
            collection.Add(current.Value);
            ToCollection(current.Right, ref collection);
        }

        public T Min()
        {
            if (IsEmpty) {
                return default(T);
            }

            Node current = root;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current.Value;
        }

        public T Max()
        {
            if (IsEmpty) {
                return default(T);
            }

            Node current = root;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current.Value;
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new BinaryTreeEnumerator(this);
        }
    }
}