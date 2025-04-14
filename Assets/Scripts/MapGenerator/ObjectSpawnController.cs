using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnController : BaseManager
{
    [Header("Debug")]
    [SerializeField]
    private bool debug;

    [Header("Dependencies")]
    [SerializeField]
    private MapManager _map;
    private TileManager _tile;

    [Header("Entity")]
    public List<GameObject> objectsPrefabs; // Префабы объектов для размещения

    [Header("SpawnSettings")]
    [SerializeField]
    [Range(0, 1)] public float spawnDensity = 0.1f; // Плотность размещения
    private int minDistanceBetweenObjects = 1; // Минимальное расстояние

    public Transform parentPrefab;
    public Transform parentObj;

    public void Boot(MapManager map, TileManager tile)
    {
        _map = map;
        _tile = tile;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    public void PlaceObjects(List<Vector3Int> availablePositions)
    {
        if (parentObj != null)
        {
            Destroy(parentObj.gameObject);
            _tile.occupationMap.Clear();
        }

        parentObj = Instantiate(parentPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        List<Vector3Int> positionsToRemove = new(); // Сюда будем сохранять тайлы, занятые объектами

        foreach (var tilePos in availablePositions)
        {
            if (Random.value < spawnDensity && IsPositionValid(tilePos))
            {
                GameObject randomPrefab = objectsPrefabs[Random.Range(0, objectsPrefabs.Count)];
                Vector3 worldPos = _map.GetTileCenterWorld(tilePos);
                Instantiate(randomPrefab, new Vector3(worldPos.x, worldPos.y, -1), Quaternion.identity, parentObj);

                _tile.SetTileOccupied(tilePos, true);
                positionsToRemove.Add(tilePos); // 💡 Запоминаем занятый тайл

                if (debug) Debug.Log($"tilePos - {tilePos}");
            }
        }

        // 💥 Удаляем занятые тайлы из доступных
        foreach (var pos in positionsToRemove)
        {
            availablePositions.Remove(pos);
        }
    }

    private bool IsPositionValid(Vector3Int position)
    {
        foreach (var occupied in _tile.occupationMap)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }
        return true;
    }
}