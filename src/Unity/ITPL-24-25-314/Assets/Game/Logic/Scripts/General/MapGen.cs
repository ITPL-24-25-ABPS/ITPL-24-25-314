using System.Collections.Generic;
using Game.Logic.Scripts.General;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGen : MonoBehaviour
{
    
    [SerializeField] private int _mapWidth = 10;
    [SerializeField] private int _mapHeigth = 10;
    [SerializeField] private float _tileSize = 1;
    [SerializeField] private Transform _mapParent;
    
    [SerializeField] private GameObject _tileGrassPrefab;
    [SerializeField] private GameObject _tileWaterPrefab;
    [SerializeField] private GameObject _tileDesertPrefab;
    [SerializeField] private GameObject _tileGoldPrefab;
    [SerializeField] private GameObject _tileWastelandPrefab;
    [SerializeField] private GameObject _poiPrefab;

    [SerializeField] private float _noiseSeed = 1276473;
    [SerializeField] private float _noiseFrequency = 100f;
    [SerializeField] private float _WaterThreashold = 0.5f;
    [SerializeField] private float _DesertThreashold = 0.5f;
    [SerializeField] private float _GoldThreashold = 0.5f;
    [SerializeField] private float _WasteThreashold = 0.5f;

    private TileScript[,] _tiles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeMapGrid();
        PlacePOIs(2);
    }

    private Vector2 GetHexCoords(int x, int z){
            float xPos = x * (_tileSize * 0.75f * 2); // 1.5 times _tileSize
            float zPos = z * (_tileSize * Mathf.Sqrt(3)) + (x % 2) * (_tileSize * Mathf.Sqrt(3) / 2.0f);
            return new Vector2(xPos, zPos);
    }

    // Update is called once per frame
    void MakeMapGrid()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int z = 0; z < _mapHeigth; z++)
            {
                Vector2 hexCoords = GetHexCoords(x, z);
                Vector3 position = new Vector3(hexCoords.x, 0, hexCoords.y);
                Quaternion rotation = Quaternion.Euler(180, 0, 0); 

                if (_noiseSeed == -1){
                    _noiseSeed = Random.Range(0, 1000000);
                }
                
                float noise1 = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.y + _noiseSeed) / _noiseFrequency);
                float noise2 = Mathf.PerlinNoise((hexCoords.x + 100 + _noiseSeed) / _noiseFrequency, (hexCoords.y + 100 + _noiseSeed) / _noiseFrequency);
                float combinedNoise = (noise1 + noise2) / 2.0f;

                GameObject prefab = _tileGrassPrefab;

                if (combinedNoise > _DesertThreashold)
                {
                    prefab = _tileDesertPrefab;
                }
                else if (combinedNoise < _WaterThreashold)
                {
                    prefab = _tileWaterPrefab;
                }
                else if (combinedNoise == _GoldThreashold)
                {
                    prefab = _tileGoldPrefab;
                }
                else if (combinedNoise < _WasteThreashold)
                {
                    prefab = _tileWastelandPrefab;
                }

                GameObject tile = Instantiate(prefab, position, rotation, _mapParent);
                tile.AddComponent<TileScript>();
                tile.AddComponent<BoxCollider>();
                var script = tile.GetComponent<TileScript>();
                script.X = x;
                script.Z = z;
                AddHexBorder(tile);
            }
        }
    }

    void PlacePOIs(int percentage)
    {
        Debug.Log("Placing");
        List<Transform> tiles = new ();
        List<Transform> poiTiles = new ();

        for (int i = 0; i < _mapParent.childCount; i++)
            tiles.Add(_mapParent.GetChild(i));

        Debug.Log("Tiles: " + tiles.Count);

        int numPOIs = Mathf.FloorToInt(tiles.Count * percentage / 100f);

        Debug.Log("Num POIs: " + numPOIs);


        for (int i = 0; i < numPOIs; i++)
        {
            int randomIndex = Random.Range(0, tiles.Count);
            Transform tile = tiles[randomIndex];
            TileScript tileScript = tile.GetComponent<TileScript>();
            tileScript.poiType = POIType.Resource; // Set the POI type
            tiles.RemoveAt(randomIndex); // Remove the tile from the list to avoid duplicates
            poiTiles.Add(tile);
        }

        foreach (var tile in poiTiles)
        {
            var script = tile.GetComponent<TileScript>();
            var transform = tile.transform;
            Object cube = Instantiate(_poiPrefab, new Vector3() {x = tile.position.x, y = tile.position.y, z = tile.position.z}, Quaternion.identity, tile);
            var poiScript = cube.GetComponent<POIScript>();
            poiScript.tile = tile;
            cube.name = "POI";
        }

    }


    void AddHexBorder(GameObject tile)
    {
        LineRenderer lineRenderer = tile.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 7; // 6 corners + 1 to close the hexagon
        lineRenderer.startWidth = 0.04f; // Increase the width
        lineRenderer.endWidth = 0.04f;   // Increase the width
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;

        // Set the material and color of the border
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        float radius = _tileSize * 1f; // Increase the radius slightly
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60 * i);
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            lineRenderer.SetPosition(i, new Vector3(x, 0, y));
        }
        lineRenderer.SetPosition(6, lineRenderer.GetPosition(0)); // Close the hexagon
    }


}