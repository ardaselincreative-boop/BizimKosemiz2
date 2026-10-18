namespace BizimKosemiz.Networking
{
    /// <summary>
    /// Oyuncu nesnesinin yerel istemciye (Local Owner) ait olup olmadığını belirleyen arayüz.
    /// Tek oyunculu prototipte her zaman true döner; Aşama 5'te Netcode for GameObjects (IsOwner)
    /// tarafından yönetilir. Böylece kamera ve giriş çakışmaları mimari düzeyde engellenir.
    /// </summary>
    public interface IPlayerAuthority
    {
        bool IsLocalPlayerControlled { get; }
        ulong NetworkClientId { get; }
    }
}
