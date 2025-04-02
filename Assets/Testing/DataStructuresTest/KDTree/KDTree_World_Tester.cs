using UnityEngine;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using System.Collections.Generic;
using GalacticBoundStudios.DataScribes.Managed.Lists;

public class KDTree_World_Tester : MonoBehaviour
{
    // protected enum QueryType
    // {
    //     Radius,
    //     KNearest,
    //     Interval
    // }

    // protected enum TreeType
    // {
    //     Standard,
    //     Arrays,
    //     Native
    // }

    // [Header("Configuration")]
    // [SerializeField]
    // private float radius = 10f;
    // [SerializeField]
    // private int numPoints = 20000;
    // [SerializeField]
    // private QueryType queryType = QueryType.Radius;
    // [SerializeField]
    // private TreeType treeType = TreeType.Arrays;

    


















    // private Vector3[] points;
    // private KDTree_Arrays tree_array;
    // private KDTree<int> tree_standard;
    // private NativeKDTree tree_native;

    // [Header("Query")]
    // [SerializeField]
    // protected bool useStandardKDTree = false;
    // [SerializeField]
    // protected QueryType queryType = QueryType.Radius;

    // #region Radius Query

    // #region KDTree_Arrays

    // KDTree_Arrays.QueryRadius radiusQuery;

    // #endregion // KDTree_Arrays

    // private Vector3 radiusQueryPoint = Vector3.zero;
    // private float radiusQueryRadius = 1f;

    // #endregion // Radius Query

    // #region K Nearest Query

    // #region KDTree_Arrays

    // KDTree_Arrays.QueryKNearest kNearestQuery;

    // #endregion // KDTree_Arrays

    // private Vector3 kNearestQueryPoint = Vector3.zero;
    // private int kNearestQueryCount = 10;

    // #endregion // K Nearest Query

    // #region Interval Query

    // #region KDTree_Arrays

    // KDTree_Arrays.QueryInterval intervalQuery;

    // #endregion // KDTree_Arrays

    // private Vector3 intervalQueryMinBounds = new Vector3(-10, 0, -10);
    // private Vector3 intervalQueryMaxBounds = new Vector3(10, 1, 10);
    // private bool intervalQueryHorizontal = true;

    // #endregion // Interval Query

    // private void InitializePoints()
    // {
    //     if (points == null)
    //     {
    //         points = new Vector3[numPoints];
    //     }

    //     for (int i = 0; i < numPoints; i++)
    //     {
    //         points[i] = new Vector3(Random.value, Random.value, Random.value) * radius;
    //     }
    // }

    // private void InitializeTree_Array()
    // {
    //     if (tree_array == null)
    //     {
    //         tree_array = new KDTree_Arrays();
    //     }
    //     tree_array.SetPoints(points);
    // }

    // private void InitializeTree_Standard()
    // {
    //     if (tree_standard == null)
    //     {
    //         tree_standard = new KDTree<int>();
    //     }
    //     tree_standard.Clear();
    //     for (int i = 0; i < points.Length; i++)
    //     {
    //         tree_standard.Add(i, new double[] { points[i].x, points[i].y, points[i].z });
    //     }
    // }

    // private void InitializeTree_Native()
    // {
    //     if (!tree_native.IsCreated)
    //     {
    //         tree_native = new NativeKDTree(Unity.Collections.Allocator.Persistent);
    //     }
    //     tree_native.SetPoints(points);
    // }

    // private void InitializeRadiusQuery()
    // {
    //     radiusQuery = new KDTree_Arrays.QueryRadius();
    // }

    // private void RadiusQueryUpdate()
    // {
    //     radiusQueryRadius += Time.deltaTime;

    //     if (radiusQueryRadius > radius)
    //     {
    //         InitializePoints();
    //         if (useStandardKDTree)
    //         {
    //             InitializeTree_Standard();
    //         }
    //         else
    //         {
    //             InitializeTree_Array();
    //         }

    //         radiusQueryRadius = 1f;

    //         radiusQueryPoint = Random.insideUnitSphere * radius;
    //     }
    // }

    // private void InitializeKNearestQuery()
    // {
    //     kNearestQuery = new KDTree_Arrays.QueryKNearest();
    // }

    // private void KNearestQueryUpdate()
    // {
    //     kNearestQueryCount += 10;
    //     if (kNearestQueryCount > points.Length)
    //     {
    //         InitializePoints();
    //         if (useStandardKDTree)
    //         {
    //             InitializeTree_Standard();
    //         }
    //         else
    //         {
    //             InitializeTree_Array();
    //         }

    //         kNearestQueryCount = 10;
    //     }
    // }

    // private void InitializeIntervalQuery()
    // {
    //     intervalQuery = new KDTree_Arrays.QueryInterval();

    //     intervalQueryMinBounds = new Vector3(-radius, -radius, -radius);
    //     intervalQueryMaxBounds = new Vector3(radius, radius, radius);
    //     intervalQueryHorizontal = true;
    // }

    // private void IntervalQueryUpdate()
    // {
    //     if (intervalQueryHorizontal)
    //     {
    //         intervalQueryMinBounds.x += Time.deltaTime;
    //         intervalQueryMaxBounds.x += Time.deltaTime;
    //         if (intervalQueryMinBounds.x > radius)
    //         {
    //             intervalQueryHorizontal = false;

    //             InitializePoints();
    //             if (useStandardKDTree)
    //             {
    //                 InitializeTree_Standard();
    //             }
    //             else
    //             {
    //                 InitializeTree_Array();
    //             }

    //             intervalQueryMinBounds = new Vector3(-radius, -radius, -radius);
    //             intervalQueryMaxBounds = new Vector3(radius, -radius + 1, radius);
    //         }
    //     }
    //     else
    //     {
    //         intervalQueryMinBounds.y += Time.deltaTime;
    //         intervalQueryMaxBounds.y += Time.deltaTime;
    //         if (intervalQueryMinBounds.y > radius)
    //         {
    //             intervalQueryHorizontal = true;

    //             InitializePoints();
    //             if (useStandardKDTree)
    //             {
    //                 InitializeTree_Standard();
    //             }
    //             else
    //             {
    //                 InitializeTree_Array();
    //             }

    //             intervalQueryMinBounds = new Vector3(-radius, -radius, -radius);
    //             intervalQueryMaxBounds = new Vector3(-radius + 1, radius, radius);
    //         }
    //     }
    // }








    // void Awake()
    // {
    //     InitializePoints();
    //     if (useStandardKDTree)
    //     {
    //         InitializeTree_Standard();
    //     }
    //     else
    //     {
    //         InitializeTree_Array();
    //     }

    //     switch (queryType)
    //     {
    //         case QueryType.Radius:
    //             InitializeRadiusQuery();
    //             break;
    //         case QueryType.KNearest:
    //             InitializeKNearestQuery();
    //             break;
    //         case QueryType.Interval:
    //             InitializeIntervalQuery();
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // void Update()
    // {
    //     switch (queryType)
    //     {
    //         case QueryType.Radius:
    //             RadiusQueryUpdate();
    //             break;
    //         // case QueryType.KNearest:
    //         //     KNearestQueryUpdate();
    //         //     break;
    //         case QueryType.Interval:
    //             IntervalQueryUpdate();
    //             break;
    //     }
    // }

    // private void OnDrawGizmos()
    // {
    //     if (points == null || points.Length == 0)
    //     {
    //         return;
    //     }

    //     Vector3 size = Vector3.one * 0.1f;
    //     Gizmos.color = Color.red;
    //     foreach (var point in points)
    //     {
    //         Gizmos.DrawCube(point, size);
    //     }

    //     List<int> resultIndices = new List<int>();
    //     switch (queryType)
    //     {
    //         case QueryType.Radius:
    //             if (useStandardKDTree)
    //             {
    //                 OrderedList<float, int> points = tree_standard.RadialSearch(new double[] { radiusQueryPoint.x, radiusQueryPoint.y, radiusQueryPoint.z }, radiusQueryRadius);
    //                 foreach (var point in points)
    //                 {
    //                     resultIndices.Add(point.Value);
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.Log($"Radius: {radiusQueryRadius}");
    //                 radiusQuery.Query(tree_array, radiusQueryPoint, radiusQueryRadius, resultIndices);
    //             }
    //             break;
    //         case QueryType.KNearest:
    //             KNearestQueryUpdate();
    //             if (useStandardKDTree)
    //             {
    //                 OrderedList<float, int> points = tree_standard.GetNearestNeighbors(new double[] { kNearestQueryPoint.x, kNearestQueryPoint.y, kNearestQueryPoint.z }, kNearestQueryCount);
    //                 foreach (var point in points)
    //                 {
    //                     resultIndices.Add(point.Value);
    //                 }
    //             }
    //             else
    //             {
    //                 kNearestQuery.Query(tree_array, kNearestQueryPoint, kNearestQueryCount, resultIndices);
    //             }

                
    //             break;
    //         case QueryType.Interval:
    //             if (useStandardKDTree)
    //             {
    //                 OrderedList<float, int> points = tree_standard.IntervalQuery(new double[] { intervalQueryMinBounds.x, intervalQueryMinBounds.y, intervalQueryMinBounds.z }, new double[] { intervalQueryMaxBounds.x, intervalQueryMaxBounds.y, intervalQueryMaxBounds.z });
    //                 foreach (var point in points)
    //                 {
    //                     resultIndices.Add(point.Value);
    //                 }
    //             }
    //             else
    //             {
    //                 intervalQuery.Query(tree_array, intervalQueryMinBounds, intervalQueryMaxBounds, resultIndices);
    //             }
    //             break;
    //     }

    //     foreach (int index in resultIndices)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(points[index], 0.2f);
    //     }
    // }
}
