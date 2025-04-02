using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using System.Collections.Generic;
using GalacticBoundStudios.DataScribes.Managed.Lists;

public class KDTree_Array_Tester : MonoBehaviour
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

    private Vector3[] points = null;
    private KDTree_Arrays kDTree = null;

    private KDTree_Arrays.QueryRadius radiusQuery = new KDTree_Arrays.QueryRadius();
    private float radiusQueryRadius = 0.1f;
    private Vector3 radiusQueryCenter = Vector3.zero;

    private KDTree_Arrays.QueryKNearest kNearestQuery = new KDTree_Arrays.QueryKNearest();
    private int kNearestQueryK = 1;
    private Vector3 kNearestQueryPoint = Vector3.zero;

    private KDTree_Arrays.QueryInterval intervalQuery = new KDTree_Arrays.QueryInterval();
    private Vector3 intervalQueryMin = Vector3.zero;
    private Vector3 intervalQueryMax = Vector3.zero;
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
        if (points == null)
        {
            points = new Vector3[numPoints];
        }
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
        }
    }

    private void InitializeTree()
    {
        if (kDTree == null)
        {
            kDTree = new KDTree_Arrays();
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

            InitializePoints();
            InitializeTree();
        }
    }

    private void UpdateKNearestQuery()
    {
        kNearestQueryK++;
        if (kNearestQueryK > numPoints)
        {
            kNearestQueryK = 1;
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

        List<int> resultIndices = new List<int>();
        switch (queryType)
        {
        case QueryType.Radius:
            radiusQuery.Query(kDTree, radiusQueryCenter, radiusQueryRadius, resultIndices);
            break;

        case QueryType.Interval:
            intervalQuery.Query(kDTree, intervalQueryMin, intervalQueryMax, resultIndices);
            break;

        case QueryType.KNearest:
            UpdateKNearestQuery();
            kNearestQuery.Query(kDTree, kNearestQueryPoint, kNearestQueryK, resultIndices);
            break;
        }

        Gizmos.color = Color.green;
        foreach (int index in resultIndices)
        {
            Gizmos.DrawSphere(points[index], 0.2f);
        }
    }
}