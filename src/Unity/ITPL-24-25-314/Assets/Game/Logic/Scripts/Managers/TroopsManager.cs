using System.Collections.Generic;
using UnityEngine;

public class TroopsManager : MonoBehaviour
{
    [Space(10)]
    [Header("Troops")]
    [SerializeField] private List<UnitData> unitTypes;

    private UnitData selectedUnit;
    private PlayerManager player;
    
    public void SetPlayer(PlayerManager p)
    {
        player = p;
    }

    public void SelectUnit(UnitData unit)
    {
        selectedUnit = unit;
    }

    public void TrySpawnSelectedUnit(Vector3 position)
    {
        if (selectedUnit == null) return;

        var resourceManager = player.ressourceManager;

        if (resourceManager.TrySpendGrain(selectedUnit.cost))
        {
            GameObject unitGo = Instantiate(selectedUnit.prefab, position, Quaternion.identity, player.unitParent);
            
            Unit unit = unitGo.GetComponent<Unit>();
            unit.data = selectedUnit;
            unit.owner = player;
            unit.RevealVision();
        }
        else
        {
            Debug.Log("Not enough grain");
        }
    }

    public List<UnitData> GetAvailableUnits() => unitTypes;
}
