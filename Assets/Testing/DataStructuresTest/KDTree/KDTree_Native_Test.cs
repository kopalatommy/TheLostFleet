using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using System.Collections.Generic;
using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Unmanaged;
using Unity.Collections;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class KDTree_Native_Tester : MonoBehaviour
{
    protected enum QueryType
    {
        Radius,
        KNearest,
        Interval
    }

    [Header("Configuration")]
    [SerializeField]
    private float radius = 10f;
    [SerializeField]
    private int numPoints = 20000;
    [SerializeField]
    private QueryType queryType = QueryType.Radius;

    private NativeArray<float3> points = default;
    private NativeKDTree kDTree = default;

    private float radiusQueryRadius = 0.1f;
    private float3 radiusQueryCenter = Vector3.zero;

    private int kNearestQueryK = 1;
    private float3 kNearestQueryPoint = Vector3.zero;

    private float3 intervalQueryMin = Vector3.zero;
    private float3 intervalQueryMax = Vector3.zero;
    private int axisUpdateIndex = 0;

    void Start()
    {
        InitializePoints();
        InitializeTree();

        intervalQueryMin = intervalQueryMax = new Vector3(radius, radius, radius);

        switch (queryType)
        {
        case QueryType.Radius:
            UpdateRadiusQuery();
            break;
        case QueryType.Interval:
            UpdateIntervalQuery();
            break;
        case QueryType.KNearest:
            UpdateKNearestQuery();
            break;
        }
    }

    private void InitializePoints()
    {
        if (!points.IsCreated)
        {
            points = new NativeArray<float3>(numPoints, Allocator.Persistent);
        }
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = new float3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
        }
    }

    private void InitializeTree()
    {
        if (!kDTree.IsCreated)
        {
            kDTree = new NativeKDTree(Allocator.Persistent);
        }
        kDTree.SetPoints(points);
    }

    private void UpdateRadiusQuery()
    {
        radiusQueryRadius += Time.deltaTime;
        if (radiusQueryRadius > radius)
        {
            radiusQueryRadius = 0.1f;
            radiusQueryCenter = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));

            // InitializePoints();
            // InitializeTree();
        }
    }

    private void UpdateKNearestQuery()
    {
        kNearestQueryK++;
        if (kNearestQueryK > numPoints)
        {
            kNearestQueryK = 1;
            kNearestQueryPoint = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
        }
    }

    private void UpdateIntervalQuery()
    {
        switch (axisUpdateIndex)
        {
        case 0:
            intervalQueryMin.x += Time.deltaTime;
            intervalQueryMax.x += Time.deltaTime;
            if (intervalQueryMax.x > radius)
            {
                intervalQueryMin.x = -radius;
                intervalQueryMax.x = radius;
                intervalQueryMin.y = -radius;
                intervalQueryMax.y = -radius + 1;
                intervalQueryMin.z = -radius;
                intervalQueryMax.z = radius;
                axisUpdateIndex++;
            }
            break;

        case 1:
            intervalQueryMin.y += Time.deltaTime;
            intervalQueryMax.y += Time.deltaTime;
            if (intervalQueryMax.y > radius)
            {
                intervalQueryMin.x = -radius;
                intervalQueryMax.x = radius;
                intervalQueryMin.y = -radius;
                intervalQueryMax.y = radius;
                intervalQueryMin.z = -radius;
                intervalQueryMax.z = -radius + 1;
                axisUpdateIndex++;
            }
            break;

        case 2:
            intervalQueryMin.z += Time.deltaTime;
            intervalQueryMax.z += Time.deltaTime;
            if (intervalQueryMax.z > radius)
            {
                intervalQueryMin.x = -radius;
                intervalQueryMax.x = -radius + 1;
                intervalQueryMin.y = -radius;
                intervalQueryMax.y = radius;
                intervalQueryMin.z = -radius;
                intervalQueryMax.z = radius;
                axisUpdateIndex = 0;
            }
            break;
        }
    }
    
    void Update()
    {
        switch (queryType)
        {
        case QueryType.Radius:
            UpdateRadiusQuery();
            break;
        case QueryType.Interval:
            UpdateIntervalQuery();
            break;
        // case QueryType.KNearest:
        //     UpdateKNearestQuery();
        //     break;
        }
    }

    void OnDrawGizmos()
    {
        if (points == null)
        {
            return;
        }

        Gizmos.color = Color.white;
        Vector3 size = Vector3.one * 0.1f;
        Gizmos.color = Color.red;
        foreach (Vector3 point in points)
        {
            Gizmos.DrawCube(point, size);
        }

        NativeList<int> result = new NativeList<int>(Allocator.TempJob);
        switch (queryType)
        {
        case QueryType.Radius:
            kDTree.QueryRadius(radiusQueryCenter, radiusQueryRadius, result);
            break;

        case QueryType.Interval:
            kDTree.QueryInterval(intervalQueryMin, intervalQueryMax, result);
            break;

        case QueryType.KNearest:
            UpdateKNearestQuery();
            kDTree.QueryKNearest(kNearestQueryPoint, kNearestQueryK, result);
            break;
        }

        Gizmos.color = Color.green;
        foreach (var point in result)
        {
            Gizmos.DrawSphere(points[point], 0.2f);
        }

        result.Dispose();
    }
}