using UnityEngine;

public class RessourceManager : MonoBehaviour
{
    [Space(10)] [Header("Passiv Ressource gen")] 
    [SerializeField] public float grain;
    [SerializeField] private float wood;
    [SerializeField] private float stone;
    [SerializeField] private float gold;
    [SerializeField] private float grainPerSecond = 1f;
    void Start()
    {
        InvokeRepeating(nameof(AddGrain), 1f, 1f);
    }

    void AddGrain()
    {
        grain += grainPerSecond;
    }

    public bool TrySpendGrain(float amount)
    {
        if (grain >= amount)
        {
            grain -= amount;
            return true;
        }
        return false;
    }

    public float GetGrain()
    {
        return grain;
    }
}
