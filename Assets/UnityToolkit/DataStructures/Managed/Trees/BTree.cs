using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Queues;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    public class BTree<T> : ITree<T> where T : System.IComparable
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
            public bool isLeaf { get { return children.IsEmpty; } }

            // Returns true if the node is full
            public bool IsFull()
            {
                return values.Count == (2 * degree) - 1;
            }

            public Node(int degree)
            {
                this.degree = degree;
                values = new ArrayList<T>((2 * degree) - 1);
                children = new ArrayList<Node>((2 * degree) - 1);
            }
        }

        protected class BTreeEnumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator
        {
            private BTree<T> tree;
            private T current;
            private Queue<T> queue;

            public BTreeEnumerator(BTree<T> tree)
            {
                this.tree = tree;
                queue = new Queue<T>();
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

        protected Node? root = null;
        // Determines the number of children a node can have
        protected int degree;
        // Tracks the number of elements in the tree
        protected int count = 0;

        public BTree()
        {
            root = null;
            degree = 3;
        }

        public BTree(int degree)
        {
            root = null;
            this.degree = degree;
        }

        public void Add(T value)
        {
            if (root == null) {
                root = new Node(degree);
                root.values.Add(value);
                count++;
            } else {
                if (root.IsFull()) {
                    Node newRoot = new Node(degree);
                    newRoot.children.Add(root);
                    Split(newRoot, 0);
                    root = newRoot;
                    Add(value);
                } else {
                    Insert(root, value);
                }
            }
        }

        public void Remove(T value)
        {
            // return Remove(root, value);
            Remove(root, value);
        }

        protected void Split(Node parent, int childIndex)
        {
            Node newChild = new Node(degree);
            Node child = parent.children[childIndex];

            // Copy the second half of the values from the full child to the new child
            for (int i = degree; i < child.values.Count; i++) {
                newChild.values.Add(child.values[i]);
            }
            child.values.RemoveRange(degree, child.values.Count - degree);

            // Copy the second half of the children from the full child to the new child
            if (!child.isLeaf) {
                for (int i = degree; i < child.children.Count; i++) {
                    newChild.children.Add(child.children[i]);
                }
                child.children.RemoveRange(degree, child.children.Count - degree);
            }

            // Insert the new child into the parent
            parent.children.Insert(childIndex + 1, newChild);

            // Insert the median value into the parent
            parent.values.Insert(childIndex, child.values[degree - 1]);

            // Remove the median value from the full child
            child.values.RemoveAt(degree - 1);
        }

        protected void Insert(Node node, T value)
        {
            if (node.isLeaf) {
                // node.values.Add(value);
                // node.values.Sort();
                // count++;

                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                node.values.Insert(index, value);
                count++;
            } else {
                int index = 0;
                while (index < node.values.Count && value.CompareTo(node.values[index]) > 0) {
                    index++;
                }
                if (node.children[index].IsFull()) {
                    Split(node, index);
                    if (value.CompareTo(node.values[index]) > 0) {
                        index++;
                    }
                }
                Insert(node.children[index], value);
            }
        }

        protected bool Remove(Node node, T value)
        {
            // Base case
            if (node == null) {
                return false;
            }

            // Find the value to remove
            for (int i = 0; i < node.values.Count; i++) {
                // Less than
                if (node.values[i].CompareTo(value) < 0) {
                    // Continue searching. All the values to the left will be less than the value to remove
                    continue;
                }
                // Equal to
                else if (node.values[i].CompareTo(value) == 0) {
                    // If the node is a leaf
                    if (node.isLeaf) {
                        // Remove the value
                        node.values.RemoveAt(i);
                        count--;
                        return true;
                    }
                    // If the node is not a leaf
                    else {
                        // Get the predecessor
                        T predecessor = GetPredecessor(node.children[i]);
                        // Replace the value with the predecessor
                        node.values[i] = predecessor;
                        // Remove the predecessor
                        return Remove(node.children[i], predecessor);
                    }
                }
                // Greater than
                else {
                    if (!node.isLeaf) {
                        // If in the tree, will be in the next child node
                        return Remove(node.children[i], value);
                    } else {
                        return false;
                    }
                }
            }

            if (!node.isLeaf && node.values.Count < node.children.Count) {
                return Remove(node.children[node.values.Count], value);
            } else {
                return false;
            }
        }

        protected T GetPredecessor(Node node)
        {
            Node current = node;
            while (!current.isLeaf) {
                current = current.children[current.children.Count - 1];
            }
            return current.values[current.values.Count - 1];
        }

        protected T GetSuccessor(Node node)
        {
            Node current = node;
            while (!current.isLeaf) {
                current = current.children[0];
            }
            return current.values[0];
        }

        public bool Contains(T value)
        {
            if (IsEmpty) {
                return false;
            }

            return Contains(root, value);
        }

        protected bool Contains(Node node, T value)
        {
            for (int i = 0; i < node.values.Count; i++) {
                // Less than
                if (node.values[i].CompareTo(value) < 0) {
                    continue;
                }
                else if (node.values[i].CompareTo(value) == 0) {
                    return true;
                } else {
                    if (node.isLeaf) {
                        return false;
                    } else {
                        return Contains(node.children[i], value);
                    }
                }
            }

            if (!node.isLeaf && node.values.Count < node.children.Count) {
                return Contains(node.children[node.values.Count], value);
            } else {
                return false;
            }
        }

        public void Clear()
        {
            root = null;
            count = 0;
        }

        public T Min()
        {
            Node current = root;
            while (current != null && !current.isLeaf) {
                current = current.children[0];
            }
            return current != null ? current.values[0] : default(T);
        }

        public T Max()
        {
            Node current = root;
            while (current != null && !current.isLeaf) {
                current = current.children[current.children.Count - 1];
            }
            return current != null ? current.values[current.values.Count - 1] : default(T);
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            if (IsEmpty || collection == null) {
                return;
            }

            ToCollection(root, ref collection);
        }

        protected void ToCollection(Node node, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (node.isLeaf) {
                foreach (T value in node.values) {
                    collection.Add(value);
                }
            } else {
                for (int i = 0; i < node.values.Count; i++) {
                    ToCollection(node.children[i], ref collection);
                    collection.Add(node.values[i]);
                }
                if (node.children.Count == node.values.Count + 1) {
                    ToCollection(node.children[node.values.Count], ref collection);
                }
            }
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new BTreeEnumerator(this);
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
                if (!node.isLeaf) {
                    for (int i = 0; i < node.children.Count; i++) {
                        ToStr(node.children[i], ref result);
                    }
                }
            }
        }

        // public void Display()
        // {
        //     ArrayList<string> result = new ArrayList<string>();
        //     Display(root, 0, result);

        //     Console.WriteLine("B-Tree");
        //     for (int i = 0; i < result.Count; i++) {
        //         Console.WriteLine(result[i]);
        //     }
        // }

        // protected void Display(Node node, int level, ArrayList<string> levelStrings)
        // {
        //     if (node == null) {
        //         return;
        //     }

        //     if (levelStrings.Count <= level) {
        //         levelStrings.Add(string.Empty);
        //     }

        //     levelStrings[level] += (node.isLeaf ? "T" : "F") + "<" + node.values + "> ";

        //     for (int i = 0; i < node.children.Count; i++) {
        //         Display(node.children[i], level + 1, levelStrings);
        //     }
        // }
    }
}