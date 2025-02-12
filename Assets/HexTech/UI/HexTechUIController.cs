using UnityEngine;
using TMPro;
using GalacticBoundStudios.HexTech;
using Unity.Mathematics;
using System;

public class HexTechUIController : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text currentCoordsText;
    [SerializeField]
    protected TMP_Text selectedCoordsText;

    [SerializeField]
    private Transform mouseMarkerTransform;

    [SerializeField]
    protected GameObject hexCoordsPrefab;
    [SerializeField]
    protected GameObject uiCanvas;

    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        InitializeEventListeners();
        RemoveLingeringCoords();
    }

    protected void InitializeEventListeners()
    {
        HexMapManager.Instance.onCreateHexagon += OnNewHexagon;
        HexMapManager.Instance.onSelectHexagon += onSelectHexagonAction;
    }

    void RemoveLingeringCoords()
    {
        foreach (Transform child in uiCanvas.transform)
        {
            Destroy(child.gameObject);
        }
    }

    protected void Update()
    {
        Ray ray;
        Camera camera = Camera.main;
        if (Camera.main.orthographic)
        {
            ray = ScreenPointToRay_Orthographic(Input.mousePosition, camera.aspect, mainCamera.transform.position, mainCamera.transform.rotation, camera.orthographicSize, mainCamera.transform.forward);
        }
        else
        {
            ray = ScreenPointToRay_Standard(Input.mousePosition, camera.fieldOfView, camera.aspect, mainCamera.transform.position, mainCamera.transform.rotation);
        }

        Vector3 intersection = DetermineWhereRayIntersectsPlane(ray);

        HexCoord selectedCoord = HexMath.PixelToHex(new Unity.Mathematics.float2(intersection.x, intersection.z), HexMapManager.Instance.Config.TransformData);

        mouseMarkerTransform.position = intersection;

        currentCoordsText.text = intersection.ToString() + " - " + selectedCoord;
    }

    public Vector3 DetermineWhereRayIntersectsPlane(Ray ray)
    {
        float intersection = CalculateIntersection(ray.origin, ray.direction, Vector3.zero, Vector3.up);

        if (float.IsNaN(intersection))
        {
            return Vector3.zero;
        }
        return ray.origin + (ray.direction * intersection);
    }

    public float CalculateIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePoint, Vector3 planeNormal)
    {
        float denominator = Vector3.Dot(planeNormal, rayDirection);
        // Make sure the ray is not parallel to the plane
        if (Mathf.Abs(denominator) < 1e-6)
        {
            return float.NaN;
        }

        Vector3 difference = planePoint - rayOrigin;
        float t = Vector3.Dot(difference, planeNormal) / denominator;
        return t;
    }

    public Ray ScreenPointToRay_Orthographic(Vector3 screenPos, float aspect, Vector3 position, Quaternion rotation, float orthographicSize, Vector3 forward)
    {
        // Remap so (0, 0) is the center of the window,
        // and the edges are at -0.5 and +0.5.
        Vector2 relative = new Vector2(
            screenPos.x / Screen.width - 0.5f,
            screenPos.y / Screen.height - 0.5f
        );

        // Scale using half-height of camera.
        Vector3 worldUnits = relative * orthographicSize * 2f;
        worldUnits.x *= aspect;

        // Orient and position to match camera transform.
        Vector3 origin = rotation * worldUnits;
        origin += position;

        // Output a ray from this point, along camera's axis.
        return new Ray(origin, forward);
    }

    public Ray ScreenPointToRay_Standard(Vector3 screenPos, float fieldOfView, float aspect, Vector3 position, Quaternion rotation)
    {
        // Remap so (0, 0) is the center of the window,
        // and the edges are at -0.5 and +0.5.
        Vector2 relative = new Vector2(
            screenPos.x / Screen.width - 0.5f,
            screenPos.y / Screen.height - 0.5f
        );

        // Angle in radians from the view axis
        // to the top plane of the view pyramid.
        float verticalAngle = 0.5f * Mathf.Deg2Rad * fieldOfView;

        // World space height of the view pyramid
        // measured at 1 m depth from the camera.
        float worldHeight = 2f * Mathf.Tan(verticalAngle);

        // Convert relative position to world units.
        Vector3 worldUnits = relative * worldHeight;
        worldUnits.x *= aspect;
        worldUnits.z = 1;

        // Rotate to match camera orientation.
        Vector3 direction = rotation * worldUnits;

        // Output a ray from camera position, along this direction.
        return new Ray(position, direction);
    }

    public void OnNewHexagon(HexCoord coord)
    {
        float2 pixelCoords = HexMath.HexToPixel(coord, HexMapManager.Instance.Config.TransformData);

        GameObject hexCoords = Instantiate(hexCoordsPrefab, new Vector3(pixelCoords.x, 0, pixelCoords.y), Quaternion.identity, uiCanvas.transform);
        hexCoords.GetComponentInChildren<TMP_Text>().text = coord.ToString();
    }

    public void onSelectHexagonAction(HexCoord coord)
    {
        selectedCoordsText.text = coord.ToString();
    }
}
