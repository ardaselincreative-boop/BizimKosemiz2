#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Netcode;
using UnityEngine;

namespace BizimKosemiz.Editor
{
    /// <summary>
    /// Aşama 5 için oyuncu prefab'ına NetworkObject ekler.
    /// NetworkManager'ı sahneye otomatik yerleştirmez; kullanıcı mevcut sahne kurulumunu kontrol eder.
    /// </summary>
    public static class NetworkingSetup
    {
        private const string PlayerPrefabPath = "Assets/Prefabs/Player/Player.prefab";

        [MenuItem("Bizim Köşemiz/3. Aşama 5 - Oyuncu Prefab'ını Ağ İçin Hazırla")]
        public static void PreparePlayerPrefab()
        {
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            try
            {
                if (prefabContents.GetComponent<NetworkObject>() == null)
                    prefabContents.AddComponent<NetworkObject>();

                PrefabUtility.SaveAsPrefabAsset(prefabContents, PlayerPrefabPath);
                AssetDatabase.SaveAssets();
                Debug.Log("Player.prefab üzerinde NetworkObject doğrulandı/eklendi. Şimdi GardenScene içine NetworkManager + UnityTransport ekleyip Player Prefab alanını atayın.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
        }

        [MenuItem("Bizim Köşemiz/3. Aşama 5 - Oyuncu Prefab'ını Ağ İçin Hazırla", true)]
        private static bool ValidatePreparePlayerPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath) != null;
        }
    }
}
#endif
