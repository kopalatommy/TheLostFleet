#if UNITY_STANDALONE

using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;
using GalacticBoundStudios.DataScribes.Managed.Trees; // Make sure this matches your namespace

/// <summary>
/// A MonoBehaviour script to run validation tests on the NativeKDTree implementation.
/// It compares the results of KD-Tree queries against brute-force methods to ensure correctness.
/// </summary>
public class NativeKDTree_Validation : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private int numPoints = 10000;
    [SerializeField] private float worldSize = 100f;
    [SerializeField] private int randomSeed = 12345;

    [Header("Query Parameters")]
    [SerializeField] private float queryRadius = 10f;
    [SerializeField] private int kNearest = 15;

    void Start()
    {
        Debug.Log("--- Starting NativeKDTree Validation ---");

        // Generate a consistent set of random points for testing
        var points = new NativeArray<float3>(numPoints, Allocator.Persistent);
        var random = new Unity.Mathematics.Random((uint)randomSeed);
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = random.NextFloat3(-worldSize / 2, worldSize / 2);
        }

        var queryPoint = random.NextFloat3(-worldSize / 2, worldSize / 2);
        var queryMin = queryPoint - new float3(queryRadius, queryRadius, queryRadius);
        var queryMax = queryPoint + new float3(queryRadius, queryRadius, queryRadius);

        // --- Run all tests for both build strategies ---
        RunAllTests(BuildStrategy.Midpoint, points, queryPoint, queryRadius, kNearest, queryMin, queryMax);
        RunAllTests(BuildStrategy.Median, points, queryPoint, queryRadius, kNearest, queryMin, queryMax);

        // --- Test Edge Cases ---
        TestEdgeCases();

        Debug.Log("--- NativeKDTree Validation Complete ---");

        points.Dispose();
    }

    private void RunAllTests(BuildStrategy strategy, NativeArray<float3> points, float3 queryPoint, float radius, int k, float3 queryMin, float3 queryMax)
    {
        Debug.Log($"\n--- Testing with Build Strategy: {strategy} ---");

        using var kdTree = new NativeKDTree(Allocator.Persistent, strategy);
        kdTree.SetPoints(points);

        TestRadiusQuery(kdTree, points, queryPoint, radius);
        TestKNearestQuery(kdTree, points, queryPoint, k);
        TestIntervalQuery(kdTree, points, queryMin, queryMax);
    }

    /// <summary>
    /// Tests the QueryRadius function for correctness.
    /// </summary>
    private void TestRadiusQuery(NativeKDTree kdTree, NativeArray<float3> points, float3 queryPoint, float radius)
    {
        // 1. Get results from the KD-Tree
        var kdTreeResults = new NativeList<int>(Allocator.Temp);
        kdTree.QueryRadius(queryPoint, radius, kdTreeResults);
        var kdTreeResultsSorted = kdTreeResults.AsArray().ToList();
        kdTreeResultsSorted.Sort();
        kdTreeResults.Dispose();

        // 2. Get results via brute-force for ground truth
        var bruteForceResults = new List<int>();
        float radiusSq = radius * radius;
        for (int i = 0; i < points.Length; i++)
        {
            if (math.lengthsq(points[i] - queryPoint) <= radiusSq)
            {
                bruteForceResults.Add(i);
            }
        }
        bruteForceResults.Sort();

        // 3. Compare results
        bool success = kdTreeResultsSorted.SequenceEqual(bruteForceResults);
        Debug.Log($"[Radius Query] Test Passed: {success}. Found {kdTreeResultsSorted.Count} points.");
        if (!success)
        {
            Debug.LogError($"Radius Query Mismatch! KDTree got {kdTreeResultsSorted.Count}, BruteForce got {bruteForceResults.Count}");
        }
    }

    /// <summary>
    /// Tests the QueryKNearest function for correctness.
    /// </summary>
    private void TestKNearestQuery(NativeKDTree kdTree, NativeArray<float3> points, float3 queryPoint, int k)
    {
        // 1. Get results from the KD-Tree
        var kdTreeResults = new NativeList<int>(Allocator.Temp);
        kdTree.QueryKNearest(queryPoint, k, kdTreeResults);
        // The results from QueryKNearest are already sorted by the implementation.
        var kdTreeResultsList = kdTreeResults.AsArray().ToList();
        kdTreeResults.Dispose();

        // 2. Get results via brute-force for ground truth
        var bruteForceResults = new List<int>();
        var allDistances = new List<(float, int)>();
        for (int i = 0; i < points.Length; i++)
        {
            allDistances.Add((math.lengthsq(points[i] - queryPoint), i));
        }

        // Sort by distance and take the top k
        var sortedDistances = allDistances.OrderBy(d => d.Item1).Take(k).ToList();
        bruteForceResults = sortedDistances.Select(d => d.Item2).ToList();
        bruteForceResults.Sort(); // Sort indices for comparison

        // 3. Compare results
        bool success = kdTreeResultsList.SequenceEqual(bruteForceResults);
        Debug.Log($"[K-Nearest Query] Test Passed: {success}. Found {kdTreeResultsList.Count} points for k={k}.");
        if (!success)
        {
            Debug.LogError($"K-Nearest Query Mismatch! KDTree got {kdTreeResultsList.Count}, BruteForce got {bruteForceResults.Count}");
        }
    }

    /// <summary>
    /// Tests the QueryInterval function for correctness.
    /// </summary>
    private void TestIntervalQuery(NativeKDTree kdTree, NativeArray<float3> points, float3 queryMin, float3 queryMax)
    {
        // 1. Get results from the KD-Tree
        var kdTreeResults = new NativeList<int>(Allocator.Temp);
        kdTree.QueryInterval(queryMin, queryMax, kdTreeResults);
        var kdTreeResultsSorted = kdTreeResults.AsArray().ToList();
        kdTreeResultsSorted.Sort();
        kdTreeResults.Dispose();

        // 2. Get results via brute-force for ground truth
        var bruteForceResults = new List<int>();
        for (int i = 0; i < points.Length; i++)
        {
            if (math.all(points[i] >= queryMin) && math.all(points[i] <= queryMax))
            {
                bruteForceResults.Add(i);
            }
        }
        bruteForceResults.Sort();

        // 3. Compare results
        bool success = kdTreeResultsSorted.SequenceEqual(bruteForceResults);
        Debug.Log($"[Interval Query] Test Passed: {success}. Found {kdTreeResultsSorted.Count} points.");
        if (!success)
        {
            Debug.LogError($"Interval Query Mismatch! KDTree got {kdTreeResultsSorted.Count}, BruteForce got {bruteForceResults.Count}");
        }
    }

    /// <summary>
    /// Tests various edge cases like empty trees or zero-k queries.
    /// </summary>
    private void TestEdgeCases()
    {
        Debug.Log($"\n--- Testing Edge Cases ---");

        // Test with an empty set of points
        using var emptyPoints = new NativeArray<float3>(0, Allocator.Persistent);
        using var kdTree = new NativeKDTree(Allocator.Persistent);
        kdTree.SetPoints(emptyPoints);

        var results = new NativeList<int>(Allocator.Temp);

        kdTree.QueryRadius(float3.zero, 10f, results);
        bool emptyRadius = results.Length == 0;

        kdTree.QueryKNearest(float3.zero, 5, results);
        bool emptyKNearest = results.Length == 0;

        kdTree.QueryInterval(float3.zero, new float3(1, 1, 1), results);
        bool emptyInterval = results.Length == 0;

        results.Dispose();

        Debug.Log($"[Edge Case] Empty Tree Queries Passed: {emptyRadius && emptyKNearest && emptyInterval}");
        if (!(emptyRadius && emptyKNearest && emptyInterval))
        {
            Debug.LogError("Querying an empty tree did not return an empty result list.");
        }
    }
}

#endif // UNITY_STANDALONE
