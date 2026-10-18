using UnityEngine;

namespace BizimKosemiz.Player
{
    /// <summary>
    /// Karakterin yürüme, koşma ve bekleme (Idle) durumlarını Animator Controller'a iletir.
    /// Harici 3D karakter animasyonu henüz bağlanmamışsa bile (Placeholder aşamasında)
    /// görsel kök nesneye (VisualRoot) hafif ve doğal bir adım/nefes salınımı uygular.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerAnimatorBridge : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        [SerializeField] private Animator animator;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float bobFrequency = 9f;
        [SerializeField] private float bobAmplitude = 0.035f;

        private float bobTimer;

        public float CurrentNormalizedSpeed { get; private set; }
        public bool CurrentIsRunning { get; private set; }

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (visualRoot == null)
            {
                Transform found = transform.Find("VisualRoot");
                visualRoot = found != null ? found : transform;
            }
        }

        public void UpdateAnimationState(float planarSpeed, float maxRunSpeed, bool isGrounded, bool isRunning)
        {
            CurrentNormalizedSpeed = maxRunSpeed > 0.01f ? Mathf.Clamp01(planarSpeed / maxRunSpeed) : 0f;
            CurrentIsRunning = isRunning;

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetFloat(SpeedHash, planarSpeed, 0.1f, Time.deltaTime);
                animator.SetBool(IsGroundedHash, isGrounded);
                animator.SetBool(IsRunningHash, isRunning);
            }

            // Placeholder modelde yürüme/koşma okunabilirliğini artıran yumuşak adım salınımı (ayak hizasını bozmaz)
            if (visualRoot != null && visualRoot != transform)
            {
                if (planarSpeed > 0.15f && isGrounded)
                {
                    float speedMultiplier = isRunning ? 1.35f : 1.0f;
                    bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;
                    float verticalBob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitude * CurrentNormalizedSpeed;
                    visualRoot.localPosition = new Vector3(0f, verticalBob, 0f);
                }
                else
                {
                    bobTimer = 0f;
                    visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, Vector3.zero, Time.deltaTime * 10f);
                }
            }
        }
    }
}
