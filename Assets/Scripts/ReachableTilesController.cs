using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ReachableTilesController : BaseManager
{
    [Header("Debug")] 
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private Grid grid; 
    [SerializeField] private Tilemap tilemap; 
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileManager _tileManager;
    
    private static readonly Vector3Int[] directions =
    { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    [SerializeField] private TilePropertiesCollection _tilePropertiesCollection; // Коллекция свойств тайлов

    public void Boot(MapManager mapManager, TileManager tileManager)
    {
        _mapManager = mapManager;
        _tileManager = tileManager;
    }

    public override void Boot()
    {
        grid = _mapManager.grid;
        tilemap = _mapManager.mainTilemap;

        _tilePropertiesCollection = _mapManager.tilePropertiesCollection;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    /// <summary>
    /// Рассчитывает доступные клетки (тайлы) на сетке для перемещения юнита 
    /// исходя из его текущей позиции и радиуса перемещения.
    /// </summary>
    /// <param name="unitPosition">Текущая позиция юнита на сетке.</param>
    /// <param name="radius">Максимальный радиус перемещения в клетках.</param>
    /// <returns>Список клеток (Vector3Int), доступных для перемещения.</returns>
    /// <remarks>
    /// Используется алгоритм поиска в ширину (BFS), чтобы пройти по клеткам.
    /// Проверяются проходимость клеток и их доступность в пределах указанного радиуса.
    /// </remarks>
    public List<Vector3Int> GetReachableTilesForMoving(Vector3Int unitPosition, int radius, bool ignorCost = false)
    {
        List<Vector3Int> reachablePositions = new List<Vector3Int>();
        Queue<Vector3Int> bfsQueue = new Queue<Vector3Int>(); // Очередь для BFS
        Dictionary<Vector3Int, int> tileCosts = new Dictionary<Vector3Int, int>(); // Стоимость достижения клетки

        // Начальная клетка
        bfsQueue.Enqueue(unitPosition);
        tileCosts[unitPosition] = 0;

        // Устанавливаем тайл как временно не занят
        _tileManager.SetTileOccupied(unitPosition, false);

        while (bfsQueue.Count > 0)
        {
            Vector3Int current = bfsQueue.Dequeue();
            int currentCost = tileCosts[current];

            // Проверяем соседей (в 4 направлениях)
            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = current + direction;

                // Проверяем, есть ли тайл на клетке
                if (!IsTileReachable(neighbor))
                {
                    //Debug.Log($"Клетка {neighbor} недостижима.");
                    continue;
                }

                TileBase tile = tilemap.GetTile(neighbor);
                if (tile == null)
                {
                    Debug.LogWarning($"Клетка {neighbor} не имеет тайла.");
                    continue;
                }

                int tileMovementCost = _tilePropertiesCollection.GetTileCost(tile);

                int newCost;

                if (!ignorCost)
                {
                    newCost = currentCost + tileMovementCost; // Увеличиваем стоимость
                }
                else newCost = 1;

                int existingCost;
                if (newCost <= radius && (!tileCosts.TryGetValue(neighbor, out existingCost) || newCost < existingCost))
                {
                    tileCosts[neighbor] = newCost;
                    bfsQueue.Enqueue(neighbor);
                    reachablePositions.Add(neighbor);
                }
            }
        }

        _tileManager.SetTileOccupied(unitPosition, true);

        return reachablePositions;
    }

    /// <summary>
    /// Проверяет, доступна ли указанная клетка для перемещения.
    /// </summary>
    /// <param name="position">Координаты клетки на сетке (Vector3Int).</param>
    /// <returns>
    /// Возвращает true, если клетка существует на tilemap и является проходимой, 
    /// иначе возвращает false.
    /// </returns>
    /// <remarks>
    /// Клетка считается доступной, если она существует на tilemap и её свойства 
    /// позволяют перемещение (проходимость тайла проверяется через TilePropertiesCollection).
    /// </remarks>
        // Обновленный метод проверки достижимости
    private bool IsTileReachable(Vector3Int position)
    {
        if (!tilemap.HasTile(position))
        {
            return false;
        }

        // Проверяем, является ли тайл проходимым
        bool walkable = IsTileWalkable(position);
        return walkable;
    }

    public bool IsTileWalkable(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);

        if (tile == null)
        {
            return false;
        }

        TileProperties properties = _tilePropertiesCollection.GetTileProperties(tile);
        if (properties == null)
        {
            return false;
        }

        if (!properties.isWalkable)
        {
        }

        if (_tileManager.IsTileOccupied(position))
        {
        }

        return properties != null &&
               properties.isWalkable &&
               !_tileManager.IsTileOccupied(position);
    }

    /// <summary>
    /// Проверяет, находится ли цель (target) в пределах указанного диапазона (range) от юнита (caster).
    /// </summary>
    /// <param name="caster">Unit, от которого измеряется расстояние.</param>
    /// <param name="target">Целевой unit, до которого измеряется расстояние.</param>
    /// <param name="range">Максимальная допустимая дистанция в клетках.</param>
    /// <returns>Возвращает true, если цель находится в пределах диапазона, иначе false.</returns>
    public bool IsTargetInRange(Vector3 casterPosition, Vector3 targetPosition, int range)
    {
        Debug.Log($"caster - {casterPosition}, target{targetPosition}");
        Vector3Int casterPos = grid.WorldToCell(casterPosition);
        Vector3Int targetPos = grid.WorldToCell(targetPosition);
        int distance = Mathf.Abs(targetPos.x - casterPos.x) + Mathf.Abs(targetPos.y - casterPos.y);
        return distance <= range;
    }

}
