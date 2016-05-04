using UnityEngine;

namespace AndysUnityFramework
{
    public class MouseRotateController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        Transform _groundingNode;
        [SerializeField]
        Transform _mouseLookNode;

        [Header("Configuration")]
        [SerializeField]
        bool _clampRotation = true;
        [SerializeField]
        float _minXRotation = -90.0f;
        [SerializeField]
        float _maxXRotatrion = 90.0f;
        [SerializeField]
        bool _smoothRotation = true;
        [SerializeField]
        float _smoothTime = 0.1f;

        [Header("Controls")]
        [SerializeField]
        int _mouseRotateButtton = 2;
        [SerializeField]
        float _mouseSensitivity = 2.0f;

        Vector3 _groundNodeVelo;
        Vector3 _mouseLookNodeVelo;

        Quaternion _groundNodeRot;
        Quaternion _mouseLookNodeRot;

        void Start()
        {
            _groundNodeRot      = _groundingNode.localRotation;
            _mouseLookNodeRot   = _mouseLookNode.localRotation;
        }

        void Update()
        {
            if (Input.GetMouseButton(_mouseRotateButtton))
            {
                float yRotation = Input.GetAxisRaw("Mouse X") * _mouseSensitivity;
                float xRotation = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;

                _groundNodeRot *= Quaternion.Euler(0, yRotation, 0);
                _mouseLookNodeRot *= Quaternion.Euler(-xRotation, 0, 0);

                if (_clampRotation)
                {
                    _mouseLookNodeRot = ClampRotationAroundXAxis(_mouseLookNodeRot);
                }
            }

            if (_smoothRotation)
            {
                _mouseLookNode.localRotation = SmoothDampRotation(_mouseLookNode.localRotation, _mouseLookNodeRot, ref _mouseLookNodeVelo, _smoothTime);
                _groundingNode.localRotation = SmoothDampRotation(_groundingNode.localRotation, _groundNodeRot, ref _groundNodeVelo, _smoothTime);
            }
            else
            {
                _mouseLookNode.localRotation = _mouseLookNodeRot;
                _groundingNode.localRotation = _groundNodeRot;
            }

            
        }

        Quaternion SmoothDampRotation(Quaternion a_current, Quaternion a_target, ref Vector3 a_currentVelocity, float a_smoothTime)
        {
            Vector3 currentEuler    = a_current.eulerAngles;
            Vector3 targetEuler     = a_target.eulerAngles;

            return Quaternion.Euler(
                Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref a_currentVelocity.x, a_smoothTime),
                Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref a_currentVelocity.y, a_smoothTime),
                Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref a_currentVelocity.z, a_smoothTime));
        }

        Quaternion ClampRotationAroundXAxis(Quaternion a_quaternion)
        {
            a_quaternion.x /= a_quaternion.w;
            a_quaternion.y /= a_quaternion.w;
            a_quaternion.z /= a_quaternion.w;
            a_quaternion.w = 1.0f;

            float angleX    = 2.0f * Mathf.Rad2Deg * Mathf.Atan(a_quaternion.x);
            angleX          = Mathf.Clamp(angleX, _minXRotation, _maxXRotatrion);
            a_quaternion.x  = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return a_quaternion;
        }
    }
}
