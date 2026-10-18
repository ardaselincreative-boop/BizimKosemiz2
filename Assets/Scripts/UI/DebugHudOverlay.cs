using BizimKosemiz.Player;
using UnityEngine;

namespace BizimKosemiz.UI
{
    /// <summary>
    /// Oyun içinde kontrol tuşlarını, oyuncu hızını, zemin temas durumunu ve
    /// Aşama 5 Multiplayer hazırlık durumunu gösteren sade bilgi paneli.
    /// </summary>
    public class DebugHudOverlay : MonoBehaviour
    {
        [SerializeField] private bool showHud = true;
        private PlayerMovementController localPlayer;

        public void BindPlayer(PlayerMovementController player)
        {
            localPlayer = player;
        }

        private void OnGUI()
        {
            if (!showHud)
            {
                return;
            }

            GUI.Box(new Rect(16, 16, 360, 125), "Bizim Köşemiz — Oynanabilir Temel Sürüm (v0.1)");
            GUI.Label(new Rect(28, 44, 340, 22), "Hareket: WASD / Ok Tuşları | Koşma: Sol Shift");
            GUI.Label(new Rect(28, 66, 340, 22), "Kamera: Sağ Fare Tuşu Basılı Tut + Fareyi Hareket Ettir");

            if (localPlayer != null)
            {
                string groundedText = localPlayer.IsGrounded ? "Zeminde (Doğrulandı)" : "Havada";
                GUI.Label(
                    new Rect(28, 90, 340, 22),
                    $"Hız: {localPlayer.CurrentPlanarSpeed:F2} m/sn | Durum: {groundedText}"
                );
                GUI.Label(
                    new Rect(28, 112, 340, 22),
                    $"Konum (Ayak Hizası): {localPlayer.transform.position:F2}"
                );
            }
        }
    }
}
