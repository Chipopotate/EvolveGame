using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private Tilemap mainTilemap; // Основная карта для получения cellBounds
    [SerializeField] private Tilemap fogTilemap; // Слой для тумана
    [SerializeField] private TeamManager _teamManager;

    [Header("TilesDependencies")]
    [SerializeField] private TileBase fogTile; // Черный тайл для тумана
    [SerializeField] private TileBase exploredfogTile;  // Серый тайл для разведанной области

    // Словари для каждой команды (ключ — enum TeamType)
    private Dictionary<TeamManager.TeamType, bool[,]> fogGrids = new Dictionary<TeamManager.TeamType, bool[,]>();
    private Dictionary<TeamManager.TeamType, bool[,]> exploredGrids = new Dictionary<TeamManager.TeamType, bool[,]>();

    private Vector3Int origin;
    private HashSet<Vector3Int> currentVisibleTiles = new HashSet<Vector3Int>();

    public void Boot(TeamManager teamManager, MapManager mapManager)
    {
        _teamManager = teamManager;
        mainTilemap = mapManager.mainTilemap;
    }

    public override void Boot()
    {
        fogTilemap = GetComponent<Tilemap>();
        ValidateComponent(fogTilemap);

        // Проверяем зависимости
        ValidateAsset(fogTile);
        ValidateAsset(exploredfogTile);

        // Задаем origin как минимальные координаты основной Tilemap
        origin = mainTilemap.cellBounds.min;
        Vector3Int size = mainTilemap.cellBounds.size;

        // Инициализируем fogGrids и exploredGrids для всех команд
        foreach (TeamManager.TeamType team in System.Enum.GetValues(typeof(TeamManager.TeamType)))
        {
            bool[,] grid = new bool[size.x, size.y];
            bool[,] explored = new bool[size.x, size.y];
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    grid[x, y] = true;       // изначально все клетки покрыты туманом
                    explored[x, y] = false;  // разведанных еще нет
                }
            fogGrids.Add(team, grid);
            exploredGrids.Add(team, explored);
        }

        if (debug) Debug.Log(this.GetType().Name + " инициализирован!");
    }

    // Метод обновления тумана для конкретной команды
    public void UpdateFog(TeamManager.TeamType teamType)
    {
        if (debug) Debug.Log($"Начало обновления тумана для команды: {teamType}");

        // Обнуляем временные данные видимости
        bool[,] fogGrid = fogGrids[teamType];
        bool[,] exploredGrid = exploredGrids[teamType];
        if (debug) Debug.Log($"Инициализированы grid для команды: {teamType}");

        // Сначала заливаем всю карту тайлом тумана или разведанным тайлом
        Vector3Int size = mainTilemap.cellBounds.size;
        if (debug) Debug.Log($"Размер карты: {size.x}x{size.y}");

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0) + origin;
                if (exploredGrid[x, y])
                {
                    fogTilemap.SetTile(cell, exploredfogTile);
                    //if (debug) Debug.Log($"Установлен разведанный тайл на клетке: {cell}");
                }
                else
                {
                    fogTilemap.SetTile(cell, fogTile);
                    //if (debug) Debug.Log($"Установлен туманный тайл на клетке: {cell}");
                }
                fogGrid[x, y] = true; // по умолчанию, скрыто
            }
        }

        ClearCurrentVisibility();
        if (debug) Debug.Log("Текущая видимость очищена.");

        // Для каждого юнита из команды открываем область
        foreach (var unit in _teamManager.GetTeamByIndex(teamType))
        {
            if (unit == null)
            {
                if (debug) Debug.LogWarning("Пропущен null-юнит.");
                continue;
            }

            if (debug) Debug.Log($"Обработка юнита {unit.name} с радиусом обзора {unit.sightRange}");
            Vector3Int unitTilePos = mainTilemap.WorldToCell(unit.transform.position);
            unitTilePos.z = 0;
            RevealArea(unitTilePos, unit.sightRange, fogGrid, exploredGrid);
        }

        if (debug) Debug.Log($"Туман для команды {teamType} обновлен. Теперь обновляем врагов...");

        // Обновление видимости врагов для других команд
        foreach (TeamManager.TeamType team in System.Enum.GetValues(typeof(TeamManager.TeamType)))
        {
            if (team != teamType)
            {
                if (debug) Debug.Log($"Обновление видимости для команды: {team}");
                UpdateEnemyVisibility(_teamManager.GetTeamByIndex(team));
            }
        }

        if (debug) Debug.Log($"Обновление тумана завершено для команды: {teamType}");
    }

    void ClearCurrentVisibility()
    {
        currentVisibleTiles.Clear();
    }

    void RevealArea(Vector3Int unitPos, int range, bool[,] fogGrid, bool[,] exploredGrid)
    {
        Vector3Int size = mainTilemap.cellBounds.size;

        if (unitPos == null)
        {
            Debug.LogError($"Юнит не находится на карте!");
            return;
        }

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                {
                    Vector3Int tilePos = unitPos + new Vector3Int(dx, dy, 0);
                    int indexX = tilePos.x - origin.x;
                    int indexY = tilePos.y - origin.y;
                    if (indexX < 0 || indexY < 0 || indexX >= size.x || indexY >= size.y)
                    {
                        Debug.LogWarning($"Координаты {tilePos} выходят за пределы карты!");
                        continue;
                    }

                    // Обновляем fogGrid и exploredGrid
                    fogGrid[indexX, indexY] = false;
                    exploredGrid[indexX, indexY] = true;

                    // Запоминаем, что эта клетка сейчас видна
                    currentVisibleTiles.Add(tilePos);

                    // Убираем туман с клетки
                    fogTilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    public void UpdateEnemyVisibility(List<Unit> enemyUnits)
    {
        foreach (var enemy in enemyUnits)
        {
            if (enemy != null)
            {
                Vector3Int enemyTilePos = mainTilemap.WorldToCell(enemy.transform.position);
                enemyTilePos.z = 0;

                // Проверяем, находится ли позиция врага в текущем обзоре
                if (currentVisibleTiles.Contains(enemyTilePos))
                {
                    enemy.SwitchVisibility(true);
                    Debug.Log($"Враг {enemy.name} виден на позиции {enemyTilePos}");
                }
                else
                {
                    enemy.SwitchVisibility(false);
                    Debug.Log($"Враг {enemy.name} скрыт на позиции {enemyTilePos}");
                }
            }
        }
    }
}
