using GalacticBoundStudios.DataScribes.Managed.Lists;
using System;
using ProjectWorlds.Algorithms;
// using System.Collections.Generic;
using System.Diagnostics;
using GalacticBoundStudios.DataScribes.Managed.Queues;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    // This class represents a KDTree, a data structure that is used to store points in a k-dimensional space
    public class KDTree<T> : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<double[], T>>, System.Collections.IEnumerable
    {
        protected class Node : System.IComparable<Node>
        {
            public bool IsLeaf
            {
                get
                {
                    return bottomChild == null && topChild == null;
                }
            }

            public int Count 
            {
                get
                {
                    return count;
                }
            }

            // The axis that this node is splitting on
            public int axis;
            // The point that this node is splitting on
            public double axisCenter;

            // The index of the node, used to determine which item array to use
            public int nodeIndex = -1;

            // Node that contains all the points that are less than the axisCenter
            public Node? bottomChild;
            // Node that contains all the points that are greater than the axisCenter
            public Node? topChild;

            // The number of points in this node
            public int count;

            // Tracks the node bounds
            public double[] minBound;
            public double[] maxBound;

            public Node(int axis, int nodeIndex, int numDimensions)
            {
                bottomChild = null;
                topChild = null;
                this.axis = axis;
                this.nodeIndex = nodeIndex;
                minBound = new double[numDimensions];
                maxBound = new double[numDimensions];
            }

            public int CompareTo(Node? other)
            {
                if (other == null)
                {
                    return 1;
                }
                else
                {
                    return axisCenter.CompareTo(other.axisCenter);
                }
            }
        }

        protected class KDTreeEnumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<double[], T>>, System.Collections.IEnumerator
        {
            private KDTree<T> tree;
            private System.Collections.Generic.KeyValuePair<double[], T> current;
            private Queues.Queue<System.Collections.Generic.KeyValuePair<double[], T>> queue;

            public KDTreeEnumerator(KDTree<T> tree)
            {
                this.tree = tree;
                queue = new Queues.Queue<System.Collections.Generic.KeyValuePair<double[], T>>();
                System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<double[], T>> collection = queue;
                tree.ToCollection(ref collection);
            }

            public System.Collections.Generic.KeyValuePair<double[], T> Current
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
                current = new System.Collections.Generic.KeyValuePair<double[], T>();
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
                System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<double[], T>> collection = queue;
                tree.ToCollection(ref collection);
            }
        }

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        // Determines the number of items in each node
        protected int maxNodeItems = 32;

        // The root node of the tree
        protected Node? head = null;

        // The number of dimensions in the tree
        protected int dimensions;

        // The number of items in the tree
        protected int count;

        // Holds the items in the tree
        protected System.Collections.Generic.KeyValuePair<double[], T>[] items;

        // Tracks the number of nodes in the tree
        protected int numNodes;

        public KDTree()
        {
            head = null;
            dimensions = 3;
            items = new System.Collections.Generic.KeyValuePair<double[], T>[maxNodeItems];
        }

        public KDTree(int dimensions)
        {
            head = null;
            this.dimensions = dimensions;
            items = new System.Collections.Generic.KeyValuePair<double[], T>[maxNodeItems];
        }

        public KDTree(int dimensions, int maxNodeItems)
        {
            this.dimensions = dimensions;
            this.maxNodeItems = maxNodeItems;
            items = new System.Collections.Generic.KeyValuePair<double[], T>[maxNodeItems];
        }

        public void Add(T value, double[] coordinates)
        {
            if (coordinates == null)
            {
                throw new System.ArgumentNullException("The coordinates cannot be null");
            }
            else if (coordinates.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            } 

            System.Collections.Generic.KeyValuePair<double[], T> pair = new System.Collections.Generic.KeyValuePair<double[], T>(coordinates, value);
            count++;

            // If the head is null, then create the head node and add the item
            if (head == null)
            {
                // Create the head item
                head = new Node(0, numNodes++, dimensions);

                // Set the bounds of the head node
                Array.Copy(coordinates, head.minBound, dimensions);
                Array.Copy(coordinates, head.maxBound, dimensions);

                // Add the item to the value buffer in a position based on the node index
                items[head.count++] = pair;
            }
            else
            {
                // Traverse through the tree until a leaf node is found
                Node curNode = head;

                // Update the bounds of the current node
                for (int i = 0; i < dimensions; i++)
                {
                    curNode.maxBound[i] = Math.Max(curNode.maxBound[i], coordinates[i]);
                    curNode.minBound[i] = Math.Min(curNode.minBound[i], coordinates[i]);
                }

                while (!curNode.IsLeaf)
                {
                    // Check if above or below the center point
                    if (coordinates[curNode.axis] > curNode.axisCenter)
                    {
                        // Above, move to top node
                        curNode = curNode.topChild;
                    }
                    else
                    {
                        curNode = curNode.bottomChild;
                    }

                    // Update the bounds of the current node
                    for (int i = 0; i < dimensions; i++)
                    {
                        curNode.maxBound[i] = Math.Max(curNode.maxBound[i], coordinates[i]);
                        curNode.minBound[i] = Math.Min(curNode.minBound[i], coordinates[i]);
                    }
                }

                // If the node is full, need to Split into 2 nodes
                if (curNode.Count == maxNodeItems)
                {
                    SplitNode(curNode);

                    // Update curNode to the node that will contain the new node
                    curNode = FindNode(coordinates, curNode);
                }

                // Insert the item into the value buffer in the next location in the node's array
                items[(curNode.nodeIndex * maxNodeItems) + curNode.count] = pair;
                curNode.count++;
            }
        }

        private void SplitNode(Node node)
        {
            // Since the node items are about to be split, sort the sub section of the values buffer
            int startIndex = node.nodeIndex * maxNodeItems;
            int nextAxis = (node.axis + 1) % 3;
            int sortAxis = node.axis;

            // Sort each value based on the coordinate of the current axis: x, y, or z
            QuickSort.Sort(items, startIndex, startIndex + maxNodeItems - 1, (x, y) => x.Key[sortAxis].CompareTo(y.Key[sortAxis]));

            // Find the center point of all of the nodes
            double[] avgPos = new double[dimensions];
            for (int i = 0; i < node.count; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    avgPos[j] += items[startIndex + i].Key[j];
                }
            }
            for (int i = 0; i < dimensions; i++)
            {
                avgPos[i] /= node.count;
            }

            node.axisCenter = avgPos[sortAxis];

            // Resize the value buffer to give space for the split
            Array.Resize(ref items, items.Length + maxNodeItems);

            // Create the new top and bottom nodes
            Node topNode = new Node(nextAxis, numNodes, dimensions);
            Node bottomNode = new Node(nextAxis, node.nodeIndex, dimensions);

            // Set the bounds of the new nodes
            Array.Copy(node.minBound, bottomNode.minBound, dimensions);
            Array.Copy(node.maxBound, bottomNode.maxBound, dimensions);

            Array.Copy(node.minBound, topNode.minBound, dimensions);
            Array.Copy(node.maxBound, topNode.maxBound, dimensions);

            // Update the bounds of the new nodes
            bottomNode.maxBound[sortAxis] = topNode.minBound[sortAxis] = avgPos[sortAxis];

            numNodes++;

            // Set the child node values
            node.bottomChild = bottomNode;
            node.topChild = topNode;

            // Set the node index of the current node to -1 because it is no longer a leaf
            node.nodeIndex = -1;

            // Find the relative index of the first node above the average point
            int index = 0;
            for (; index < maxNodeItems; index++)
            {
                // If the node should be in the top node, break
                if (items[startIndex + index].Key[sortAxis] > avgPos[sortAxis])
                {
                    break;
                }
            }

            // If this case is hit, need to divide node again, max of 2 more times
            if (index == 0 || index == maxNodeItems)
            {
                throw new System.Exception("Tree is broken");
            }

            // Set the count values for the new nodes
            bottomNode.count = index;
            topNode.count = maxNodeItems - index;

            // Move the remaining items in the node array to the new buffer location
            // Get the index of the next start index
            int newStart = topNode.nodeIndex * maxNodeItems;
            for (; index < maxNodeItems; index++)
            {
                items[newStart++] = items[startIndex + index];
                items[startIndex + index] = new System.Collections.Generic.KeyValuePair<double[], T>();
            }
        }

        // Returns true if a node contains the given location
        public bool ContainsPoint(double[] point)
        {
            if (point.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            }

            if (head == null)
            {
                return false;
            }

            Node curNode = FindNode(point, head);

            int startIndex = curNode.nodeIndex * maxNodeItems;
            for (int i = 0; i < curNode.count; i++)
            {
                if (PointsAreEqual(items[startIndex++].Key, point))
                {
                    return true;
                }
            }

            return false;
        }

        // Helper function that handles traversing the tree to find the child node for the given point
        private Node FindNode(double[] position, Node curNode)
        {
            while (!curNode.IsLeaf)
            {
                // Check if above or below the center point
                if (position[curNode.axis] > curNode.axisCenter)
                {
                    // Above, move to top node
                    curNode = curNode.topChild;
                }
                else
                {
                    // Less than or equal to, move to bottom child
                    curNode = curNode.bottomChild;
                }
            }
            return curNode;
        }

        // Get k nearest neighbors to the given point
        public OrderedList<float, T> GetNearestNeighbors(double[] point, int maxItems)
        {
            OrderedList<float, T> result = new OrderedList<float, T>();

            // If the head is null, return an empty list
            if (head == null)
            {
                return result;
            }

            // This tracks which nodes have been searched
            ArrayList<Node> searched = new ArrayList<Node>() { head };

            // This tracks the nodes that have yet to be searched
            Queues.Queue<Node> toSearch = new Queues.Queue<Node>() { head };

            // Perform an initial depth first search to find the nearest node
            while (toSearch.Count > 0)
            {
                Node curNode = toSearch.Dequeue();

                // Check if the curNode could contain a new point, if not skip it
                if (!CircleIntersectsRect(point, result.Count < maxItems ? float.MaxValue : result.Last().Key, curNode.maxBound, curNode.minBound))
                {
                    continue;
                }

                // If the current node is a leaf, check each item to see if should be in list
                if (curNode.IsLeaf)
                {
                    int start = curNode.nodeIndex * maxNodeItems;
                    for (int i = 0; i < curNode.count; i++)
                    {
                        float dist = Distance(point, items[start + i].Key);

                        if (result.Count < maxItems)
                        {
                            result.Add(dist, items[i + start].Value);
                        }
                        else if (result.Last().Key > dist)
                        {
                            result.RemoveLast();
                            result.Add(dist, items[i + start].Value);
                        }
                    }
                }
                else
                {
                    if (!searched.Contains(curNode.topChild))
                    {
                        searched.Add(curNode.topChild);
                        toSearch.Add(curNode.topChild);
                    }
                    if (!searched.Contains(curNode.bottomChild))
                    {
                        searched.Add(curNode.bottomChild);
                        toSearch.Add(curNode.bottomChild);
                    }
                }
            }

            return result;
        }

        // This function will return all nodes within the given distance to the given point
        public OrderedList<float, T> RadialSearch(double[] center, float searchRadius)
        {
            if (searchRadius < 0) {
                throw new System.ArgumentException("The distance must be greater than or equal to 0");
            }

            if (center.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            }

            OrderedList<float, T> result = new OrderedList<float, T>();

            if (IsEmpty) {
                return result;
            }

            Queues.Queue<Node> toSearch = new Queues.Queue<Node>() { head };
            while (toSearch.Count > 0)
            {
                Node curNode = toSearch.TakeFirst();

                if (curNode.IsLeaf)
                {
                    // If the node might contain points within the range, search each point to see if in bounds
                    if (CircleIntersectsRect(center, searchRadius, curNode.maxBound, curNode.minBound))
                    {
                        int startIndex = curNode.nodeIndex * maxNodeItems;
                        for (int i = 0; i < curNode.count; i++)
                        {
                            float distance = Distance(items[startIndex + i].Key, center);
                            if (distance <= searchRadius)
                            {
                                result.Add(distance, items[startIndex + i].Value);
                            }
                        }
                    }
                }
                else
                {
                    // If the node might contain points within the range, add the children to the search queue
                    if (CircleIntersectsRect(center, searchRadius, curNode.maxBound, curNode.minBound))
                    {
                        toSearch.Add(curNode.topChild);
                        toSearch.Add(curNode.bottomChild);
                    }
                }
            }

            return result;
        }

        private bool CircleIntersectsRect(double[] circleCenter, float circleRadius, double[] rectMax, double[] rectMin)
        {
            // Find the closest point in the rectangle to the circle
            double[] closest = new double[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                closest[i] = Math.Clamp(circleCenter[i], rectMin[i], rectMax[i]);
            }

            // Return true if the distance from closet to circle center is less than or equal to the radius
            return Distance(closest, circleCenter) <= circleRadius;
        }

        protected float Distance(double[] point1, double[] point2)
        {
            float sum = 0;
            for (int i = 0; i < dimensions; i++)
            {
                sum += (float)Math.Pow(point1[i] - point2[i], 2);
            }
            return (float)Math.Sqrt(sum);
        }

        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<double[], T>> GetEnumerator()
        {
            return new KDTreeEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new KDTreeEnumerator(this);
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<double[], T>> collection)
        {
            ToCollection(head, collection);
        }

        protected void ToCollection(Node? node, System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<double[], T>> collection)
        {
            if (node == null)
            {
                return;
            }

            if (node.IsLeaf)
            {
                int startIndex = node.nodeIndex * maxNodeItems;
                for (int i = 0; i < node.count; i++)
                {
                    collection.Add(items[startIndex + i]);
                }
            }
            else
            {
                ToCollection(node.bottomChild, collection);
                ToCollection(node.topChild, collection);
            }
        }

        public void Clear()
        {
            numNodes = 0;
            head = null;
            count = 0;
            items = new System.Collections.Generic.KeyValuePair<double[], T>[maxNodeItems];
        }

        public bool Remove(T value, double[] coordinates)
        {
            if (coordinates.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            }

            if (head == null)
            {
                return false;
            }

            Node curNode = FindNode(coordinates, head);

            int startIndex = curNode.nodeIndex * maxNodeItems;
            for (int i = 0; i < curNode.count; i++)
            {
                if (PointsAreEqual(items[startIndex + i].Key, coordinates))
                {
                    for (int j = i; j < curNode.count - 1; j++)
                    {
                        items[startIndex + j] = items[startIndex + j + 1];
                    }
                    items[startIndex + curNode.count - 1] = new System.Collections.Generic.KeyValuePair<double[], T>();
                    curNode.count--;
                    count--;
                    return true;
                }
            }

            return false;
        }

        public bool Remove(double[] coordinates)
        {
            if (coordinates.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            }

            if (head == null)
            {
                return false;
            }

            Node curNode = FindNode(coordinates, head);

            int startIndex = curNode.nodeIndex * maxNodeItems;
            for (int i = 0; i < curNode.count; i++)
            {
                if (PointsAreEqual(items[startIndex + i].Key, coordinates))
                {
                    for (int j = i; j < curNode.count - 1; j++)
                    {
                        items[startIndex + j] = items[startIndex + j + 1];
                    }
                    items[startIndex + curNode.count - 1] = new System.Collections.Generic.KeyValuePair<double[], T>();
                    curNode.count--;
                    count--;
                    return true;
                }
            }

            return false;
        }

        protected bool PointsAreEqual(double[] point1, double[] point2)
        {
            for (int i = 0; i < dimensions; i++)
            {
                if (point1[i] != point2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public OrderedList<float, T> IntervalQuery(double[] minBounds, double[] maxBounds)
        {
            OrderedList<float, T> result = new OrderedList<float, T>();

            if (minBounds.Length != dimensions || maxBounds.Length != dimensions)
            {
                throw new System.ArgumentException("The number of coordinates must match the number of dimensions in the tree");
            }

            if (IsEmpty) {
                return result;
            }

            Queues.Queue<Node> toSearch = new Queues.Queue<Node>() { head };
            while (toSearch.Count > 0)
            {
                Node curNode = toSearch.TakeFirst();

                if (curNode.IsLeaf)
                {
                    int startIndex = curNode.nodeIndex * maxNodeItems;
                    for (int i = 0; i < curNode.count; i++)
                    {
                        double[] point = items[startIndex + i].Key;
                        if (PointInBounds(point, minBounds, maxBounds))
                        {
                            result.Add(Distance(point, minBounds), items[startIndex + i].Value);
                        }
                    }
                }
                else
                {
                    if (curNode.bottomChild != null && BoundsIntersect(curNode.bottomChild.minBound, curNode.bottomChild.maxBound, maxBounds, minBounds))
                    {
                        toSearch.Add(curNode.bottomChild);
                    }
                    if (curNode.topChild != null && BoundsIntersect(curNode.topChild.minBound, curNode.topChild.maxBound, maxBounds, minBounds))
                    {
                        toSearch.Add(curNode.topChild);
                    }
                }
            }

            return result;
        }

        private bool BoundsIntersect(double[] minBounds, double[] maxBounds, double[] rectMax, double[] rectMin)
        {
            for (int i = 0; i < dimensions; i++)
            {
                if (minBounds[i] > rectMax[i] || maxBounds[i] < rectMin[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool PointInBounds(double[] point, double[] minBounds, double[] maxBounds)
        {
            for (int i = 0; i < dimensions; i++)
            {
                if (point[i] < minBounds[i] || point[i] > maxBounds[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}