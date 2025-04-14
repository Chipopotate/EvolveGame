using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private MapManager _map;
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _mainTilemap;

    public List<Vector3Int> occupationMap = new List<Vector3Int>();

    public void Boot(MapManager map)
    {
        _map = map;
        _grid = _map.grid;
        _mainTilemap = _map.mainTilemap;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован!");
    }

    public bool IsTileOccupied(Vector3Int position)
    {
        return occupationMap.Contains(position);
    }

    public void SetTileOccupied(Vector3Int position, bool occupied)
    {
        if (debug) Debug.Log($"Position - {position}");
        if (occupied && !occupationMap.Contains(position))
        {
            occupationMap.Add(position);
            SetDebugColor(position, new Color(0.95f, 0.95f, 0.95f, 1f));
        }
        else if (!occupied && occupationMap.Contains(position))
        {
            occupationMap.Remove(position);
            SetDebugColor(position, Color.white);
        }
    }

    private void SetDebugColor(Vector3Int position, Color color)
    {
        if (debug) Debug.Log($"Position_color - {position}");
        _mainTilemap.SetTileFlags(position, TileFlags.None);
        _mainTilemap.SetColor(position, color);
    }
}