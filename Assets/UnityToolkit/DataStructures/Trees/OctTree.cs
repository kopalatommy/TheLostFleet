namespace ProjectWorlds.DataStructures.Trees
{
    // Does not work with duplicate coordinates
    public class Octree<T>  : System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable
    {
        protected class Node
        {
            public bool IsLeaf
            {
                get
                {
                    return children == null;
                }
            }

            // Min and max bounds of the node
            public double[] min;
            public double[] max;

            // Children of the node
            public Node[] children;

            // Data stored in the node
            public System.Collections.Generic.KeyValuePair<double[], T>? data = null;

            public Node(double[] min, double[] max)
            {
                this.min = min;
                this.max = max;
                children = null;
                data = null;
            }
        }

        protected class OctreeEnumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator
        {
            private Octree<T> tree;
            private T current;
            private ProjectWorlds.DataStructures.Queues.Queue<T> queue;

            public OctreeEnumerator(Octree<T> tree)
            {
                this.tree = tree;
                queue = new ProjectWorlds.DataStructures.Queues.Queue<T>();
                System.Collections.Generic.ICollection<T> collection = queue;
                tree.ToCollection(collection);
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
                tree.ToCollection( collection);
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

        protected Node root;
        protected int count;
        protected int dimensions;

        protected double[] min;
        protected double[] max;

        public Octree(int dimensions, double[] min, double[] max)
        {
            this.dimensions = dimensions;
            this.min = min;
            this.max = max;
            root = new Node(min, max);
            count = 0;
        }

        public void Add(double[] coordinates, T value)
        {
            // Make sure the coordinates are valid
            if (coordinates == null || coordinates.Length != dimensions)
            {
                return;
            }

            // Make sure the coordinates are within the bounds of the tree
            if (!InBounds(coordinates, min, max))
            {
                return;
            }

            // Traverse the tree to find the leaf node
            Node current = root;
            while (!current.IsLeaf)
            {
                for (int i = 0; i < current.children.Length; i++)
                {
                    if (InBounds(coordinates, current.children[i].min, current.children[i].max))
                    {
                        current = current.children[i];
                        break;
                    }
                }
            }

            // If the node doesn't have any data, add the data to the node
            if (current.data == null)
            {
                current.data = new System.Collections.Generic.KeyValuePair<double[], T>(coordinates, value);
                count++;
            }
            else if (!CoordsAreEqual(current.data.Value.Key, coordinates))
            {
                // If the node already has data, split the node and add the data to the appropriate child node
                Split(current);
                Add(coordinates, value);
            }
        }

        protected void Split(Node node)
        {
            // Find the mid point on each axis
            double[] min = node.min;
            double[] max = node.max;
            double[] mid = new double[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                mid[i] = (min[i] + max[i]) * 0.5f;
            }

            // Create the child nodes
            node.children = new Node[8];
            for (int i = 0; i < 8; i++) {
                double[] childMin = new double[dimensions];
                double[] childMax = new double[dimensions];

                for (int j = 0; j < dimensions; j++)
                {
                    if ((i & (1 << j)) == 0)
                    {
                        childMin[j] = min[j];
                        childMax[j] = mid[j];
                    }
                    else
                    {
                        childMin[j] = mid[j];
                        childMax[j] = max[j];
                    }
                }

                node.children[i] = new Node(childMin, childMax);
            }

            for (int i = 0; i < node.children.Length; i++)
            {
                if (InBounds(node.children[i].min, min, max) || InBounds(node.children[i].max, min, max))
                {
                    node.children[i].data = node.data;
                    break;
                }
            }
        }

        public bool Contains(double[] coordinates)
        {
            return Contains(root, coordinates);
        }

        protected bool Contains(Node node, double[] coordinates)
        {
            if (node == null)
            {
                return false;
            }

            if (node.IsLeaf)
            {
                return node.data != null && CoordsAreEqual(node.data.Value.Key, coordinates);
            }
            else
            {
                int index = 0;
                for (int i = 0; i < node.children.Length; i++)
                {
                    if (InBounds(coordinates, node.children[i].min, node.children[i].max))
                    {
                        return Contains(node.children[i], coordinates);
                    }
                }
            }

            return false;
        }

        public void Remove(double[] coordinates)
        {
            if (IsEmpty)
            {
                return;
            }

            if (RemoveInternal(coordinates))
            {
                count--;
            }
        }

        protected bool RemoveInternal(double[] coordinates)
        {
            Node current = root;
            
            // Find the node with the data
            while (!current.IsLeaf)
            {
                for (int i = 0; i < current.children.Length; i++)
                {
                    if (InBounds(coordinates, current.children[i].min, current.children[i].max))
                    {
                        current = current.children[i];
                        break;
                    }
                }
            }

            // Remove the data from the node
            if (current.data != null && CoordsAreEqual(current.data.Value.Key, coordinates))
            {
                current.data = null;
                return true;
            }

            return false;
        }

        protected bool InBounds(double[] coords, double[] min, double[] max)
        {
            for (int i = 0; i < dimensions; i++)
            {
                if (coords[i] < min[i] || coords[i] > max[i])
                {
                    return false;
                }
            }
            return true;
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new OctreeEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            root = new Node(min, max);
            count = 0;
        }

        public void ToCollection(System.Collections.Generic.ICollection<T> collection)
        {
            ToCollection(root, collection);
        }

        protected void ToCollection(Node node, System.Collections.Generic.ICollection<T> collection)
        {
            if (node == null)
            {
                return;
            }

            if (node.IsLeaf)
            {
                if (node.data != null)
                {
                    collection.Add(node.data.Value.Value);
                }
            }
            else
            {
                for (int i = 0; i < node.children.Length; i++)
                {
                    ToCollection(node.children[i], collection);
                }
            }
        }

        protected bool CoordsAreEqual(double[] coords1, double[] coords2)
        {
            if (coords1.Length != coords2.Length)
            {
                return false;
            }

            for (int i = 0; i < coords1.Length; i++)
            {
                if (coords1[i] != coords2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}