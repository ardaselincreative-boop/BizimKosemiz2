using BizimKosemiz.Networking;
using UnityEngine;

namespace BizimKosemiz.Player
{
    /// <summary>
    /// Üçüncü şahıs kamera açısına göre karakterin yürümesini, koşmasını, dönüşünü
    /// ve yerçekimi / zemin hizalamasını yöneten ana kontrolcü.
    /// Pivot noktası tam ayak tabanında (y = 0) olacak şekilde CharacterController'ı yapılandırır.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(GroundChecker))]
    [RequireComponent(typeof(LocalPlayerInputSource))]
    [RequireComponent(typeof(PlayerAnimatorBridge))]
    [RequireComponent(typeof(PlayerNetworkAdapter))]
    [DisallowMultipleComponent]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Hareket Hızları")]
        [SerializeField] private float walkSpeed = 3.6f;
        [SerializeField] private float runSpeed = 6.2f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 12f;

        [Header("Yerçekimi ve Zemin")]
        [SerializeField] private float gravity = -19.62f;
        [SerializeField] private float groundedStickVelocity = -2.0f;

        [Header("Referanslar")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraFollowTarget;

        private CharacterController characterController;
        private GroundChecker groundChecker;
        private LocalPlayerInputSource inputSource;
        private PlayerAnimatorBridge animatorBridge;
        private PlayerNetworkAdapter networkAdapter;

        private float currentPlanarSpeed;
        private float rotationVelocity;
        private float verticalVelocity;

        public Transform CameraFollowTarget => cameraFollowTarget;
        public float CurrentPlanarSpeed => currentPlanarSpeed;
        public bool IsGrounded => groundChecker != null && groundChecker.IsGrounded;
        public bool IsLocalPlayer => networkAdapter == null || networkAdapter.IsLocalPlayerControlled;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            groundChecker = GetComponent<GroundChecker>();
            inputSource = GetComponent<LocalPlayerInputSource>();
            animatorBridge = GetComponent<PlayerAnimatorBridge>();
            networkAdapter = GetComponent<PlayerNetworkAdapter>();

            EnsureCharacterControllerDimensions();
            EnsureCameraTargetReference();
        }

        private void Start()
        {
            if (IsLocalPlayer && cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                ThirdPersonCameraController camCtrl = Camera.main.GetComponent<ThirdPersonCameraController>();
                if (camCtrl != null)
                {
                    camCtrl.BindLocalPlayerTarget(cameraFollowTarget, inputSource);
                }
            }
        }

        /// <summary>
        /// Karakterin zeminin altına girmesini veya havada asılı kalmasını önlemek için
        /// CharacterController alt noktasını tam olarak yerel y = 0.0f (ayak tabanı) hizasına sabitler.
        /// </summary>
        public void EnsureCharacterControllerDimensions()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            characterController.center = new Vector3(0f, 0.9f, 0f);
            characterController.skinWidth = 0.04f;
            characterController.stepOffset = 0.3f;
            characterController.slopeLimit = 45f;
        }

        private void EnsureCameraTargetReference()
        {
            if (cameraFollowTarget == null)
            {
                Transform found = transform.Find("CameraTarget");
                if (found == null)
                {
                    GameObject targetObj = new GameObject("CameraTarget");
                    targetObj.transform.SetParent(transform, false);
                    targetObj.transform.localPosition = new Vector3(0f, 1.45f, 0f);
                    found = targetObj.transform;
                }
                cameraFollowTarget = found;
            }
        }

        private void Update()
        {
            // Uzak (Remote) oyuncular kendi girişlerini çalıştırmaz; ağ senkronizasyonu konumlarını günceller
            if (!IsLocalPlayer)
            {
                return;
            }

            inputSource.ReadInputs(true);
            bool isGrounded = groundChecker.EvaluateGrounded(characterController);

            UpdateVerticalGravity(isGrounded);
            UpdatePlanarMovement(isGrounded);
        }

        private void UpdateVerticalGravity(bool isGrounded)
        {
            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickVelocity;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void UpdatePlanarMovement(bool isGrounded)
        {
            Vector2 moveInput = inputSource.MoveInput;
            bool isRunning = inputSource.IsSprintPressed && moveInput.sqrMagnitude > 0.01f;
            float targetSpeed = moveInput.sqrMagnitude > 0.01f
                ? (isRunning ? runSpeed : walkSpeed) * moveInput.magnitude
                : 0f;

            currentPlanarSpeed = Mathf.Lerp(currentPlanarSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            if (currentPlanarSpeed < 0.01f)
            {
                currentPlanarSpeed = 0f;
            }

            Vector3 moveDirection = Vector3.zero;
            if (moveInput.sqrMagnitude > 0.001f)
            {
                float cameraYaw = cameraTransform != null ? cameraTransform.eulerAngles.y : transform.eulerAngles.y;
                float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraYaw;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }

            Vector3 displacement = moveDirection.normalized * (currentPlanarSpeed * Time.deltaTime)
                                   + new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

            characterController.Move(displacement);

            animatorBridge.UpdateAnimationState(currentPlanarSpeed, runSpeed, isGrounded, isRunning);
        }
    }
}
