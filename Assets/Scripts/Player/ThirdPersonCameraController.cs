using UnityEngine;

namespace BizimKosemiz.Player
{
    /// <summary>
    /// Yalnızca yerel oyuncunun (Local Player) CameraTarget noktasını takip eden,
    /// çevre engelleriyle çarpışmayı önleyen (SphereCast) yumuşak üçüncü şahıs kamera kontrolcüsü.
    /// </summary>
    [DisallowMultipleComponent]
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Takip ve Mesafe")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private float targetDistance = 5.2f;
        [SerializeField] private float minDistance = 1.4f;
        [SerializeField] private float positionSmoothTime = 0.06f;

        [Header("Bakış Açıları")]
        [SerializeField] private float yaw = 0f;
        [SerializeField] private float pitch = 22f;
        [SerializeField] private float minPitch = -10f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float lookSensitivity = 1.2f;

        [Header("Çarpışma Koruması")]
        [SerializeField] private float cameraCollisionRadius = 0.22f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private LocalPlayerInputSource boundInputSource;
        private Vector3 followVelocity;

        public Transform FollowTarget => followTarget;

        public void BindLocalPlayerTarget(Transform target, LocalPlayerInputSource input)
        {
            followTarget = target;
            boundInputSource = input;
            if (followTarget != null)
            {
                yaw = followTarget.eulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            if (boundInputSource != null)
            {
                Vector2 look = boundInputSource.LookInput;
                yaw += look.x * lookSensitivity;
                pitch = Mathf.Clamp(pitch - look.y * lookSensitivity, minPitch, maxPitch);
            }

            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredDirection = orbitRotation * Vector3.back;
            float adjustedDistance = ResolveCameraCollisionDistance(followTarget.position, desiredDirection, targetDistance);

            Vector3 desiredPosition = followTarget.position + desiredDirection * adjustedDistance;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, positionSmoothTime);
            transform.rotation = orbitRotation;
        }

        private float ResolveCameraCollisionDistance(Vector3 pivotPos, Vector3 direction, float maxDist)
        {
            if (Physics.SphereCast(pivotPos, cameraCollisionRadius, direction, out RaycastHit hit, maxDist, collisionMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform != followTarget && !hit.transform.IsChildOf(followTarget.root))
                {
                    return Mathf.Clamp(hit.distance - 0.1f, minDistance, maxDist);
                }
            }
            return maxDist;
        }
    }
}
