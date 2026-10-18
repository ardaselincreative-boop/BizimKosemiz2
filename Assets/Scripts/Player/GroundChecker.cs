using UnityEngine;

namespace BizimKosemiz.Player
{
    /// <summary>
    /// Karakterin ayak hizasında (y = 0 pivot noktası) zeminle temasını kontrol eder.
    /// CharacterController.isGrounded bilgisini küresel fizik taramasıyla (CheckSphere) destekler.
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float checkRadius = 0.24f;
        [SerializeField] private float checkOffsetFromPivot = 0.08f;
        [SerializeField] private LayerMask groundLayerMask = ~0;

        public bool IsGrounded { get; private set; }

        public bool EvaluateGrounded(CharacterController controller)
        {
            if (controller != null && controller.isGrounded)
            {
                IsGrounded = true;
                return true;
            }

            Vector3 probePosition = groundCheckPoint != null
                ? groundCheckPoint.position
                : transform.position + Vector3.up * checkOffsetFromPivot;

            Collider[] hits = Physics.OverlapSphere(probePosition, checkRadius, groundLayerMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform != transform && !hits[i].transform.IsChildOf(transform))
                {
                    IsGrounded = true;
                    return true;
                }
            }

            IsGrounded = false;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Vector3 probePosition = groundCheckPoint != null
                ? groundCheckPoint.position
                : transform.position + Vector3.up * checkOffsetFromPivot;
            Gizmos.DrawWireSphere(probePosition, checkRadius);
        }
    }
}
