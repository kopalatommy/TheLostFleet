using UnityEngine;
using TMPro;
using GalacticBoundStudios.HexTech;
using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.RTSCamera;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.UI;
using GalacticBoundStudios.HexTech.MapGeneration;
using GalacticBoundStudios.HexTech.PathFinding;
using GalacticBoundStudios.SpawnPrefabsSystem;
using GalacticBoundStudios.EchoesOfTheFarRim.SystemMap;

namespace GalacticBoundStudios.TheLostFleet
{
    public class HexTechTestUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField]
        protected TMP_Text currentCoordsText;
        [SerializeField]
        protected TMP_Text selectedCoordsText;
        [SerializeField]
        protected TMP_Text startCoordsText;
        [SerializeField]
        protected TMP_Text endCoordsText;
        [SerializeField]
        protected Button startPathfinderButton;

        [Header("Object References")]
        [SerializeField]
        private Transform mouseMarkerTransform;
        [SerializeField]
        protected GameObject hexCoordCanvas;

        [Header("Prefabs")]
        [SerializeField]
        protected GameObject hexCoordsPrefab;
        [SerializeField]
        protected GameObject mapEntityPrefab;
        Entity prefabEntity;

        private EntityManager entityManager;
        private EntityQuery cameraQuery;
        private EntityQuery focusHexagonQuery;
        private EntityQuery selectedHexagonQuery;

        // For path finding
        protected HexCoord currentPos;
        protected HexCoord pathStartPos;
        protected HexCoord pathEndPos;

        void Awake()
        {
            RemoveLingeringCoords();
            
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            cameraQuery = entityManager.CreateEntityQuery(typeof(RTSCameraTag));
            focusHexagonQuery = entityManager.CreateEntityQuery(typeof(SystemMapFocusHexagonData));
            selectedHexagonQuery = entityManager.CreateEntityQuery(typeof(SystemMapSelectedHexagonData));
        
            startPathfinderButton.onClick.AddListener(onStartPathfinderClicked);

            InitializeEventListeners();
        }

        private void Start()
        {
            //prefabEntity = entityManager.Instantiate(mapEntityPrefab);
        }

        private void OnDisable()
        {
            RemoveLingeringCoords();
        }

        private void OnDestroy()
        {
            RemoveLingeringCoords();
        }

        void Update()
        {
            if (focusHexagonQuery.TryGetSingleton(out SystemMapFocusHexagonData focusHexagon))
            {
                currentCoordsText.text = focusHexagon.coord.ToString();
            }
            if (selectedHexagonQuery.TryGetSingleton(out SystemMapSelectedHexagonData selectedHexagonData))
            {
                selectedCoordsText.text = "Selected(2): " + selectedHexagonData.coord.ToString();
            }
            
            // NativeArray<Entity> cameraEntities = cameraQuery.ToEntityArray(Allocator.TempJob);

            // foreach (var entity in cameraEntities)
            // {
            //     LocalTransform cameraTransform = entityManager.GetComponentData<LocalTransform>(entity);
            //     RTSCameraSettings cameraSettings = entityManager.GetComponentData<RTSCameraSettings>(entity);
                
            //     Ray cameraRay;
            //     if (cameraSettings.orthographic)
            //     {
            //         cameraRay = CameraUtilities.ScreenPointToRay_Orthographic(Input.mousePosition, cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation, cameraSettings.orthographicSize, cameraTransform.Forward());
            //     }
            //     else
            //     {
            //         cameraRay = CameraUtilities.ScreenPointToRay_Standard(Input.mousePosition, cameraSettings.fieldOfView, cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation);
            //     }

            //     Vector3 intersection = CameraUtilities.DetermineWhereRayIntersectsPlain(cameraRay, float3.zero, new float3(0, 1, 0));
            //     HexCoord hexCoord = HexMath.PixelToHex(new float2(intersection.x, intersection.z), HexMapManager.Instance.Config.TransformData);

            //     mouseMarkerTransform.position = intersection;
            //     currentCoordsText.text = hexCoord.ToString();
            // }

            // cameraEntities.Dispose();
        }
        
        private void InitializeEventListeners()
        {
            // HexMapManager.Instance.onCreateHexagon += OnNewHexagon;
            // HexMapManager.Instance.onSelectHexagon += onSelectHexagonAction;

            // HexMapManager.Instance.setPathStartPos += SetPathStartPos;
            // HexMapManager.Instance.setPathEndPos += SetPathEndPos;
            // HexMapManager.Instance.startPathFinder += onStartPathfinderClicked;
        }

        void RemoveLingeringCoords()
        {
            Debug.Log("RemoveLingeringCoords: " + hexCoordCanvas.transform.childCount);
            foreach (Transform child in hexCoordCanvas.transform)
            {
                Debug.Log("Destroying " + child.gameObject.name);
                Destroy(child.gameObject);
            }
        }

        public void OnNewHexagon(HexCoord coord)
        {
            // float2 pixelCoords = HexMath.HexToPixel(coord, HexMapManager.Instance.Config.TransformData);

            // GameObject hexCoords = Instantiate(hexCoordsPrefab, new Vector3(pixelCoords.x, 0, pixelCoords.y), Quaternion.identity, hexCoordCanvas.transform);
            // hexCoords.GetComponentInChildren<TMP_Text>().text = coord.ToString();
        }

        public void onSelectHexagonAction(HexCoord coord)
        {
            currentPos = coord;
            selectedCoordsText.text = coord.ToString();
        }

        public void onStartPathfinderClicked()
        {
            Debug.Log("Creating pathfinder request");

            Debug.Log("Create path from " + pathStartPos + " to " + pathEndPos);

            //HexMapTransformData mapTransformData = HexMapManager.Instance.Config.TransformData;
            //float2 spawnLoc = HexMath.HexToPixel(pathStartPos, in mapTransformData);
            //Instantiate(mapEntityPrefab, new Vector3(spawnLoc.x, 0, spawnLoc.y), Quaternion.identity, GameObject.Find("EntityInjection").transform);

            //Entity entity = entityManager.Instantiate(mapEntityPrefab);

            // Entity spawnRequestEntity = entityManager.CreateEntity(typeof(HexTechCreatePathRequest));
            // entityManager.SetComponentData(spawnRequestEntity, new HexTechCreatePathRequest()
            // {
            //     startPos = pathStartPos,
            //     endPos = pathEndPos,
            // });

           
           
           
           
           
            // HexMapTransformData transformData = HexMapManager.Instance.Config.TransformData;
            // float2 mapPos = HexMath.HexToPixel(pathStartPos, in transformData);

            // Entity spawnUnitRequest = entityManager.CreateEntity(typeof(SpawnPrefabRequest));
            // entityManager.SetComponentData(spawnUnitRequest, new SpawnPrefabRequest
            // {
            //     keyHash = "MapEntity".GetHashCode(),
            //     position = new float3(mapPos.x, 0, mapPos.y),
            //     rotation = quaternion.identity,
            // });
        }

        public void SetPathStartPos(HexCoord startPos)
        {
            pathStartPos = startPos;
            startCoordsText.text = "Start: " + startPos;
        }
        public void SetPathEndPos(HexCoord endPos)
        {
            pathEndPos = endPos;
            endCoordsText.text = "End: " + endPos;
        }
    }
}