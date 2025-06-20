using Unity.Entities;
using Unity.Collections;
using GalacticBoundStudios.HexTech;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.Rendering;
using GalacticBoundStudios.MeshMania;
using GalacticBoundStudios.HexTech.MapGeneration;

namespace GalacticBoundStudios.EchoesOfTheFarRim.GalaxyMap
{
    public static class GalaxyMapCreateUtils
    {
        #region Default Maps

        public static NativeHashMap<HexCoord, byte> DefaultHexMap(int segmentLength)
        {
            NativeHashMap<HexCoord, byte> result = new NativeHashMap<HexCoord, byte>((int)(segmentLength * segmentLength * 0.75), Allocator.Persistent);

            for (int q = -segmentLength; q <= segmentLength; q++)
            {
                for (int r = -segmentLength; r <= segmentLength; r++)
                {
                    if (q + r >= -segmentLength && q + r <= segmentLength)
                    {
                        result.Add(new HexCoord { q = q, r = r }, 0);
                    }
                }
            }

            return result;
        }

        public static NativeHashMap<HexCoord, byte> DefaultSquareMap(int segmentLength)
        {
            NativeHashMap<HexCoord, byte> result = new NativeHashMap<HexCoord, byte>(segmentLength * segmentLength, Allocator.Persistent);

            for (int q = 0; q <= segmentLength; q++)
            {
                for (int r = 0; r <= segmentLength; r++)
                {
                    result.Add(new HexCoord { q = q, r = r }, 0);
                }
            }

            return result;
        }

        public static NativeHashMap<HexCoord, byte> DefaultTriangleMap(int segmentLength)
        {
            NativeHashMap<HexCoord, byte> result = new NativeHashMap<HexCoord, byte>(segmentLength * segmentLength / 2, Allocator.Persistent);

            for (int q = 0; q <= segmentLength; q++)
            {
                for (int r = 0; r <= segmentLength - q; r++)
                {
                    result.Add(new HexCoord { q = q, r = r }, 0);
                }
            }

            return result;
        }

        public static NativeHashMap<HexCoord, byte> DefaultRingMap(int segmentLength)
        {
            NativeHashMap<HexCoord, byte> result = new NativeHashMap<HexCoord, byte>(segmentLength * 4, Allocator.Persistent);

            for (int q = -segmentLength; q <= segmentLength; q++)
            {
                for (int r = -segmentLength; r <= segmentLength; r++)
                {
                    if (math.abs(q + r) == segmentLength)
                    {
                        result.Add(new HexCoord { q = q, r = r }, 0);
                    }
                }
            }

            return result;
        }

        #endregion // Default Maps


        #region Prefab

        public static RenderMeshArray DefaultRenderMeshArray()
        {
            Mesh mesh = new Mesh();

            NativeArray<float3> vertices = new NativeArray<float3>(HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Temp);
            NativeArray<int> tris = new NativeArray<int>(HexTechMapGenerationUtils.HEXAGON_HOLLOW_TRIS, Allocator.Temp);

            HexMapTransformData transformData = HexMapTransformData.Default;
            HexTechMapGenerationUtils.GenerateHexagonVertices_Hollow(0, 0f, 0f, 0.85f, in transformData, ref vertices);
            HexTechMapGenerationUtils.GenerateHexagonTriangles_Hollow(0, ref tris);

            Vector3[] _verts = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _verts[i] = vertices[i];
            }
            int[] _tris = new int[tris.Length];
            for (int i = 0; i < tris.Length; i++)
            {
                _tris[i] = tris[i];
            }

            mesh.vertices = _verts;
            mesh.triangles = _tris;

            Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

            return new RenderMeshArray(new Material[] {material}, new Mesh[]{mesh});
        }

        public static EntityArchetype CreateCellPrefab(ref EntityManager entityManager)
        {
            return entityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(RenderMeshArray),
                typeof(MeshIndexData),
                typeof(HexCoord)
            );

            // Entity prototype = entityManager.CreateEntity();
            // entityManager.SetName(prototype, "GalaxyMapTilePrototype");

            // Mesh mesh = new Mesh();
            // mesh.vertices = new[] { Vector3.up * 1000, Vector3.left * 1000, Vector3.right * 1000, -Vector3.up * 1000 };
            // mesh.triangles = new[] { 0, 1, 2, 1, 2, 3 };

            // entityManager.AddComponentData(prototype, new LocalTransform
            // {
            //     Position = new float3(0, 0, 0),
            //     Rotation = quaternion.identity,
            //     Scale = 1
            // });
            // entityManager.AddComponentData(prototype, new LocalToWorld
            // {
            //     Value = float4x4.Translate(new float3(0, 0, 0))
            // });

            // RenderMeshDescription desc = new RenderMeshDescription(
            //     shadowCastingMode: ShadowCastingMode.Off,
            //     receiveShadows: false);

            // Material material = new Material(Shader.Find("HexTech/HexGridHighlightURPv2"));
            // RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { material }, new Mesh[] { mesh });

            // RenderMeshUtility.AddComponents(
            //     prototype,
            //     entityManager,
            //     desc,
            //     renderMeshArray,
            //     MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            // entityManager.AddComponentData(prototype, new MeshIndexData
            // {
            //     index = mesh.GetInstanceID()
            // });

            // return prototype;
        }

        public static MeshData CreatePrefabMesh_Hollow()
        {
            MeshData data = new MeshData
            {
                vertices = new NativeArray<float3>(HexTechMapGenerationUtils.HEXAGON_HOLLOW_VERTS, Allocator.Persistent),
                triangles = new NativeArray<int>(HexTechMapGenerationUtils.HEXAGON_HOLLOW_TRIS, Allocator.Persistent),
                normals = default,
                colors = default,
                addUV2 = false,
                uv2 = default
            };

            HexMapTransformData hexMapTransformData = HexMapTransformData.Default;
            HexTechMapGenerationUtils.GenerateHexagonVertices_Hollow(0, 0f, 0f, 0.85f, in hexMapTransformData, ref data.vertices);
            HexTechMapGenerationUtils.GenerateHexagonTriangles_Hollow(0, ref data.triangles);

            return data;
        }

        public static MeshData CreatePrefabMesh_Solid()
        {
            MeshData data = new MeshData
            {
                vertices = new NativeArray<float3>(HexTechMapGenerationUtils.HEXAGON_SOLID_VERTS, Allocator.Persistent),
                triangles = new NativeArray<int>(HexTechMapGenerationUtils.HEXAGON_SOLID_TRIS, Allocator.Persistent),
                normals = default,
                colors = default,
                addUV2 = false,
                uv2 = default
            };

            HexMapTransformData hexMapTransformData = HexMapTransformData.Default;
            HexTechMapGenerationUtils.GenerateHexagonVertices_Solid(0, 0f, 0f, in hexMapTransformData, ref data.vertices);
            HexTechMapGenerationUtils.GenerateHexagonTriangles_Solid(0, ref data.triangles);

            return data;
        }
        
        #endregion
    }
}
