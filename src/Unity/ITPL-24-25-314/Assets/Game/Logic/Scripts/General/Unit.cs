using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject highlightRing;
    
    public UnitData stats;
    public PlayerManager owner;

    private int currentHealth;
    private int movementLeft;
    private bool hasAttacked;

    public bool IsAlive => currentHealth > 0;
    
    public UnitData data;

    void Start()
    {
        currentHealth = stats.maxHealth;
        movementLeft = stats.movementPoints;
        hasAttacked = false;
    }

    public void StartTurn()
    {
        movementLeft = stats.movementPoints;
        hasAttacked = false;
        
        RevealVision();
    }

    public void MoveTo(Vector3 position)
    {
        Debug.Log("Moving to: " + position);
        if (movementLeft > 0)
        {
            transform.position = new Vector3(position.x, transform.position.y, position.z);
            movementLeft--;
            RevealVision();
        }
    }

    public void Attack(Unit target)
    {
        if (hasAttacked) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= stats.attackRange)
        {
            target.TakeDamage(stats.attackDamage);
            hasAttacked = true;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} has died.");
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        GameManager.Instance.SelectUnit(this);
    }
    public void SetSelected(bool selected)
    {
        if (highlightRing != null)
            highlightRing.SetActive(selected);
    }
    
    public void RevealVision()
    {
        if (owner != null)
        {
            int radius = data.visionRange; // from UnitData
            Vector3 unitPos = transform.position;

            MapGen map = FindObjectOfType<MapGen>();
            GameObject[,] fogGrid = map.FogGrid; 

            for (int x = 0; x < fogGrid.GetLength(0); x++)
            {
                for (int z = 0; z < fogGrid.GetLength(1); z++)
                {
                    Vector3 tilePos = fogGrid[x, z].transform.position;
                    float distance = Vector3.Distance(unitPos, tilePos);

                    if (distance <= radius)
                    {
                        fogGrid[x, z].SetActive(false);
                    }
                }
            }
        }
    }
    private Vector2Int GetClosestHexCoord(Vector3 position)
    {
        int closestX = 0;
        int closestZ = 0;
        float closestDist = float.MaxValue;

        for (int x = 0; x < MapGen.MapWidth; x++)
        {
            for (int z = 0; z < MapGen.MapHeight; z++)
            {
                Vector3 tilePos = MapGen.FogOverlays[x, z].transform.parent.position;
                float dist = Vector3.Distance(position, tilePos);
                if (dist < closestDist)
                {
                    closestX = x;
                    closestZ = z;
                    closestDist = dist;
                }
            }
        }
        return new Vector2Int(closestX, closestZ);
    }
}