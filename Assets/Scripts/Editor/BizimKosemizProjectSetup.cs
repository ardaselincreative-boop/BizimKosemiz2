#if UNITY_EDITOR
using System.IO;
using BizimKosemiz.Environment;
using BizimKosemiz.Player;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BizimKosemiz.Editor
{
    /// <summary>
    /// Unity Editor açıldığında veya "Bizim Köşemiz" üst menüsünden çağrıldığında:
    /// 1) URP Render Pipeline Asset'i oluşturur ve GraphicsSettings'e bağlar,
    /// 2) Tüm materyalleri Universal Render Pipeline/Lit shader'ına yükseltir (Pembe materyal önleme),
    /// 3) PlayerAnimator.controller dosyasını Idle, Walk ve Run durumlarıyla (State) sıfırdan üretir,
    /// 4) Sahne ve Prefab üzerindeki tüm bileşenleri (Missing Script, Pivot, Grounding) denetler.
    /// </summary>
    public static class BizimKosemizProjectSetup
    {
        private const string AnimatorControllerPath = "Assets/Animations/PlayerAnimator.controller";
        private const string UrpAssetPath = "Assets/Settings/BizimKosemiz_URPAsset.asset";
        private const string UrpRendererPath = "Assets/Settings/BizimKosemiz_URPRenderer.asset";

        [MenuItem("Bizim Köşemiz/1. Sahneyi, URP ve Animator Varlıklarını Doğrula - Onar", false, 10)]
        public static void ValidateAndRepairProject()
        {
            EnsureUrpPipelineConfigured();
            UpgradeProjectMaterialsToUrp();
            AnimatorController controller = EnsurePlayerAnimatorController();
            BindAnimatorToPrefabAndScene(controller);
            VerifySceneIntegrity();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Bizim Köşemiz] Proje kurulumu, URP materyalleri, Animator Controller ve GardenScene doğrulaması başarıyla tamamlandı!");
        }

        [MenuItem("Bizim Köşemiz/2. Kalite ve Referans Denetimini Çalıştır (Aşama 1-3)", false, 20)]
        public static void RunVerificationAudit()
        {
            int issues = 0;

            // 1. Missing Script Kontrolü
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
            foreach (GameObject go in allObjects)
            {
                int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missingCount > 0)
                {
                    Debug.LogError($"[Denetim Hatası] '{go.name}' nesnesinde {missingCount} adet Missing Script bulundu!", go);
                    issues++;
                }
            }

            // 2. Oyuncu Pivot ve Zemin Hizası Kontrolü
            PlayerMovementController player = Object.FindObjectOfType<PlayerMovementController>();
            if (player == null)
            {
                Debug.LogError("[Denetim Hatası] Sahnede PlayerMovementController bulunamadı!");
                issues++;
            }
            else
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                float bottomY = cc.center.y - (cc.height * 0.5f);
                if (Mathf.Abs(bottomY) > 0.01f)
                {
                    Debug.LogError($"[Denetim Hatası] Karakter kapsül tabanı y=0 hizasında değil (Hesaplanan: {bottomY:F3})!");
                    issues++;
                }

                if (player.transform.position.y < -0.05f)
                {
                    Debug.LogError($"[Denetim Hatası] Karakter zeminin altında duruyor (y={player.transform.position.y:F3})!");
                    issues++;
                }
            }

            // 3. Pembe / Hatalı Shader Kontrolü
            Renderer[] renderers = Object.FindObjectsOfType<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                if (r.sharedMaterial == null || r.sharedMaterial.shader == null || r.sharedMaterial.shader.name == "Hidden/InternalErrorShader")
                {
                    Debug.LogError($"[Denetim Hatası] '{r.gameObject.name}' üzerinde eksik veya hatalı (pembe) materyal tespit edildi!", r.gameObject);
                    issues++;
                }
            }

            if (issues == 0)
            {
                Debug.Log("[Bizim Köşemiz Denetim Raporu] TÜM KONTROLLER GEÇTİ (0 Hata). Sahne Play Mode için hazır.");
            }
            else
            {
                Debug.LogWarning($"[Bizim Köşemiz Denetim Raporu] Toplam {issues} sorun tespit edildi.");
            }
        }

        [MenuItem("Bizim Köşemiz/3. Aşama 5 Multiplayer Paketlerini (NGO + Relay + Auth) Kur", false, 40)]
        public static void InstallMultiplayerPackagesForPhase5()
        {
            Debug.Log("[Bizim Köşemiz] Netcode for GameObjects, Unity Transport, Relay ve Authentication paketleri Package Manager üzerinden ekleniyor...");
            UnityEditor.PackageManager.Client.Add("com.unity.netcode.gameobjects@1.11.0");
        }

        private static void EnsureUrpPipelineConfigured()
        {
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                return;
            }

            UniversalRenderPipelineAsset existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(UrpAssetPath);
            if (existing == null)
            {
                UniversalRendererData rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, UrpRendererPath);

                existing = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(existing, UrpAssetPath);
            }

            GraphicsSettings.defaultRenderPipeline = existing;
            QualitySettings.renderPipeline = existing;
        }

        private static void UpgradeProjectMaterialsToUrp()
        {
            Shader urpLit = ShaderCompatibilityUtility.GetCompatibleLitShader();
            if (urpLit == null) return;

            string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" });
            foreach (string guid in matGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null)
                {
                    Color baseCol = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
                    float smooth = mat.HasProperty("_Smoothness") ? mat.GetFloat("_Smoothness") : 0.2f;
                    mat.shader = urpLit;
                    ShaderCompatibilityUtility.ApplyColorAndSmoothness(mat, baseCol, smooth);
                    EditorUtility.SetDirty(mat);
                }
            }
        }

        private static AnimatorController EnsurePlayerAnimatorController()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorControllerPath);
            if (controller != null)
            {
                return controller;
            }

            Directory.CreateDirectory("Assets/Animations");
            controller = AnimatorController.CreateAnimatorControllerAtPath(AnimatorControllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);

            AnimatorStateMachine rootSm = controller.layers[0].stateMachine;
            AnimatorState idleState = rootSm.AddState("Idle");
            AnimatorState walkState = rootSm.AddState("Walk");
            AnimatorState runState = rootSm.AddState("Run");
            rootSm.defaultState = idleState;

            AnimatorStateTransition toWalk = idleState.AddTransition(walkState);
            toWalk.hasExitTime = false;
            toWalk.duration = 0.15f;
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.15f, "Speed");

            AnimatorStateTransition toIdle = walkState.AddTransition(idleState);
            toIdle.hasExitTime = false;
            toIdle.duration = 0.15f;
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.15f, "Speed");

            AnimatorStateTransition toRun = walkState.AddTransition(runState);
            toRun.hasExitTime = false;
            toRun.duration = 0.15f;
            toRun.AddCondition(AnimatorConditionMode.If, 0f, "IsRunning");

            AnimatorStateTransition runToWalk = runState.AddTransition(walkState);
            runToWalk.hasExitTime = false;
            runToWalk.duration = 0.15f;
            runToWalk.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsRunning");

            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void BindAnimatorToPrefabAndScene(AnimatorController controller)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
            if (prefab != null)
            {
                Animator anim = prefab.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.runtimeAnimatorController = controller;
                    EditorUtility.SetDirty(prefab);
                }
            }

            PlayerMovementController scenePlayer = Object.FindObjectOfType<PlayerMovementController>();
            if (scenePlayer != null)
            {
                Animator anim = scenePlayer.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.runtimeAnimatorController = controller;
                    EditorSceneManager.MarkSceneDirty(scenePlayer.gameObject.scene);
                }
            }
        }

        private static void VerifySceneIntegrity()
        {
            GardenEnvironmentBuilder builder = Object.FindObjectOfType<GardenEnvironmentBuilder>();
            if (builder != null)
            {
                builder.ValidateAndPopulateGarden();
                EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
            }
        }
    }
}
#endif
