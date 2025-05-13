using System.Linq;
using GalacticBoundStudios.DataScribes.Managed;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    [BurstCompile]
    // Uses the A* algorithm to build a path between the given nodes
    public partial struct HexTechCreatePathJob : IJobEntity
    {
        [BurstCompile]
        struct PathStep : System.IComparable<PathStep>, System.IEquatable<PathStep>
        {
            public HexCoord step;
            public float cost;

            [BurstCompile]
            public int CompareTo(PathStep obj)
            {
                return cost.CompareTo(obj.cost);
            }

            [BurstCompile]
            public bool Equals(PathStep other)
            {
                return step.Equals(other.step);
            }
        }

        [ReadOnly]
        public NativeHashMap<HexCoord, float> costMap;

        public EntityCommandBuffer.ParallelWriter entityCommandBuffer;

        public void Execute(HexTechCreatePathAspect aspect)
        {
            NativeList<HexCoord> path = FindPath(aspect.pathRequest.ValueRO.startPos, aspect.pathRequest.ValueRO.endPos);
            
            entityCommandBuffer.RemoveComponent<HexTechCreatePathRequest>(aspect.entity.Index, aspect.entity);
            entityCommandBuffer.AddComponent<HexTechMapPath>(aspect.entity.Index, aspect.entity, new HexTechMapPath()
            {
                path = path
            });
        }

        public NativeList<HexCoord> FindPath(HexCoord start, HexCoord goal)
        {
            NativePriorityQueue<PathStep> openQueue = new NativePriorityQueue<PathStep>(Allocator.TempJob);
            NativeHashMap<HexCoord, HexCoord> pathStepMap = new NativeHashMap<HexCoord, HexCoord>(128, Allocator.TempJob);

            NativeArray<HexCoord> neighborsArray = new NativeArray<HexCoord>(6, Allocator.TempJob);

            while (!openQueue.IsEmpty) {
                PathStep currentStep = openQueue.Dequeue();

                if (currentStep.step.Equals(goal)) {
                    openQueue.Dispose();
                    pathStepMap.Dispose();
                    neighborsArray.Dispose();
                    return RebuildPath(start, goal, pathStepMap);
                }

                GetNeighbors(currentStep.step, neighborsArray);

                // Add each neighbor to the queue
                for (int i = 0; i < 6; i++)
                {
                    // Skip nodes that have already been accessed
                    if (pathStepMap.ContainsKey(neighborsArray[i])) {
                        continue;
                    }

                    float cost = currentStep.cost + Heuristic(neighborsArray[i], goal);
                    openQueue.Add(new PathStep()
                    {
                        step = neighborsArray[i],
                        cost = cost + currentStep.cost + costMap[neighborsArray[i]],
                    });
                    // Create a map between the current step and it's predecessor
                    pathStepMap.Add(neighborsArray[i], currentStep.step);

                    // Resize the path map if at capacity
                    if (pathStepMap.Capacity == pathStepMap.Count) {
                        pathStepMap.Capacity += 128;
                    }
                }
            }

            openQueue.Dispose();
            pathStepMap.Dispose();
            neighborsArray.Dispose();

            return new NativeList<HexCoord>(Allocator.Persistent);
        }

        private float Heuristic(HexCoord coord, HexCoord goal)
        {
            return HexMath.Distance(coord, goal);
        }

        private void GetNeighbors(HexCoord coord, NativeArray<HexCoord> neighborsArray)
        {
            neighborsArray[0] = HexMath.Add(coord, new HexCoord(0, 1));
            neighborsArray[1] = HexMath.Add(coord, new HexCoord(0, -1));
            neighborsArray[2] = HexMath.Add(coord, new HexCoord(1, 0));
            neighborsArray[3] = HexMath.Add(coord, new HexCoord(-1, 0));
            neighborsArray[4] = HexMath.Add(coord, new HexCoord(1, -1));
            neighborsArray[5] = HexMath.Add(coord, new HexCoord(-1, 1));
        }

        private NativeList<HexCoord> RebuildPath(HexCoord start, HexCoord goal, NativeHashMap<HexCoord,HexCoord> pathStepMap)
        {
            NativeList<HexCoord> path = new NativeList<HexCoord>(Allocator.Persistent);

            // Rebuild the path from end to start, until the current node is the start
            while (!goal.Equals(start))
            {
                // Add the current node to the path
                path.Add(goal);
                // Get the predecessor
                goal = pathStepMap[goal];
            }
            // Add the start node because it has not yet been added
            path.Add(start);
            // Reverse the path because it was built in reverse
            path.Reverse();
            // Return the resulting path
            return path;
        }
    }
}