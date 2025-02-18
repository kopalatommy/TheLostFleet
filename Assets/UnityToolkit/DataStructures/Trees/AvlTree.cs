using System.Collections;

namespace ProjectWorlds.DataStructures.Trees
{
    public class AvlTree<T> : ITree<T> where T : System.IComparable<T>
    {
        protected class Node
        {
            public T Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public int Height { get; set; }

            public Node(T value)
            {
                Value = value;
                Left = null;
                Right = null;
                Height = 1;
            }
        }

        protected class AvlEnumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator
        {
            protected ProjectWorlds.DataStructures.Queues.Queue<T> queue = null;
            T Value = default(T);

            public AvlEnumerator(AvlTree<T> tree)
            {
                queue = new ProjectWorlds.DataStructures.Queues.Queue<T>();
                System.Collections.Generic.ICollection<T> collection = queue;
                tree.ToCollection(ref collection);
            }

            public T Current
            {
                get
                {
                    return Value;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Value;
                }
            }

            public void Dispose()
            {
                queue.Clear();
            }

            public bool MoveNext()
            {
                if (queue.IsEmpty) {
                    return false;
                }
                Value = queue.Dequeue();
                return true;
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }
        }

        public int Count { get; private set; } = 0;

        public bool IsEmpty { get { return Count == 0; } }

        public T MinValue { get { return Min(); } }

        public T MaxValue { get { return Max(); } }

        protected Node root = null;

        public AvlTree()
        {
            root = null;
        }

        public AvlTree(System.Collections.Generic.IEnumerable<T> collection)
        {
            root = null;
            foreach (T value in collection)
            {
                Add(value);
            }
        }

        public void Add(T value)
        {
            root = Add(root, value);
            Count++;
        }

        public void Clear()
        {
            root = null;
            Count = 0;
        }

        public bool Contains(T value)
        {
            return Contains(root, value);
        }

        protected bool Contains(Node node, T value)
        {
            if (node == null)
            {
                return false;
            }

            if (value.CompareTo(node.Value) < 0)
            {
                return Contains(node.Left, value);
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                return Contains(node.Right, value);
            }
            else
            {
                return true;
            }
        }

        public void Remove(T value)
        {
            bool removed = false;
            root = Remove(root, value, ref removed);
            if (removed)
            {
                Count--;
            }
        }

        protected Node Remove(Node node, T value, ref bool removed)
        {
            // Base case
            if (node == null)
            {
                return node;
            }

            // If less than current node, go left
            if (value.CompareTo(node.Value) < 0)
            {
                node.Left = Remove(node.Left, value, ref removed);
            }
            // If greater than current node, go right
            else if (value.CompareTo(node.Value) > 0)
            {
                node.Right = Remove(node.Right, value, ref removed);
            }
            // If equal, remove node
            else
            {
                removed = true;
                if (node.Left == null || node.Right == null)
                {
                    Node temp = null;
                    if (node.Left == null)
                    {
                        temp = node.Right;
                    }
                    else
                    {
                        temp = node.Left;
                    }

                    if (temp == null)
                    {
                        node = null;
                    }
                    else
                    {
                        node = temp;
                    }
                }
                else
                {
                    Node temp = MinValueNode(node.Right);
                    node.Value = temp.Value;
                    node.Right = Remove(node.Right, temp.Value, ref removed);
                }
            }

            if (node == null)
            {
                return node;
            }

            node.Height = 1 + System.Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            if (balance > 1 && GetBalance(node.Left) >= 0)
            {
                return RightRotate(node);
            }

            if (balance > 1 && GetBalance(node.Left) < 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && GetBalance(node.Right) <= 0)
            {
                return LeftRotate(node);
            }

            if (balance < -1 && GetBalance(node.Right) > 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
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

        protected Node Add(Node node, T value)
        {
            if (node == null)
            {
                return new Node(value);
            }

            if (value.CompareTo(node.Value) < 0)
            {
                node.Left = Add(node.Left, value);
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                node.Right = Add(node.Right, value);
            }
            else
            {
                return node;
            }

            node.Height = 1 + System.Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            if (balance > 1 && value.CompareTo(node.Left.Value) < 0)
            {
                return RightRotate(node);
            }

            if (balance < -1 && value.CompareTo(node.Right.Value) > 0)
            {
                return LeftRotate(node);
            }

            if (balance > 1 && value.CompareTo(node.Left.Value) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && value.CompareTo(node.Right.Value) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public T Min()
        {
            if (IsEmpty)
            {
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
            if (IsEmpty)
            {
                return default(T);
            }

            Node current = root;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current.Value;
        }

        protected Node RightRotate(Node node)
        {
            Node newRoot = node.Left;
            Node temp = newRoot.Right;

            newRoot.Right = node;
            node.Left = temp;

            node.Height = 1 + System.Math.Max(Height(node.Left), Height(node.Right));
            newRoot.Height = 1 + System.Math.Max(Height(newRoot.Left), Height(newRoot.Right));

            return newRoot;
        }

        protected Node LeftRotate(Node node)
        {
            Node newRoot = node.Right;
            Node temp = newRoot.Left;

            newRoot.Left = node;
            node.Right = temp;

            node.Height = 1 + System.Math.Max(Height(node.Left), Height(node.Right));
            newRoot.Height = 1 + System.Math.Max(Height(newRoot.Left), Height(newRoot.Right));

            return newRoot;
        }

        protected int Height(Node node)
        {
            if (node == null)
            {
                return 0;
            }

            return node.Height;
        }

        protected int GetBalance(Node node)
        {
            if (node == null)
            {
                return 0;
            }

            return Height(node.Left) - Height(node.Right);
        }

        protected Node MinValueNode(Node node)
        {
            Node current = node;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current;
        }

        protected Node MaxValueNode(Node node)
        {
            Node current = node;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current;
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new AvlEnumerator(this);
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("{");
            foreach (T value in this)
            {
                builder.Append(value);
                builder.Append(", ");
            }
            if (builder.Length > 1)
            {
                builder.Remove(builder.Length - 2, 2);
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}