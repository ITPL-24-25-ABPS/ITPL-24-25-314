using UnityEngine;

public class UnitInputController : MonoBehaviour
{
    public TurnSystem turnSystem;

    void Update()
    {
        Unit active = turnSystem.GetActiveUnit();
        if (active == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Select hex or target unit under mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Unit target = hit.collider.GetComponent<Unit>();
                if (target != null && target.owner != active.owner)
                {
                    active.Attack(target);
                    turnSystem.EndTurn();
                }
                else
                {
                    active.MoveTo(hit.point);
                    turnSystem.EndTurn();
                }
            }
        }
    }
}
