using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using System.Collections.Generic;
using GalacticBoundStudios.DataScribes.Managed.Lists;

public class KDTree_Standard_Tester : MonoBehaviour
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
    private KDTree<int> kDTree = null;

    private float radiusQueryRadius = 0.1f;
    private Vector3 radiusQueryCenter = Vector3.zero;

    private int kNearestQueryK = 1;
    private Vector3 kNearestQueryPoint = Vector3.zero;

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
            kDTree = new KDTree<int>(3);
        }
        kDTree.Clear();
        for (int i = 0; i < numPoints; i++)
        {
            kDTree.Add(i, new double[] { points[i].x, points[i].y, points[i].z });
        }
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

        List<int> resultIndices = new List<int>();
        OrderedList<float, int> queryPoints = null;
        switch (queryType)
        {
        case QueryType.Radius:
            queryPoints = kDTree.RadialSearch(new double[] { radiusQueryCenter.x, radiusQueryCenter.y, radiusQueryCenter.z }, radiusQueryRadius);
            break;

        case QueryType.Interval:
            queryPoints = kDTree.IntervalQuery(new double[] { intervalQueryMin.x, intervalQueryMin.y, intervalQueryMin.z }, new double[] { intervalQueryMax.x, intervalQueryMax.y, intervalQueryMax.z });
            break;

        case QueryType.KNearest:
            UpdateKNearestQuery();
            queryPoints = kDTree.GetNearestNeighbors(new double[] { kNearestQueryPoint.x, kNearestQueryPoint.y, kNearestQueryPoint.z }, kNearestQueryK);
            break;
        }

        if (queryPoints == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var point in queryPoints)
        {
            Gizmos.DrawSphere(points[point.Value], 0.2f);
        }
    }
}