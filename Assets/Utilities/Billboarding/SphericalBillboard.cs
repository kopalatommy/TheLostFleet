using UnityEngine;

namespace GalacticBoundStudios.Utilities.Billboards
{
    // Always align to face the camera, on every axis
    public class SphericalBillboard : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        void LateUpdate()
        {
            if (_cam == null) return;

            Vector3 dir = _cam.transform.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = lookRot;
        }
    }
}