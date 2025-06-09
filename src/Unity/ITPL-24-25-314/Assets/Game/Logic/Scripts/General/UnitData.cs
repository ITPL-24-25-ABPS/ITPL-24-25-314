using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("General")]
    public string unitName;
    public GameObject prefab;
    public Sprite icon;
    public float cost;

    [Header("Stats")]
    public int maxHealth = 100;
    public int attackDamage = 10;
    public int attackRange = 1; // hexes
    public int movementPoints = 3;
    public int visionRange = 2; // hexes
}
