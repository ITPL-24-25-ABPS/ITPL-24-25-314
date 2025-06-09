using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [Space(10)]
    [Header("PlayerID")]
    [SerializeField] private int playerId;
    
    [Space(10)]
    [Header("Ressources")]
    [SerializeField] public RessourceManager ressourceManager;
    
    [Space(10)]
    [Header("Units")]
    [SerializeField] public TroopsManager troopsManager;
    
    public Transform unitParent;
        
    public void Init(int id)
    {
        playerId = id;
        ressourceManager = gameObject.AddComponent<RessourceManager>();
        troopsManager = gameObject.AddComponent<TroopsManager>();
        troopsManager.SetPlayer(this);
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("I am the local player!");
            FindObjectOfType<MainGameUI>().AssignPlayer(this);
        }
    }
}
