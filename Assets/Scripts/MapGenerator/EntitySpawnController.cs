using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntitySpawnController : BaseManager
{
    [Header("Debug")]
    [SerializeField]
    private bool debug = false;

    [Header("MapSettings")]
    [SerializeField]
    private int _width = 100;
    private int _height = 100;

    [Header("Dependencies")]
    [SerializeField]
    private TeamManager _team;
    private MapManager _map;
    private MapGenerationController _mapGenerator;

    [Header("Components")]
    [SerializeField]
    private Tilemap tileMap;

    public void Boot(TeamManager team, MapManager map, MapGenerationController mapGenerator, int width, int height)
    {
        _team = team;
        _map = map;
        _mapGenerator = mapGenerator;

        _width = width;
        _height = height;
    }

    public void PlaceEntities()
    {
        // Размещаем монстров вдоль края карты
        foreach (var monsterPrefab in _team.monsterTeam)
        {
            Vector3Int edgePosition = FindEdgePosition(); // Находим позицию вдоль края карты
            Vector3 newPos = _map.GetTileCenterWorld(edgePosition); newPos.z = -1;
            monsterPrefab.transform.position = newPos;
            monsterPrefab.SwitchVisibility(true);

            Debug.Log($"Монстр размещён на позиции: {edgePosition}");
        }

        // Размещаем охотников по кругу в центре карты
        Vector3Int centerPosition = new Vector3Int(_width / 2, _height / 2, -1);
        if (!IsGround(centerPosition))
        {
            centerPosition = FindNearestGround(centerPosition); // Ищем ближайшую землю
        }

        float radius = 3f; // Радиус круга
        int numberOfHunters = _team.hunterTeam.Count;
        float angleStep = 360f / numberOfHunters; // Угол между охотниками

        for (int i = 0; i < numberOfHunters; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3Int hunterPosition = new Vector3Int(
                (int)(centerPosition.x + radius * Mathf.Cos(angle)),
                (int)(centerPosition.y + radius * Mathf.Sin(angle)),
                -1
            );

            if (!IsGround(hunterPosition))
            {
                hunterPosition = FindNearestGround(hunterPosition);
            }

            Vector3 newPos = _map.GetTileCenterWorld(hunterPosition); newPos.z = -1;
            _team.hunterTeam[i].transform.position = newPos;
            _team.hunterTeam[i].SwitchVisibility(true);

            Debug.Log($"Охотник размещён на позиции: {hunterPosition}");
        }

        // 🐸 Размещаем нейтралов случайно на доступных клетках
        List<Vector3Int> availableTiles = new List<Vector3Int>(_mapGenerator.passableTiles);

        foreach (var neutral in _team.neutralTeam)
        {
            if (availableTiles.Count == 0)
            {
                Debug.LogWarning("Нет доступных клеток для размещения нейтралов!");
                break;
            }

            int index = Random.Range(0, availableTiles.Count);
            Vector3Int pos = availableTiles[index];
            availableTiles.RemoveAt(index); // Чтобы не ставить двух нейтралов на одну клетку

            Vector3 newPos = _map.GetTileCenterWorld(pos); newPos.z = -1;
            neutral.transform.position = newPos;
            neutral.SwitchVisibility(true);

            Debug.Log($"Нейтрал размещён на позиции: {pos}");
        }
    }

    // Проверяем, является ли клетка сушей
    bool IsGround(Vector3Int position)
    {
        bool result = _mapGenerator.passableTiles.Contains(position);
        if (debug) Debug.Log($"Проверка клетки {position} на сушу: {result}");
        return result;
    }

    // Находим ближайшую землю к указанной позиции
    Vector3Int FindNearestGround(Vector3Int start)
    {
        float minDistance = float.MaxValue;
        Vector3Int bestPosition = start;

        foreach (var groundTile in _mapGenerator.passableTiles)
        {
            float distance = Vector3.Distance(start, groundTile);
            if (distance < minDistance)
            {
                minDistance = distance;
                bestPosition = groundTile;
            }
        }

        return bestPosition;
    }

    Vector3Int FindEdgePosition()
    {
        List<Vector3Int> edges = new List<Vector3Int>();

        // Добавляем все позиции вдоль краев карты
        for (int x = 0; x < _width; x++)
        {
            edges.Add(new Vector3Int(x, 0, -1)); // Верхний край
            edges.Add(new Vector3Int(x, _height, -1)); // Нижний край
        }
        for (int y = 0; y < _height; y++)
        {
            edges.Add(new Vector3Int(0, y, -1)); // Левый край
            edges.Add(new Vector3Int(_width, y, -1)); // Правый край
        }

        Debug.Log($"Всего клеток на краях: {edges.Count}");

        // Фильтруем только те, что на суше
        edges = edges.FindAll(IsGround);

        Debug.Log($"Клеток на краях, которые являются сушей: {edges.Count}");

        // Выбираем случайную позицию
        if (edges.Count > 0)
        {
            Vector3Int chosenPosition = edges[Random.Range(0, edges.Count)];
            Debug.Log($"Выбрана краевая позиция: {chosenPosition}");
            return chosenPosition;
        }

        // Если вся карта - вода, выводим предупреждение
        Debug.LogWarning("Не найдено краевых клеток на суше, возвращаем центр карты.");
        return new Vector3Int(_width / 2, _height / 2, -1);
    }
}
