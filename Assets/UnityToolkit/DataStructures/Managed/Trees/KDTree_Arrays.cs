#if UNITY_STANDALONE
using UnityEngine;
#else
using GameDevToolkitConsole.UnityToolkit.Utilities;
#endif
using GalacticBoundStudios.DataScribes.Managed.Stacks;
using GalacticBoundStudios.DataScribes.Managed.Heaps;
using System;
using System.Collections.Generic;

// https://github.com/viliwonka/KDTree
// https://github.com/ArthurBrussee/KNN

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    public class KDTree_Arrays
    {
        public class QueryNode : System.IComparable<QueryNode>
        {
            public Node node;
            public Vector3 closestPoint;
            public float distance;

            public QueryNode()
            {

            }

            public QueryNode(Node node, Vector3 closestPoint, float distance)
            {
                this.node = node;
                this.closestPoint = closestPoint;
                this.distance = distance;
            }

            public int CompareTo(QueryNode other)
            {
                return distance.CompareTo(other.distance);
            }
        }

        public class QueryBase
        {
            protected QueryNode[] nodeQueue;
            protected MinHeap<QueryNode> minHeap = null;
            protected int count = 0;
            protected int queryIndex = 0;

            protected int LeftToProcess
            {
                get
                {
                    return count - queryIndex;
                }
            }

            public QueryBase(int initialSize = 2048)
            {
                nodeQueue = new QueryNode[initialSize];
                minHeap = new MinHeap<QueryNode>(initialSize);
            }

            protected QueryNode PushGetQueue()
            {
                if (count < nodeQueue.Length)
                {
                    if (nodeQueue[count] == null)
                    {
                        nodeQueue[count] = new QueryNode();
                    }
                }
                else
                {
                    Array.Resize(ref nodeQueue, nodeQueue.Length + 32);
                    nodeQueue[count] = new QueryNode();
                }

                return nodeQueue[count++];
            }

            protected void PushToQueue(Node node, Vector3 closestPoint)
            {
                QueryNode queryNode = PushGetQueue();
                queryNode.node = node;
                queryNode.closestPoint = closestPoint;
            }

            protected void PushToHeap(Node node, Vector3 closestPoint, Vector3 queryPoint)
            {
                QueryNode queryNode = PushGetQueue();
                queryNode.node = node;
                queryNode.closestPoint = closestPoint;

                float sqrDist = Vector3.SqrMagnitude(queryPoint - closestPoint);
                queryNode.distance = sqrDist;
                minHeap.Add(queryNode);
            }

            protected QueryNode PopFromQueue()
            {
                return nodeQueue[queryIndex++];
            }

            protected QueryNode PopFromHeap()
            {
                QueryNode heapNode = minHeap.TakeMin();

                nodeQueue[queryIndex++] = heapNode;

                return heapNode;
            }

            protected void Reset()
            {
                count = 0;
                queryIndex = 0;
                minHeap.Clear();
            }
        }

        public class ClosestPointQuery : QueryBase
        {
            public void ClosestPoint(KDTree_Arrays tree, Vector3 queryPoint, List<int> resultIndices, List<float> resultDistances = null)
            {
                // Reset all query values
                Reset();

                Vector3[] points = tree.points;
                int[] permutations = tree.permutations;

                if (points.Length == 0) 
                {
                    return;
                }

                int smallestIndex = 0;

                // Smallest squared radius
                float ssr = float.MaxValue;

                Node rootNode = tree.rootNode;

                Vector3 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPoint);

                PushToHeap(rootNode, rootClosestPoint, queryPoint);

                QueryNode queryNode = null;
                Node node = null;

                int partitionAxis;
                float partitionCoord;

                Vector3 closestPoint;

                while (!minHeap.IsEmpty)
                {
                    queryNode = PopFromHeap();

                    if (queryNode.distance > ssr)
                    {
                        continue;
                    }

                    node = queryNode.node;

                    if (!node.IsLeaf)
                    {
                        partitionAxis = node.partitionAxis;
                        partitionCoord = node.partitionCoordinate;

                        closestPoint = queryNode.closestPoint;

                        if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                        {
                            PushToHeap(node.negativeChild, closestPoint, queryPoint);
                            closestPoint[partitionAxis] = partitionCoord;

                            if (node.positiveChild.Count != 0)
                            {
                                PushToHeap(node.positiveChild, closestPoint, queryPoint);
                            }
                        }
                        else
                        {
                            PushToHeap(node.positiveChild, closestPoint,queryPoint);
                            closestPoint[partitionAxis] = partitionCoord;

                            if (node.negativeChild.Count != 0)
                            {
                                PushToHeap(node.negativeChild, closestPoint, queryPoint);
                            }
                        }
                    }
                    else
                    {
                        float sqrDist;

                        for (int i = node.start; i < node.end; i++)
                        {
                            int index = permutations[i];
                            sqrDist = Vector3.SqrMagnitude(points[index] - queryPoint);

                            if (sqrDist <= ssr)
                            {
                                ssr = sqrDist;
                                smallestIndex = index;
                            }
                        }
                    }
                }

                resultIndices.Add(smallestIndex);

                if (resultDistances != null)
                {
                    resultDistances.Add(ssr);
                }
            }
        }

        public class QueryInterval : QueryBase
        {
            public void Query(KDTree_Arrays tree, Vector3 min, Vector3 max, List<int> resultIndices)
            {
                Reset();

                Vector3[] points = tree.points;
                int[] permutations = tree.permutations;

                PushToQueue(tree.rootNode, tree.rootNode.bounds.ClosestPoint(min + max) * 0.5f);

                QueryNode queryNode = null;
                Node node = null;

                while (LeftToProcess > 0)
                {
                    queryNode = PopFromQueue();
                    node = queryNode.node;

                    if (!node.IsLeaf)
                    {
                        int partitionAxis = node.partitionAxis;
                        float partitionCoord = node.partitionCoordinate;

                        Vector3 closestPoint = queryNode.closestPoint;

                        if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                        {
                            PushToQueue(node.negativeChild, closestPoint);

                            if (node.positiveChild.Count != 0 && closestPoint[partitionAxis] <= max[partitionAxis])
                            {
                                PushToQueue(node.positiveChild, closestPoint);
                            }
                        }
                        else
                        {
                            PushToQueue(node.positiveChild, closestPoint);

                            if (node.negativeChild.Count != 0 && closestPoint[partitionAxis] >= min[partitionAxis])
                            {
                                PushToQueue(node.negativeChild, closestPoint);
                            }
                        }
                    }
                    else
                    {
                        if (node.bounds.min.x >= min.x && node.bounds.min.y >= min.y && node.bounds.min.z >= min.z &&
                            node.bounds.max.x <= max.x && node.bounds.max.y <= max.y && node.bounds.max.z <= max.z)
                        {
                            for (int i = node.start; i < node.end; i++)
                            {
                                resultIndices.Add(permutations[i]);
                            }
                        }
                        else
                        {
                            for (int i = node.start; i < node.end; i++)
                            {
                                int index = permutations[i];

                                Vector3 point = points[index];

                                if (point.x >= min.x && point.y >= min.y && point.z >= min.z &&
                                    point.x <= max.x && point.y <= max.y && point.z <= max.z)
                                {
                                    resultIndices.Add(index);
                                }
                            }
                        }
                    }
                }
            }
        }

        public class QueryKNearest : QueryBase
        {
            protected SortedList<int, MinHeap<float/*disk*/, int/*permutation index*/>> heapsList = new SortedList<int, MinHeap<float/*disk*/, int/*permutation index*/>>();

            public void Query(KDTree_Arrays tree, Vector3 point, int k, List<int> resultIndices, List<float> resultDistances = null)
            {
                MinHeap<float/*disk*/, int/*permutation index*/> heap;

                if (!heapsList.TryGetValue(k, out heap) || heap == null)
                {
                    heap = new MinHeap<float/*disk*/, int/*permutation index*/>(k);
                    heapsList.Add(k, heap);
                }

                heap.Clear();
                Reset();

                Vector3[] points = tree.points;
                int[] permutations = tree.permutations;

                // Biggest smallest squared radius
                float bssr = float.MaxValue;

                PushToHeap(tree.rootNode, tree.rootNode.bounds.ClosestPoint(point), point);
                
                QueryNode queryNode = null;
                Node node = null;

                int partitionAxis;
                float partitionCoord;

                Vector3 closestPoint;

                while (minHeap.Count > 0)
                {
                    queryNode = PopFromHeap();

                    if (queryNode.distance > bssr)
                    {
                        continue;
                    }

                    node = queryNode.node;

                    if (!node.IsLeaf)
                    {
                        partitionAxis = node.partitionAxis;
                        partitionCoord = node.partitionCoordinate;

                        closestPoint = queryNode.closestPoint;

                        if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                        {
                            PushToHeap(node.negativeChild, closestPoint, point);
                            closestPoint[partitionAxis] = partitionCoord;

                            if (node.positiveChild.Count != 0)
                            {
                                PushToHeap(node.positiveChild, closestPoint, point);
                            }
                        }
                        else
                        {
                            PushToHeap(node.positiveChild, closestPoint, point);
                            closestPoint[partitionAxis] = partitionCoord;

                            if (node.negativeChild.Count != 0)
                            {
                                PushToHeap(node.negativeChild, closestPoint, point);
                            }
                        }
                    }
                    else
                    {
                        float sqrDist;

                        for (int i = node.start; i < node.end; i++)
                        {
                            int index = permutations[i];
                            sqrDist = Vector3.SqrMagnitude(points[index] - point);

                            if (sqrDist <= bssr)
                            {
                                heap.Add(sqrDist, index);

                                if (heap.Count > k)
                                {
                                    heap.TakeMax();
                                }
                            }
                        }
                    }
                }

                foreach (MinHeap<float,int>.KeyValuePair pair in heap)
                {
                    resultIndices.Add(pair.Value);
                    if (resultDistances != null)
                    {
                        resultDistances.Add(pair.Key);
                    }
                }
            }
        }

        public class QueryRadius : QueryBase
        {
            public void Query(KDTree_Arrays tree, Vector3 queryPoint, float queryRadius, List<int> resultIndices)
            {
                Reset();

                float querySqrDist = queryRadius * queryRadius;

                Vector3[] points = tree.points;
                int[] permutations = tree.permutations;

                PushToQueue(tree.rootNode, tree.rootNode.bounds.ClosestPoint(queryPoint));

                QueryNode queryNode = null;
                Node node = null;

                while (LeftToProcess > 0)
                {
                    queryNode = PopFromQueue();
                    node = queryNode.node;

                    if (!node.IsLeaf)
                    {
                        int partitionAxis = node.partitionAxis;
                        float partitionCoord = node.partitionCoordinate;

                        Vector3 closestPoint = queryNode.closestPoint;

                        if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                        {
                            PushToQueue(node.negativeChild, closestPoint);

                            float sqrDist = Vector3.SqrMagnitude(closestPoint - queryPoint);

                            if (node.positiveChild.Count != 0 && sqrDist <= querySqrDist)
                            {
                                PushToQueue(node.positiveChild, closestPoint);
                            }
                        }
                        else
                        {
                            PushToQueue(node.positiveChild, closestPoint);

                            float sqrDist = Vector3.SqrMagnitude(closestPoint - queryPoint);

                            if (node.negativeChild.Count != 0 && sqrDist <= querySqrDist)
                            {
                                PushToQueue(node.negativeChild, closestPoint);
                            }
                        }
                    }
                    else
                    {
                        for (int i = node.start; i < node.end; i++)
                        {
                            int index = permutations[i];
                            if (Vector3.SqrMagnitude(points[index] - queryPoint) <= querySqrDist)
                            {
                                resultIndices.Add(index);
                            }
                        }
                    }
                }
            }
        }

        public class Node : System.IComparable<Node>
        {
            public int Count { get { return end - start; } }

            public bool IsLeaf { get { return partitionAxis == -1; } }

            // What is the value of this node on the partition axis?
            public float partitionCoordinate;
            // What index of the dimension array is this node's partition coordinate?
            public int partitionAxis = -1;

            public Node negativeChild;
            public Node positiveChild;

            public int start;
            public int end;

            public KDBounds bounds;

            public int CompareTo(Node other)
            {
                return start.CompareTo(other.start);
            }
        }

        public class KDBounds 
        {
            public Vector3 min;
            public Vector3 max;

            public Vector3 Size { get { return max - min; } }

            public Bounds Bounds
            {
                get
                {
                    return new Bounds((min + max) * 0.5f, Size);
                }
            }

            public Vector3 ClosestPoint(Vector3 point)
            {
                point.x = Mathf.Clamp(point.x, min.x, max.x);
                point.y = Mathf.Clamp(point.y, min.y, max.y);
                point.z = Mathf.Clamp(point.z, min.z, max.z);

                return point;
            }
        }

        public int Count
        {
            get { return count; }
        }

        public Vector3[] Points
        {
            get { return points; }
        }

        // The points that make up the tree
        protected Vector3[] points;
        // Remaps the points to allow for faster access
        protected int[] permutations;
        // The number of nodes in the tree
        protected int count = 0;
        // Reference to the root node of the tree
        protected Node rootNode;
        
        // Determines the number of items that can be in a leaf
        protected int leafCapacity = 32;

        // Object pool for nodes
        protected Node[] nodePool;
        protected int nodePoolIndex = 0;

        public KDTree_Arrays(int leafCapacity = 32)
        {
            this.leafCapacity = leafCapacity;

            count = 0;
            points = new Vector3[0];
            permutations = new int[0];

            nodePool = new Node[32];
            nodePoolIndex = 0;
        }

        public KDTree_Arrays(Vector3[] points, int leafCapacity = 32)
        {
            this.leafCapacity = leafCapacity;

            nodePool = new Node[32];
            nodePoolIndex = 0;

            SetPoints(points);
        }

        public void SetPoints(Vector3[] points)
        {
            // Update the points array
            this.points = points;
            // Update the permutation array
            Array.Resize(ref permutations, points.Length);
            // Update the count variable
            count = points.Length;

            Rebuild();
        }

        public void AddPoints(Vector3[] points)
        {
            // Update the points array
            Array.Resize(ref this.points, this.points.Length + points.Length);
            Array.Copy(points, 0, this.points, this.points.Length - points.Length, points.Length);
            // Update the permutation array
            Array.Resize(ref permutations, this.points.Length);
            // Update the count variable
            count = this.points.Length;

            Rebuild();
        }

        public void AddPoint(Vector3 point)
        {
            // Update the points array
            Array.Resize(ref points, points.Length + 1);
            points[points.Length - 1] = point;
            // Update the permutation array
            Array.Resize(ref permutations, points.Length);
            // Update the count variable
            count = points.Length;

            Rebuild();
        }

        public void Rebuild()
        {
            // Reset the permutation array
            for (int i = 0; i < count; i++)
            {
                permutations[i] = i;
            }

            BuildTree();
        }

        protected void BuildTree()
        {
            // Reset position in the node pool
            nodePoolIndex = 0;
            
            rootNode = GetNode();
            rootNode.bounds = DetermineRootBounds();
            rootNode.start = 0;
            rootNode.end = count;

            SplitNode_ByLongest(rootNode);
        }

        protected void SplitNode_ByLongest(Node parent)
        {
            KDBounds parentBounds = parent.bounds;
            Vector3 parentSize = parentBounds.Size;

            // Find the axis where the bounds are the largest
            int splitAxis = 0;
            float axisSize = parentSize.x;

            if (axisSize < parentSize.y)
            {
                splitAxis = 1;
                axisSize = parentSize.y;
            }
            if (axisSize < parentSize.z)
            {
                splitAxis = 2;
                axisSize = parentSize.z;
            }

            // Determine the axis min and max
            float axisMin = parentBounds.min[splitAxis];
            float axisMax = parentBounds.max[splitAxis];

            float splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, splitAxis);

            parent.partitionAxis = splitAxis;
            parent.partitionCoordinate = splitPivot;

            // Partition the points into the negative and positive children
            int splitIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);

            if (splitIndex == parent.start || splitIndex == parent.end)
            {
                switch (splitAxis)
                {
                    case 0:
                        axisMin = parentBounds.min.y;
                        axisMax = parentBounds.max.y;

                        splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 1);

                        parent.partitionAxis = 1;
                        parent.partitionCoordinate = splitPivot;

                        splitIndex = Partition(parent.start, parent.end, splitPivot, 1);

                        if (splitIndex == parent.start || splitIndex == parent.end)
                        {
                            axisMin = parentBounds.min.z;
                            axisMax = parentBounds.max.z;

                            splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 2);

                            parent.partitionAxis = 2;
                            parent.partitionCoordinate = splitPivot;

                            splitIndex = Partition(parent.start, parent.end, splitPivot, 2);
                        }
                        break;
                    case 1:
                        axisMin = parentBounds.min.z;
                        axisMax = parentBounds.max.z;

                        splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 2);

                        parent.partitionAxis = 2;
                        parent.partitionCoordinate = splitPivot;

                        splitIndex = Partition(parent.start, parent.end, splitPivot, 2);

                        if (splitIndex == parent.start || splitIndex == parent.end)
                        {
                            axisMin = parentBounds.min.x;
                            axisMax = parentBounds.max.x;

                            splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 0);

                            parent.partitionAxis = 0;
                            parent.partitionCoordinate = splitPivot;

                            splitIndex = Partition(parent.start, parent.end, splitPivot, 0);
                        }
                        break;
                    case 2:
                        axisMin = parentBounds.min.x;
                        axisMax = parentBounds.max.x;

                        splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 0);

                        parent.partitionAxis = 0;
                        parent.partitionCoordinate = splitPivot;

                        splitIndex = Partition(parent.start, parent.end, splitPivot, 0);

                        if (splitIndex == parent.start || splitIndex == parent.end)
                        {
                            axisMin = parentBounds.min.y;
                            axisMax = parentBounds.max.y;
                            
                            splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, 1);

                            parent.partitionAxis = 1;
                            parent.partitionCoordinate = splitPivot;

                            splitIndex = Partition(parent.start, parent.end, splitPivot, 1);
                        }
                        break;
                }
            }

            if (splitIndex == parent.start || splitIndex == parent.end)
            {
                Debug.LogError("KDTree_Arrays.SplitNode_ByLongest: Split index is equal to start or end. This should never happen.");
                return;
            }

            // Create the negative child
            Vector3 negativeMin = parentBounds.min;
            Vector3 negativeMax = parentBounds.max;
            negativeMax[splitAxis] = splitPivot;

            Node negativeChild = GetNode();
            negativeChild.bounds = new KDBounds() { min = negativeMin, max = negativeMax };
            negativeChild.start = parent.start;
            negativeChild.end = splitIndex;
            parent.negativeChild = negativeChild;

            // Create the positive child
            Vector3 positiveMin = parentBounds.min;
            Vector3 positiveMax = parentBounds.max;
            positiveMin[splitAxis] = splitPivot;

            Node positiveChild = GetNode();
            positiveChild.bounds = new KDBounds() { min = positiveMin, max = positiveMax };
            positiveChild.start = splitIndex;
            positiveChild.end = parent.end;
            parent.positiveChild = positiveChild;

            if (ContinueSplit(negativeChild))
            {
                SplitNode_ByLongest(negativeChild);
            }
            if (ContinueSplit(positiveChild))
            {
                SplitNode_ByLongest(positiveChild);
            }
        }

        protected bool ContinueSplit(Node node)
        {
            return node.Count > leafCapacity;
        }

        protected int Partition(int start, int end, float partitionPivot, int axis)
        {
            int leftIndex = start - 1;
            int rightIndex = end;

            int temp;

            for (;;)
            {
                do {
                    leftIndex++;
                } while (leftIndex < rightIndex && points[permutations[leftIndex]][axis] < partitionPivot);

                do {
                    rightIndex--;
                } while (leftIndex < rightIndex && points[permutations[rightIndex]][axis] >= partitionPivot);

                if (leftIndex < rightIndex)
                {
                    // Swap the points
                    temp = permutations[leftIndex];
                    permutations[leftIndex] = permutations[rightIndex];
                    permutations[rightIndex] = temp;
                }
                else
                {
                    return leftIndex;
                }
            }
        }

        // Uses a sliding midpoint rule to determine the split axis
        protected float CalculateSplitPivot(int start, int end, float axisMin, float axisMax, int splitAxis)
        {
            float midpoint = (axisMin + axisMax) * 0.5f;

            // Flag to determine if a node is in each section of the midpoint. Both must be true to exit the loop
            bool negative = false;
            bool positive = false;

            // Loop through the points to determine the split pivot
            for (int i = start; i < end; i++)
            {
                float value = points[permutations[i]][splitAxis];

                if (value < midpoint)
                {
                    negative = true;
                }
                else
                {
                    positive = true;
                }

                if (negative && positive)
                {
                    return midpoint;
                }
            }

            if (negative)
            {
                float max = float.MinValue;
                for (int i = 0; i < end; i++) 
                {
                    float value = points[permutations[i]][splitAxis];
                    if (value > max) max = value;
                }

                return max;
            }
            else
            {
                float min = float.MaxValue;
                for (int i = 0; i < end; i++) 
                {
                    float value = points[permutations[i]][splitAxis];
                    if (value < min) min = value;
                }

                return min;
            }
        }

        protected KDBounds DetermineRootBounds()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            int evenCount = count >> 1; // calculate even Length

            // min, max calculations
            // 3n/2 calculations instead of 2n
            for (int i = 0; i < evenCount; i += 2) 
            {
                int j = i + i;

                if (points[i].x > points[j].x)
                {
                    if (points[i].x > max.x) max.x = points[i].x;
                    if (points[j].x < min.x) min.x = points[j].x;
                }
                else
                {
                    if (points[j].x > max.x) max.x = points[j].x;
                    if (points[i].x < min.x) min.x = points[i].x;
                }

                if (points[i].y > points[j].y)
                {
                    if (points[i].y > max.y) max.y = points[i].y;
                    if (points[j].y < min.y) min.y = points[j].y;
                }
                else
                {
                    if (points[j].y > max.y) max.y = points[j].y;
                    if (points[i].y < min.y) min.y = points[i].y;
                }

                if (points[i].z > points[j].z)
                {
                    if (points[i].z > max.z) max.z = points[i].z;
                    if (points[j].z < min.z) min.z = points[j].z;
                }
                else
                {
                    if (points[j].z > max.z) max.z = points[j].z;
                    if (points[i].z < min.z) min.z = points[i].z;
                }
            }

            // If the count is odd, we need to check the last point
            if ((count & 1) == 1)
            {
                if (points[count - 1].x > max.x) max.x = points[count - 1].x;
                if (points[count - 1].x < min.x) min.x = points[count - 1].x;

                if (points[count - 1].y > max.y) max.y = points[count - 1].y;
                if (points[count - 1].y < min.y) min.y = points[count - 1].y;

                if (points[count - 1].z > max.z) max.z = points[count - 1].z;
                if (points[count - 1].z < min.z) min.z = points[count - 1].z;
            }

            return new KDBounds() { min = min, max = max };
        }

        protected Node GetNode()
        {
            if (nodePoolIndex < nodePool.Length) 
            {
                if (nodePool[nodePoolIndex] == null)
                {
                    nodePool[nodePoolIndex] = new Node();
                }
            }
            else 
            {
                Array.Resize(ref nodePool, nodePool.Length + 32);
                nodePool[nodePoolIndex] = new Node();
            }

            return nodePool[nodePoolIndex++];
        }
    }    
}