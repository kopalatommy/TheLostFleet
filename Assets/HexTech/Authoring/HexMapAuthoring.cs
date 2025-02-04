using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.RTSCamera;
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
        // Leave empty if you want to use the default hex map config
        [SerializeField]
        protected HexMapConfig mapConfigOverride;

        // Prefab used to draw the hexagon coords above the hex map
        [SerializeField]
        private GameObject hexCoordPrefab;

        public class Baker : Baker<HexMapAuthoring>
        {
            public override void Bake(HexMapAuthoring authoring)
            {
                HexMapConfig mapConfig = authoring.mapConfigOverride != null ? authoring.mapConfigOverride : HexMapManager.Instance.Config;

                Debug.Log("Baking HexMapAuthoring");

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                HexMapTransformData transformData = mapConfig.TransformData;

                AddComponent(entity, in transformData);

                AddComponent(entity, new HexHollowData
                {
                    isHollow = mapConfig.hollow,
                    innerRadius = mapConfig.innerRadius
                });

                HexagonActivationGrid gridData = new HexagonActivationGrid
                {
                    hexGrid = new NativeHashMap<HexCoord, byte>(100, Allocator.Persistent),
                    randomSeed = (uint)System.DateTime.Now.Ticks,
                };

                HexCoord minBounds = new HexCoord { q = int.MaxValue, r = int.MaxValue };
                HexCoord maxBounds = new HexCoord { q = int.MinValue, r = int.MinValue };

                PopulateHexGrid(mapConfig.gridShape, ref gridData, mapConfig.chunkSize, ref minBounds, ref maxBounds);

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

            private void PopulateHexGrid(HexGridShape gridShape, ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
            {
                switch (gridShape)
                {
                    case HexGridShape.Hexagon:
                        PopulateHexagonGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                        break;
                    case HexGridShape.Rectangle:
                        PopulateRectangleGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                        break;
                    case HexGridShape.Triangle:
                        PopulateTriangleGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                        break;
                    case HexGridShape.HexagonRing:
                        PopulateHexagonRingGrid(ref gridData, chunkSize, ref minBounds, ref maxBounds);
                        break;
                }
            }

            private void PopulateHexagonGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
            {
                for (int q = -chunkSize; q <= chunkSize; q++)
                {
                    for (int r = -chunkSize; r <= chunkSize; r++)
                    {
                        if (q + r >= -chunkSize && q + r <= chunkSize)
                        {
                            if (minBounds.q > q)
                            {
                                minBounds.q = q;
                            }
                            if (maxBounds.q < q)
                            {
                                maxBounds.q = q;
                            }

                            if (minBounds.r > r)
                            {
                                minBounds.r = r;
                            }
                            if (maxBounds.r < r)
                            {
                                maxBounds.r = r;
                            }

                            gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                            HexMapManager.Instance.onCreateHexagon?.Invoke(new HexCoord { q = q, r = r });
                        }
                    }
                }
            }

            private void PopulateRectangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
            {
                for (int q = -chunkSize; q <= chunkSize; q++)
                {
                    for (int r = -chunkSize; r <= chunkSize; r++)
                    {
                        if (minBounds.q > q)
                        {
                            minBounds.q = q;
                        }
                        if (maxBounds.q < q)
                        {
                            maxBounds.q = q;
                        }

                        if (minBounds.r > r)
                        {
                            minBounds.r = r;
                        }
                        if (maxBounds.r < r)
                        {
                            maxBounds.r = r;
                        }

                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                        HexMapManager.Instance.onCreateHexagon?.Invoke(new HexCoord { q = q, r = r });
                    }
                }
            }

            private void PopulateTriangleGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
            {
                for (int q = 0; q <= chunkSize; q++)
                {
                    for (int r = 0; r <= chunkSize - q; r++)
                    {
                        if (minBounds.q > q)
                        {
                            minBounds.q = q;
                        }
                        if (maxBounds.q < q)
                        {
                            maxBounds.q = q;
                        }

                        if (minBounds.r > r)
                        {
                            minBounds.r = r;
                        }
                        if (maxBounds.r < r)
                        {
                            maxBounds.r = r;
                        }

                        gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                        HexMapManager.Instance.onCreateHexagon?.Invoke(new HexCoord { q = q, r = r });
                    }
                }
            }

            private void PopulateHexagonRingGrid(ref HexagonActivationGrid gridData, int chunkSize, ref HexCoord minBounds, ref HexCoord maxBounds)
            {
                for (int q = -chunkSize; q <= chunkSize; q++)
                {
                    for (int r = -chunkSize; r <= chunkSize; r++)
                    {
                        if (math.abs(q + r) == chunkSize)
                        {
                            if (minBounds.q > q)
                            {
                                minBounds.q = q;
                            }
                            if (maxBounds.q < q)
                            {
                                maxBounds.q = q;
                            }

                            if (minBounds.r > r)
                            {
                                minBounds.r = r;
                            }
                            if (maxBounds.r < r)
                            {
                                maxBounds.r = r;
                            }

                            gridData.hexGrid.Add(new HexCoord { q = q, r = r }, 1);
                            HexMapManager.Instance.onCreateHexagon?.Invoke(new HexCoord { q = q, r = r });
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