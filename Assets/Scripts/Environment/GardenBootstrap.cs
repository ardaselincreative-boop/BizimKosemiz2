using BizimKosemiz.Networking;
using BizimKosemiz.Player;
using BizimKosemiz.UI;
using UnityEngine;

namespace BizimKosemiz.Environment
{
    /// <summary>
    /// GardenScene açıldığında veya Play Mode başlatıldığında:
    /// 1) Bahçe çevresinin ve URP materyallerinin sağlamlığını doğrular,
    /// 2) Spawn noktalarının zemin üzerinde olduğunu kontrol eder,
    /// 3) Yerel oyuncu karakterini doğru ölçek ve ayak hizasıyla (y = 0) hazırlar,
    /// 4) Üçüncü şahıs kamerayı yerel oyuncuya bağlar.
    /// </summary>
    [DisallowMultipleComponent]
    public class GardenBootstrap : MonoBehaviour
    {
        [SerializeField] private GardenEnvironmentBuilder environmentBuilder;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private PlayerMovementController scenePlayer;
        [SerializeField] private ThirdPersonCameraController mainCameraController;
        [SerializeField] private DebugHudOverlay hudOverlay;

        private void Awake()
        {
            if (environmentBuilder == null)
            {
                environmentBuilder = GetComponent<GardenEnvironmentBuilder>();
                if (environmentBuilder == null)
                {
                    environmentBuilder = gameObject.AddComponent<GardenEnvironmentBuilder>();
                }
            }
            environmentBuilder.ValidateAndPopulateGarden();

            if (spawnManager == null)
            {
                spawnManager = GetComponent<SpawnManager>();
                if (spawnManager == null)
                {
                    spawnManager = gameObject.AddComponent<SpawnManager>();
                }
            }

            EnsurePlayerAndCamera();
        }

        private void EnsurePlayerAndCamera()
        {
            if (scenePlayer == null)
            {
                scenePlayer = FindObjectOfType<PlayerMovementController>();
            }

            if (scenePlayer != null)
            {
                // Oyuncu görsel materyallerini URP/Built-in uyumlu olarak doğrula
                Renderer[] playerRenderers = scenePlayer.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < playerRenderers.Length; i++)
                {
                    Color color = playerRenderers[i].gameObject.name.Contains("Visor")
                        ? new Color(0.96f, 0.76f, 0.28f, 1f)
                        : new Color(0.24f, 0.52f, 0.86f, 1f);
                    ShaderCompatibilityUtility.EnsureValidMaterial(playerRenderers[i], color, 0.35f);
                }

                // Oyuncuyu doğrulanmış zemin üstü spawn noktasına yerleştir
                spawnManager.GetNextSpawnPose(out Vector3 spawnPos, out Quaternion spawnRot);
                CharacterController cc = scenePlayer.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                scenePlayer.transform.SetPositionAndRotation(spawnPos, spawnRot);
                if (cc != null) cc.enabled = true;
            }

            if (mainCameraController == null && Camera.main != null)
            {
                mainCameraController = Camera.main.GetComponent<ThirdPersonCameraController>();
                if (mainCameraController == null)
                {
                    mainCameraController = Camera.main.gameObject.AddComponent<ThirdPersonCameraController>();
                }
            }

            if (mainCameraController != null && scenePlayer != null)
            {
                LocalPlayerInputSource input = scenePlayer.GetComponent<LocalPlayerInputSource>();
                mainCameraController.BindLocalPlayerTarget(scenePlayer.CameraFollowTarget, input);
            }

            if (hudOverlay == null)
            {
                hudOverlay = GetComponent<DebugHudOverlay>();
                if (hudOverlay == null)
                {
                    hudOverlay = gameObject.AddComponent<DebugHudOverlay>();
                }
            }
            if (hudOverlay != null && scenePlayer != null)
            {
                hudOverlay.BindPlayer(scenePlayer);
            }
        }
    }
}
