using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Unit> allUnits = new List<Unit>();
    private int currentUnitIndex = 0;

    public void StartCombat(List<Unit> units)
    {
        allUnits = new List<Unit>(units);
        currentUnitIndex = 0;
        StartTurn();
    }

    public void EndTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % allUnits.Count;
        StartTurn();
    }

    private void StartTurn()
    {
        if (allUnits.Count == 0) return;

        Unit activeUnit = allUnits[currentUnitIndex];
        activeUnit.StartTurn();

        Debug.Log($"It's {activeUnit.name}'s turn.");
        // Optional: enable UI, input for this unit only
    }
    public Unit GetActiveUnit()
    {
        if (allUnits.Count == 0) return null;
        return allUnits[currentUnitIndex];
    }
}
