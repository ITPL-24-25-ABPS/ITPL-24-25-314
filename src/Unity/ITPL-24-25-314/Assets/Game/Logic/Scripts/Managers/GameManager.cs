using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Unit selectedUnit;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);    
    }

    public void SelectUnit(Unit unit)
    {
        if (selectedUnit != null)
            selectedUnit.SetSelected(false);

        selectedUnit = unit;
        selectedUnit.SetSelected(true);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnit != null)
        {
            Debug.Log("Right-click detected!");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Unit target = hit.collider.GetComponent<Unit>();
                if (target != null && target != selectedUnit)
                {
                    selectedUnit.Attack(target);
                }
                else
                {
                    selectedUnit.MoveTo(hit.point);
                }
            }
        }
    }

}
