using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;

public class MainGameStartup : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;

    private async void Start()
    {
        Debug.Log("MainGameStartup running...");

        string joinCode = LobbyManager.cachedJoinCode;
        string hostId = LobbyManager.cachedHostId;

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogError("Join code is missing!");
            return;
        }

        if (string.IsNullOrEmpty(hostId))
        {
            Debug.LogError("Host ID is missing!");
            return;
        }

        string localId = AuthenticationService.Instance.PlayerId;
        bool isHost = (localId == hostId);

        Debug.Log($"LocalID: {localId} | HostID: {hostId} | I am host: {isHost}");

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (isHost)
        {
            Debug.Log("Creating Relay allocation for host...");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            Debug.Log("Starting host...");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("Joining Relay allocation as client with code: " + joinCode);

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            Debug.Log("Starting client...");
            NetworkManager.Singleton.StartClient();
        }
    }
}
