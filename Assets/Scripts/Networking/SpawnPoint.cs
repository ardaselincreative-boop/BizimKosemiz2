using UnityEngine;

namespace BizimKosemiz.Networking
{
    /// <summary>
    /// Bahçe içinde oyuncuların zemin üzerinde güvenle doğacağı noktayı temsil eder.
    /// Zeminin altına gömülmeyi önlemek için Physics.Raycast ile zemin yüksekliğini doğrular.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private int spawnIndex;
        [SerializeField] private float groundClearanceOffset = 0.02f;

        public int SpawnIndex => spawnIndex;

        public Vector3 GetGroundedSpawnPosition()
        {
            Vector3 origin = transform.position + Vector3.up * 2.5f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
            {
                return hit.point + Vector3.up * groundClearanceOffset;
            }

            return new Vector3(transform.position.x, Mathf.Max(0f, transform.position.y), transform.position.z);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.85f, 0.35f, 0.85f);
            Vector3 pos = transform.position;
            Gizmos.DrawWireSphere(pos + Vector3.up * 0.9f, 0.35f);
            Gizmos.DrawLine(pos, pos + transform.forward * 1.0f);
        }
    }
}
