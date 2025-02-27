using GalacticBoundStudios.DataScribes.Managed.Queues;
using System;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    public class RedBlackTree<T> : ITree<T> where T : IComparable
    {
        protected class Node
        {
            public T value { get; set; }
            public Node left { get; set; }
            public Node right { get; set; }
            public Node parent { get; set; }
            public bool isRed { get; set; }

            public Node(T value)
            {
                this.value = value;
                left = null;
                right = null;
                parent = null;
                isRed = true;
            }
        }

        protected class RedBlackTreeEnumerator : System.Collections.IEnumerator, System.Collections.Generic.IEnumerator<T>
        {
            protected Queue<T> queue = null;
            T value = default(T);

            public RedBlackTreeEnumerator(RedBlackTree<T> tree)
            {
                queue = new Queue<T>();
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

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return count == 0;
            }
        }

        public T MinValue
        {
            get
            {
                return Min();
            }
        }

        public T MaxValue
        {
            get
            {
                return Max();
            }
        }

        protected Node root;
        protected int count;

        public RedBlackTree()
        {
            root = null;
            count = 0;
        }

        public void Add(T value)
        {
            if (root == null)
            {
                root = new Node(value);
                root.isRed = false;
            }
            else
            {
                Add(value, root);
            }
            count++;
        }

        protected void Add(T value, Node node)
        {
            if (value.CompareTo(node.value) < 0)
            {
                if (node.left == null)
                {
                    node.left = new Node(value);
                    node.left.parent = node;
                    FixTree(node.left);
                }
                else
                {
                    Add(value, node.left);
                }
            }
            else
            {
                if (node.right == null)
                {
                    node.right = new Node(value);
                    node.right.parent = node;
                    FixTree(node.right);
                }
                else
                {
                    Add(value, node.right);
                }
            }
        }

        protected void FixTree(Node node)
        {
            while (node != root && node.parent.isRed)
            {
                if (node.parent == node.parent.parent.left)
                {
                    Node uncle = node.parent.parent.right;
                    if (uncle != null && uncle.isRed)
                    {
                        node.parent.isRed = false;
                        uncle.isRed = false;
                        node.parent.parent.isRed = true;
                        node = node.parent.parent;
                    }
                    else
                    {
                        if (node == node.parent.right)
                        {
                            node = node.parent;
                            RotateLeft(node);
                        }
                        node.parent.isRed = false;
                        node.parent.parent.isRed = true;
                        RotateRight(node.parent.parent);
                    }
                }
                else
                {
                    Node uncle = node.parent.parent.left;
                    if (uncle != null && uncle.isRed)
                    {
                        node.parent.isRed = false;
                        uncle.isRed = false;
                        node.parent.parent.isRed = true;
                        node = node.parent.parent;
                    }
                    else
                    {
                        if (node == node.parent.left)
                        {
                            node = node.parent;
                            RotateRight(node);
                        }
                        node.parent.isRed = false;
                        node.parent.parent.isRed = true;
                        RotateLeft(node.parent.parent);
                    }
                }
            }
            root.isRed = false;
        }

        protected void RotateLeft(Node node)
        {
            Node right = node.right;
            node.right = right.left;
            if (right.left != null)
            {
                right.left.parent = node;
            }
            right.parent = node.parent;
            if (node == root)
            {
                root = right;
            }
            else if (node == node.parent.left)
            {
                node.parent.left = right;
            }
            else
            {
                node.parent.right = right;
            }
            right.left = node;
            node.parent = right;
        }

        protected void RotateRight(Node node)
        {
            Node left = node.left;
            node.left = left.right;
            if (left.right != null)
            {
                left.right.parent = node;
            }
            left.parent = node.parent;
            if (node == root)
            {
                root = left;
            }
            else if (node == node.parent.left)
            {
                node.parent.left = left;
            }
            else
            {
                node.parent.right = left;
            }
            left.right = node;
            node.parent = left;
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

            if (value.CompareTo(node.value) == 0)
            {
                return true;
            }
            else if (value.CompareTo(node.value) < 0)
            {
                return Contains(node.left, value);
            }
            else
            {
                return Contains(node.right, value);
            }
        }

        public void Remove(T value)
        {
            if (Remove(root, value))
            {
                count--;
            }
        }

        protected bool Remove(Node node, T value)
        {
            if (node == null)
            {
                return false;
            }

            if (value.CompareTo(node.value) == 0)
            {
                if (node.left == null && node.right == null)
                {
                    if (node == root)
                    {
                        root = null;
                    }
                    else if (node == node.parent.left)
                    {
                        node.parent.left = null;
                    }
                    else
                    {
                        node.parent.right = null;
                    }
                }
                else if (node.left == null)
                {
                    node.value = node.right.value;
                    node.left = node.right.left;
                    node.right = node.right.right;
                }
                else if (node.right == null)
                {
                    node.value = node.left.value;
                    node.right = node.left.right;
                    node.left = node.left.left;
                }
                else
                {
                    Node min = node.right;
                    while (min.left != null)
                    {
                        min = min.left;
                    }
                    node.value = min.value;
                    Remove(node.right, min.value);
                }
                return true;
            }
            else if (value.CompareTo(node.value) < 0)
            {
                return Remove(node.left, value);
            }
            else
            {
                return Remove(node.right, value);
            }
        }

        public void Clear()
        {
            root = null;
            count = 0;
        }

        public T Min()
        {
            if (IsEmpty)
            {
                return default(T);
            }

            Node node = root;
            while (node.left != null)
            {
                node = node.left;
            }
            return node.value;
        }

        public T Max()
        {
            if (IsEmpty)
            {
                return default(T);
            }

            Node node = root;
            while (node.right != null)
            {
                node = node.right;
            }
            return node.value;
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            ToCollection(root, ref collection);
        }

        protected void ToCollection(Node node, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (node == null)
            {
                return;
            }

            ToCollection(node.left, ref collection);
            collection.Add(node.value);
            ToCollection(node.right, ref collection);
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new RedBlackTreeEnumerator(this);
        }
    }
}