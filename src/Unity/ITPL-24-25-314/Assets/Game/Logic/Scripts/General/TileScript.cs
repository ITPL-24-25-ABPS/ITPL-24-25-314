using UnityEngine;

namespace Game.Logic.Scripts.General
{
    public class TileScript : MonoBehaviour
    {
        public int X, Z;               // Coordinates in the grid
        public POIType poiType;        // Enum (Castle, Resource, etc.)
        public int owner = 0;          // 0 = neutral, 1 = player 1, 2 = player 2
        public bool isVisibleP1 = false; // Visibility for player 1
        public bool isVisibleP2 = false; // Visibility for player 2
    }
}