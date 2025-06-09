using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MainGameUI : MonoBehaviour
{
    [Space(10)]
    [Header("Ressources")]
    public RessourceManager ressourceManager;
    public GameObject grainText;
    
    [Space(10)]
    [Header("Troops")]
    public TroopsManager troopsManager;
    public GameObject unitSelectionPanel;
    public Button spawnUnitButton;
    public Button showTroopsButton;
    public UnitData knightUnitData;
    
    public GameObject _mapParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnUnitButton.onClick.AddListener(() =>
        {
            if (troopsManager != null && knightUnitData != null)
            {
                troopsManager.SelectUnit(knightUnitData);

                var tiles = _mapParent.GetComponentsInChildren<Transform>();

                List<Transform> validTiles = new List<Transform>();
                foreach (Transform t in tiles)
                {
                    if (t != _mapParent && t.CompareTag("MainTile"))
                        validTiles.Add(t);
                }

                if (validTiles.Count > 0)
                {
                    Transform randomTile = validTiles[Random.Range(0, validTiles.Count)];
                    troopsManager.TrySpawnSelectedUnit(randomTile.position);
                }
                else
                {
                    Debug.LogWarning("No valid tiles found to spawn on.");
                }
            }
        });

        showTroopsButton.onClick.AddListener(() =>
        {
            if (unitSelectionPanel != null)
            {
                unitSelectionPanel.SetActive(!unitSelectionPanel.activeSelf);
            }
        });
    }
    public void AssignPlayer(PlayerManager player)
    {
        if (troopsManager != null)
        {
            troopsManager.SetPlayer(player);
        }

        if (ressourceManager == null)
        {
            ressourceManager = player.ressourceManager;
        }

        Debug.Log("Player assigned to UI.");
    }


    // Update is called once per frame
    void Update()
    {
    }
}
