using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Trees
{
    // A B+ tree is a B-tree where all of the values are stored in the leaves
    public class BPlusTree<T> : ITree<T> where T : System.IComparable
    {
        protected class Node
        {
            // Determines the number of children the node can have
            public int degree { get; set; }
            // Array of values
            public ArrayList<T> values { get; set; }
            // Array of children
            public ArrayList<Node> children { get; set; }
            // Determines if the node is a leaf
            public bool IsLeaf { get { return children.IsEmpty; } }

            // Returns true if the node is full
            public bool IsFull
            {
                get { return values.Count == (2 * degree) - 1; }
            }

            public Node(int degree)
            {
                this.degree = degree;
                values = new ArrayList<T>((2 * degree) - 1);
                children = new ArrayList<Node>((2 * degree) - 1);
            }
        }

        protected class BPlusTreeEnumerator : System.Collections.IEnumerator, System.Collections.Generic.IEnumerator<T>
        {
            private BPlusTree<T> tree;
            private T current;
            private ProjectWorlds.DataStructures.Queues.Queue<T> queue;

            public BPlusTreeEnumerator(BPlusTree<T> tree)
            {
                this.tree = tree;
                queue = new ProjectWorlds.DataStructures.Queues.Queue<T>();
                System.Collections.Generic.ICollection<T> collection = queue;
                tree.ToCollection(ref collection);
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return current;
                }
            }

            public void Dispose()
            {
                tree = null;
                current = default(T);
                queue.Clear();
                queue = null;
            }

            public bool MoveNext()
            {
                if (queue.Count > 0)
                {
                    current = queue.Dequeue();
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                queue.Clear();
                System.Collections.Generic.ICollection<T> collection = queue;
                tree.ToCollection(ref collection);
            }
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsEmpty
        {
            get { return count == 0; }
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

        // The root of the tree
        protected Node? root = null;
        // The degree of the tree, determines the number of children a node can have
        protected int degree;
        // The number of values in the tree
        protected int count = 0;

        public BPlusTree()
        {
            this.root = null;
            this.degree = 3;
        }

        public BPlusTree(int degree)
        {
            this.root = null;
            this.degree = degree;
        }

        public void Add(T value)
        {
            if (root == null) {
                root = new Node(degree);
                root.values.Add(value);
            } else {
                if (root.IsFull) {
                    Node newRoot = new Node(degree);
                    newRoot.children.Add(root);
                    root = newRoot;
                    Split(root, 0);
                    Insert(root, value);
                } else {
                    Insert(root, value);
                }
            }
            count++;
        }

        protected void Insert(Node node, T value)
        {
            if (node.IsLeaf) {
                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                node.values.Insert(index, value);
            } else {
                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                if (node.children[index].IsFull) {
                    Split(node, index);
                    if (value.CompareTo(node.values[index]) > 0) {
                        index++;
                    }
                }
                Insert(node.children[index], value);
            }
        }

        protected void Split(Node node, int childIndex)
        {
            Node newChild = new Node(degree);
            Node child = node.children[childIndex];

            // Copy the second half of the values and children to the new child
            for (int i = degree; i < child.values.Count; i++) {
                newChild.values.Add(child.values[i]);
            }
            child.values.RemoveRange(degree, child.values.Count - degree);

            if (!child.IsLeaf) {
                for (int i = degree; i < child.children.Count; i++) {
                    newChild.children.Add(child.children[i]);
                }
                child.children.RemoveRange(degree, child.children.Count - degree);
            }

            // Insert the new child into the parent
            node.children.Insert(childIndex + 1, newChild);

            // Insert the median value into the parent
            node.values.Insert(childIndex, child.values[degree - 1]);
        }

        public bool Contains(T value)
        {
            return Contains(root, value);
        }

        protected bool Contains(Node node, T value)
        {
            if (node == null) {
                return false;
            }

            if (node.IsLeaf) {
                return node.values.Contains(value);
            } else {
                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                return Contains(node.children[index], value);
            }
        }

        public void Remove(T value)
        {
            if (Remove(root, value)) {
                count--;
            }
        }

        protected bool Remove(Node node, T value)
        {
            if (node == null) {
                return false;
            }

            if (node.IsLeaf) {
                return node.values.Remove(value);
            } else {
                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                return Remove(node.children[index], value);
            }
        }

        public void Clear()
        {
            root = null;
            count = 0;
        }

        public T Min()
        {
            if (IsEmpty) {
                return default(T);
            }

            Node current = root;
            while (!current.IsLeaf) {
                current = current.children[0];
            }
            return current.values[0];
        }

        public T Max()
        {
            if (IsEmpty) {
                return default(T);
            }

            Node current = root;
            while (!current.IsLeaf) {
                current = current.children[current.children.Count - 1];
            }
            return current.values[current.values.Count - 1];
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            ToCollection(root, ref collection);
        }

        protected void ToCollection(Node node, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (node == null) {
                return;
            }

            if (node.IsLeaf) {
                for (int i = 0; i < node.values.Count; i++) {
                    collection.Add(node.values[i]);
                }
            } else {
                for (int i = 0; i < node.children.Count; i++) {
                    ToCollection(node.children[i], ref collection);
                }
            }
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new BPlusTreeEnumerator(this);
        }

                public override string ToString()
        {
            string result = string.Empty;
            ToStr(ref result);
            return result;
        }

        protected void ToStr(ref string result)
        {
            ToStr(root, ref result);
        }

        protected void ToStr(Node node, ref string result)
        {
            if (node != null) {
                for (int i = 0; i < node.values.Count; i++) {
                    result += node.values[i] + " ";
                }
                if (!node.IsLeaf) {
                    for (int i = 0; i < node.children.Count; i++) {
                        ToStr(node.children[i], ref result);
                    }
                }
            }
        }
    }
}
