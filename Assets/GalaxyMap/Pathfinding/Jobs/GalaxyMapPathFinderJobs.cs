using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using GalacticBoundStudios.HexTech;
using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed;
using Unity.Burst;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public struct GalaxyMapPathFinderJob : IJob
    {
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

        public GalaxyMapPathRequest request;
        public NativeList<HexCoord> resultPath;
        public Entity requester;

        [ReadOnly]
        public NativeHashMap<HexCoord, float> costMap;

        public void Execute()
        {
            Debug.Log("Finding path from: " + request.start + " -> " + request.target);

            FindPath(request.start, request.target);
        }

        public void FindPath(HexCoord start, HexCoord end)
        {
            NativePriorityQueue<PathStep> openQueue = new NativePriorityQueue<PathStep>(Allocator.Temp);
            NativeHashMap<HexCoord, HexCoord> pathStepMap = new NativeHashMap<HexCoord, HexCoord>(128, Allocator.Temp);

            NativeArray<HexCoord> neighborsArray = new NativeArray<HexCoord>(6, Allocator.Temp);

            openQueue.Add(new PathStep
            {
                step = start,
                cost = 0
            });

            while (!openQueue.IsEmpty)
            {
                PathStep currentStep = openQueue.Dequeue();

                if (currentStep.step.Equals(end))
                {
                    RebuildPath(start, end, pathStepMap);

                    openQueue.Dispose();
                    pathStepMap.Dispose();
                    neighborsArray.Dispose();

                    return;
                }

                GetNeighbors(currentStep.step, neighborsArray);

                // Add each neighbor to the queue
                for (int i = 0; i < 6; i++)
                {
                    // Skip nodes that have already been accessed
                    if (pathStepMap.ContainsKey(neighborsArray[i]))
                    {
                        continue;
                    }
                    if (!costMap.ContainsKey(neighborsArray[i]))
                    {
                        continue;
                    }

                    float cost = currentStep.cost + Heuristic(neighborsArray[i], end);
                    openQueue.Add(new PathStep()
                    {
                        step = neighborsArray[i],
                        cost = cost,
                    });
                    // Create a map between the current step and it's predecessor
                    pathStepMap.Add(neighborsArray[i], currentStep.step);

                    // Resize the path map if at capacity
                    if (pathStepMap.Capacity == pathStepMap.Count)
                    {
                        pathStepMap.Capacity += 128;
                    }
                }
            }

            openQueue.Dispose();
            pathStepMap.Dispose();
            neighborsArray.Dispose();
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

        private void RebuildPath(HexCoord start, HexCoord goal, NativeHashMap<HexCoord, HexCoord> pathStepMap)
        {
            // Rebuild the path from end to start, until the current node is the start
            while (!goal.Equals(start))
            {
                // Add the current node to the path
                resultPath.Add(goal);
                // Get the predecessor
                HexCoord next = pathStepMap[goal];
                goal = next;
            }
            // Add the start node because it has not yet been added
            resultPath.Add(start);
            // Reverse the path because it was built in reverse
            ReverseList(resultPath);
        }

        private void ReverseList(NativeList<HexCoord> list)
        {
            int s = 0;
            int e = list.Length - 1;

            while (s < e)
            {
                HexCoord t = list[s];
                list[s] = list[e];
                list[e] = t;
                s++;
                e--;
            }
        }
    }
}