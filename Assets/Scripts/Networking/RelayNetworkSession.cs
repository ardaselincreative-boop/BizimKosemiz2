using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;

namespace BizimKosemiz.Networking
{
    /// <summary>
    /// Aşama 5 başlangıç noktası: Relay üzerinden Host/Join Code oturumu.
    /// Sahneye bir NetworkManager ve UnityTransport eklenmiş olmalıdır.
    /// NetworkManager Player Prefab alanında NetworkObject içeren oyuncu prefab'ı atanmalıdır.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RelayNetworkSession : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxPlayers = 4;
        [SerializeField] private string connectionType = "wss";

        public string JoinCode { get; private set; } = string.Empty;
        public bool IsBusy { get; private set; }

        public event Action<string> StatusChanged;
        public event Action<string> JoinCodeCreated;

        public async void StartHostFromUi()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await EnsureServicesReadyAsync();
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                UnityTransport transport = GetTransport();
                ConfigureWebSocketRelay(transport, allocation);

                if (!NetworkManager.Singleton.StartHost())
                    throw new InvalidOperationException("NetworkManager Host başlatılamadı.");

                JoinCodeCreated?.Invoke(JoinCode);
                Report("Host hazır. Katılım kodu: " + JoinCode);
            }
            catch (Exception exception)
            {
                Report("Host başlatma hatası: " + exception.Message);
            }
            finally { IsBusy = false; }
        }

        public async void JoinFromUi(string joinCode)
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(joinCode))
            {
                Report("Katılım kodu boş olamaz.");
                return;
            }

            IsBusy = true;
            try
            {
                await EnsureServicesReadyAsync();
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode.Trim().ToUpperInvariant());

                UnityTransport transport = GetTransport();
                ConfigureWebSocketRelay(transport, allocation);

                if (!NetworkManager.Singleton.StartClient())
                    throw new InvalidOperationException("NetworkManager Client başlatılamadı.");

                Report("Host'a bağlanılıyor...");
            }
            catch (Exception exception)
            {
                Report("Katılma hatası: " + exception.Message);
            }
            finally { IsBusy = false; }
        }

        public void ShutdownSession()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
                NetworkManager.Singleton.Shutdown();

            JoinCode = string.Empty;
            Report("Oturum kapatıldı.");
        }

        private static async Task EnsureServicesReadyAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void ConfigureWebSocketRelay(UnityTransport transport, Allocation allocation)
        {
            // Browser clients cannot open raw UDP sockets. WSS is required for WebGL.
            transport.UseWebSockets = true;
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
        }

        private void ConfigureWebSocketRelay(UnityTransport transport, JoinAllocation allocation)
        {
            // Keep both peers on the same browser-compatible Relay protocol.
            transport.UseWebSockets = true;
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
        }

        private UnityTransport GetTransport()
        {
            if (NetworkManager.Singleton == null)
                throw new InvalidOperationException("Sahnede NetworkManager bulunamadı.");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
                throw new InvalidOperationException("NetworkManager üzerinde UnityTransport bulunamadı.");

            return transport;
        }

        private void Report(string message)
        {
            Debug.Log("[Bizim Köşemiz Network] " + message, this);
            StatusChanged?.Invoke(message);
        }
    }
}
