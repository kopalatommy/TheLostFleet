using UnityEngine;
using TMPro;
using GalacticBoundStudios.HexTech;
using Unity.Mathematics;
using Unity.Entities;
using GalacticBoundStudios.RTSCamera;
using Unity.Collections;
using Unity.Transforms;

namespace GalacticBoundStudios.TheLostFleet
{
    public class HexTechTestUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField]
        protected TMP_Text currentCoordsText;
        [SerializeField]
        protected TMP_Text selectedCoordsText;

        [Header("Object References")]
        [SerializeField]
        private Transform mouseMarkerTransform;
        [SerializeField]
        protected GameObject hexCoordCanvas;

        [Header("Prefabs")]
        [SerializeField]
        protected GameObject hexCoordsPrefab;

        private EntityManager entityManager;
        private EntityQuery cameraQuery;

        void Awake()
        {
            RemoveLingeringCoords();
            
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            cameraQuery = entityManager.CreateEntityQuery(typeof(RTSCameraTag));
        
            InitializeEventListeners();
        }

        void Update()
        {
            NativeArray<Entity> cameraEntities = cameraQuery.ToEntityArray(Allocator.TempJob);

            foreach (var entity in cameraEntities)
            {
                LocalTransform cameraTransform = entityManager.GetComponentData<LocalTransform>(entity);
                RTSCameraSettings cameraSettings = entityManager.GetComponentData<RTSCameraSettings>(entity);
                
                Ray cameraRay;
                if (cameraSettings.orthographic)
                {
                    cameraRay = CameraUtilities.ScreenPointToRay_Orthographic(Input.mousePosition, cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation, cameraSettings.orthographicSize, cameraTransform.Forward());
                }
                else
                {
                    cameraRay = CameraUtilities.ScreenPointToRay_Standard(Input.mousePosition, cameraSettings.fieldOfView, cameraSettings.aspect, cameraTransform.Position, cameraTransform.Rotation);
                }

                Vector3 intersection = CameraUtilities.DetermineWhereRayIntersectsPlain(cameraRay, float3.zero, new float3(0, 1, 0));
                HexCoord hexCoord = HexMath.PixelToHex(new float2(intersection.x, intersection.z), HexMapManager.Instance.Config.TransformData);

                mouseMarkerTransform.position = intersection;
                currentCoordsText.text = hexCoord.ToString();
            }

            cameraEntities.Dispose();
        }
        
        private void InitializeEventListeners()
        {
            HexMapManager.Instance.onCreateHexagon += OnNewHexagon;
            HexMapManager.Instance.onSelectHexagon += onSelectHexagonAction;
        }

        void RemoveLingeringCoords()
        {
            foreach (Transform child in hexCoordCanvas.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void OnNewHexagon(HexCoord coord)
        {
            float2 pixelCoords = HexMath.HexToPixel(coord, HexMapManager.Instance.Config.TransformData);

            GameObject hexCoords = Instantiate(hexCoordsPrefab, new Vector3(pixelCoords.x, 0, pixelCoords.y), Quaternion.identity, hexCoordCanvas.transform);
            hexCoords.GetComponentInChildren<TMP_Text>().text = coord.ToString();
        }

        public void onSelectHexagonAction(HexCoord coord)
        {
            selectedCoordsText.text = coord.ToString();
        }
    }
}