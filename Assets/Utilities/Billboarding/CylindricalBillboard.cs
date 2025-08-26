using UnityEngine;

namespace GalacticBoundStudios.Utilities.Billboards
{
    public class CylindricalBillboard : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (_cam == null) return;

            Vector3 camPos = _cam.transform.position;

            // Lock camera’s Y to our Y so we only rotate around Y
            camPos.y = transform.position.y;

            Vector3 direction = transform.position - _cam.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            // Make the object look at that adjusted position
            // transform.LookAt(camPos);

            // Optionally, zero out any X/Z tilt (just in case)
            // Vector3 e = transform.rotation.eulerAngles;
            // transform.rotation = Quaternion.Euler(0f, e.y, 0f);
        }
    }
}