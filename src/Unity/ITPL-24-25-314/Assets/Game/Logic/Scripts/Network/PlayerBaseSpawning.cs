using Unity.Netcode;
using UnityEngine;

public class PlayerBaseSpawning : NetworkBehaviour
{
    [SerializeField] private GameObject basePrefab; 
    [SerializeField] private Transform baseParent; 
    [SerializeField] private float baseSpacing = 10f; 

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnBasesForPlayers();
        }
    }

    private void SpawnBasesForPlayers()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;

        for (int i = 0; i < playerCount; i++)
        {
            Vector3 spawnPosition = new Vector3(i * baseSpacing, 0, 0); 
            GameObject playerBase = Instantiate(basePrefab, spawnPosition, Quaternion.identity, baseParent);
            playerBase.GetComponent<NetworkObject>().Spawn(); 
        }
    }
}
