using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Game.Logic.Scripts.General
{
    public class CloseUiScript : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast to all UI under the mouse
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                bool pointerIsOnThisPanel = false;

                foreach (var result in raycastResults)
                {
                    Debug.Log("Raycast hit: " + result.gameObject.name);

                    if (result.gameObject == gameObject || result.gameObject.transform.IsChildOf(transform))
                    {
                        pointerIsOnThisPanel = true;
                        break;
                    }
                }

                // Only close if NOT on this panel or its children
                if (!pointerIsOnThisPanel)
                {
                    CloseUI();
                }
            }
        }

        void CloseUI()
        {
            // this does not work yet
            // You can hide or destroy. Let's hide for now to debug easier:
            //gameObject.SetActive(false); // Hide the panel for testing
            // Destroy(gameObject); // Uncomment to destroy instead
        }
    }
}