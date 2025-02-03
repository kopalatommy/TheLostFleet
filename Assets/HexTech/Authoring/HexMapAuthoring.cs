using GalacticBoundStudios.MeshMania;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;

namespace GalacticBoundStudios.HexTech
{
    public enum HexGridShape
    {
        Rectangle,
        Hexagon,
        Triangle,
        HexagonRing,
    }

    public class HexMapAuthoring : MonoBehaviour
    {
        [SerializeField]
        protected HexMapConfig mapConfig;

        // Prefab used to draw the hexagon coords above the hex map
        [SerializeField]
        private GameObject hexCoordPrefab;

        // Create a default mesh for the authoring component
        // private void Awake()
        // {
        //     Mesh mesh = new Mesh();

        //     mesh.vertices = new Vector3[4];
        //     mesh.triangles = new int[6];

        //     mesh.vertices[0] = new Vector3(0, 0, 0);
        //     mesh.vertices[1] = new Vector3(1, 0, 0);
        //     mesh.vertices[2] = new Vector3(0, 0, 1);

        //     mesh.triangles[0] = 0;
        //     mesh.triangles[1] = 1;
        //     mesh.triangles[2] = 2;

        //     mesh.triangles[3] = 2;
        //     mesh.triangles[4] = 1;
        //     mesh.triangles[5] = 3;

        //     mesh.RecalculateNormals();
        //     mesh.RecalculateBounds();

        //     GetComponent<MeshFilter>().sharedMesh = mesh;
        //     GetComponent<MeshFilter>().mesh = mesh;
        // }

        public class Baker : Baker<HexMapAuthoring>
        {
            public override void Bake(HexMapAuthoring authoring)
            {
                Debug.Log("Baking HexMapAuthoring");

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                HexMapTransformData transformData = new HexMapTransformData
                {
                    orientation = authoring.mapConfig.pointyTopHexagons ? HexOrientation.PointyTop() : HexOrientation.FlatTop(),
                    scale = authoring.mapConfig.mapScale,
                    origin = authoring.mapConfig.mapOffset
                };

                AddComponent(entity, in transformData);

                AddComponent(entity, new HexHollowData
                {
                    isHollow = authoring.mapConfig.hollow,
                    innerRadius = authoring.mapConfig.innerRadius
                });

                HexagonActivationGrid gridData = new HexagonActivationGrid
                {
                    hexGrid = new NativeHashMap<HexCoord, byte>(100, Allocator.Persistent),
                    randomSeed = (uint)System.DateTime.Now.Ticks,
                };

                PopulateHexGrid(authoring.mapConfig.gridShape, ref gridData);

                //DrawHexCoords(in gridData, ref transformData, authoring.hexCoordPrefab, GameObject.Find("HexUI").transform);

                AddComponent(entity, in gridData);

                AddComponent(entity, new CreateMapTag());

                if (authoring.gameObject.GetComponent<MeshFilter>().sharedMesh == null)
                {
                    Mesh mesh = new Mesh();

                    mesh.vertices = new Vector3[4];
                    mesh.triangles = new int[6];

                    mesh.vertices[0] = new Vector3(0, 0, 0);
                    mesh.vertices[1] = new Vector3(1, 0, 0);
                    mesh.vertices[2] = new Vector3(0, 0, 1);

                    mesh.triangles[0] = 0;
                    mesh.triangles[1] = 1;
                    mesh.triangles[2] = 2;

                    mesh.triangles[3] = 2;
                    mesh.triangles[4] = 1;
                    mesh.triangles[5] = 3;

                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    authoring.gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
                    authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;
                }

                AddComponent(entity, new MeshIndexData()
                {
                    index = authoring.gameObject.GetComponent<MeshFilter>().sharedMesh.GetInstanceID()
                });

                Debug.Log("Baked HexMapAuthoring. Number of hexagons: " + gridData.hexGrid.Count);
            }

            private void PopulateHexGrid(HexGridShape gridShape, ref HexagonActivationGrid gridData)
            {
                switch (gridShape)
                {
                    case HexGridShape.Hexagon:
                        PopulateHexagonGrid(ref gridData);
                        break;
                    case HexGridShape.Rectangle:
                        PopulateRectangleGrid(ref gridData);
                        break;
                    case HexGridShape.Triangle:
                        PopulateTriangleGrid(ref gridData);
                        break;
                    case HexGridShape.HexagonRing:
                        PopulateHexagonRingGrid(ref gridData);
                        break;
                }
            }

            private void PopulateHexagonGrid(ref HexagonActivationGrid gridData)
            {
                for (int q = -5; q <= 5; q++)
                {
                    for (int r = -5; r <= 5; r++)
                    {
                        if (q + r >= -5 && q + r <= 5)
                        {
                            gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                        }
                    }
                }
            }

            private void PopulateRectangleGrid(ref HexagonActivationGrid gridData)
            {
                for (int q = -5; q <= 5; q++)
                {
                    for (int r = -5; r <= 5; r++)
                    {
                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                    }
                }
            }

            private void PopulateTriangleGrid(ref HexagonActivationGrid gridData)
            {
                for (int q = 0; q <= 5; q++)
                {
                    for (int r = 0; r <= 5 - q; r++)
                    {
                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                    }
                }
            }

            private void PopulateHexagonRingGrid(ref HexagonActivationGrid gridData)
            {
                for (int q = -5; q <= 5; q++)
                {
                    for (int r = -5; r <= 5; r++)
                    {
                        if (math.abs(q + r) == 5)
                        {
                            gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                        }
                    }
                }
            }

            private void DrawHexCoords(in HexagonActivationGrid gridData, ref HexMapTransformData orientation, GameObject hexCoordPrefab, Transform parent)
            {
                NativeArray<HexCoord> keys = gridData.hexGrid.GetKeyArray(Allocator.Temp);
                
                foreach (HexCoord key in keys)
                {
                    float2 worldPos = HexMath.HexToPixel(key, in orientation);

                    GameObject locMarker = GameObject.Instantiate(hexCoordPrefab, new Vector3(worldPos.x, 0.1f, worldPos.y), Quaternion.identity, parent);

                    // Get the text mesh pro component from the prefab to write the hex coords
                    TMP_Text text = locMarker.GetComponentInChildren<TMP_Text>();

                    text.SetText(key.q + ", " + key.r + ", " + (-key.q - key.r) + "\n(q,r,s)");
                }
            }
        }
    }
}