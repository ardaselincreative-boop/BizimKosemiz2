using UnityEngine;

namespace BizimKosemiz.Networking
{
    /// <summary>
    /// Oyuncu prefab'ı üzerinde yerel/uzak oyuncu yetkisini yöneten adaptör.
    /// Aşama 1-4 arasında yerel prototip olarak çalışır; Aşama 5'te NetworkBehaviour ile
    /// konum, dönüş ve animasyon senkronizasyonunu üstlenir.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerNetworkAdapter : MonoBehaviour, IPlayerAuthority
    {
        [SerializeField] private bool isLocalPlayerInPrototype = true;
        [SerializeField] private ulong simulatedClientId = 0;

        public bool IsLocalPlayerControlled => isLocalPlayerInPrototype;
        public ulong NetworkClientId => simulatedClientId;

        public void ConfigureAuthority(bool isLocalOwner, ulong clientId = 0)
        {
            isLocalPlayerInPrototype = isLocalOwner;
            simulatedClientId = clientId;
        }
    }
}
