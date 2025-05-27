using System;
using UnityEngine;

namespace Game.Logic.Scripts.General
{
    public class POIScript : MonoBehaviour
    {
        public Transform tile;
        [SerializeField] private GameObject POIUiprefab;
        void OnMouseDown()
        {
            Debug.Log("POI clicked: ");
            var item = Instantiate(
                POIUiprefab,
                new Vector3()
                {
                    x=tile.position.x + 1f,
                    y=tile.position.y,
                    z=tile.position.z + 1f
                },
                Quaternion.identity,
                tile
            );
            item.AddComponent<CloseUiScript>();
        }
    }
}