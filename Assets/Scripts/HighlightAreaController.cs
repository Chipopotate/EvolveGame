using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightAreaController : BaseManager
{
    [Header("Debug")] 
    [SerializeField] private bool debug = false;

    [Header("Links")]
    [SerializeField] private Grid grid; 
    [SerializeField] private Tilemap tilemap; 
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private ReachableTilesController _reachableTiles;

    [Header("Colors")]
    [SerializeField] private Color firstColor = Color.blue; // Цвет подсветки
    [SerializeField] private Color secondColor = Color.yellow; // для перемещения за более чем 1 очко
    [SerializeField] private Color defaultColor = Color.white; // Обычный цвет

    public List<Vector3Int> highlightedTiles = new List<Vector3Int>(); // Список подсвеченных клеток

    public void Boot(ReachableTilesController reachableTiles, MapManager mapManager)
    {
        _reachableTiles = reachableTiles;
        _mapManager = mapManager;
    }

    public override void Boot()
    {
        tilemap = _mapManager.mainTilemap;
        grid = _mapManager.grid;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    // Метод для подсветки области движения
    public void HighlightMovementArea(Vector3 unitPosition, int actionPoint, int unitSpeed)
    {
        if (debug) Debug.Log("Подсвечены доступные для перемещения tile-ы");
        if (debug) Debug.Log("unitPosition - " + unitPosition);

        Vector3Int unitTilePosition = grid.WorldToCell(unitPosition); unitTilePosition.z = 0;
        if (debug) Debug.Log("unitTilePosition - " + unitTilePosition);

        foreach (Vector3Int tilePos in _reachableTiles.GetReachableTilesForMoving(unitTilePosition, actionPoint * unitSpeed))
        {
            // Вычисляем манхэттенское расстояние
            int tileDistance = Mathf.Abs(tilePos.x - unitTilePosition.x) + Mathf.Abs(tilePos.y - unitTilePosition.y);
            // Вычисляем стоимость перемещения
            int cost = Mathf.CeilToInt((float)tileDistance / unitSpeed);

            // Выбираем цвет: если стоимость 1, то blue, иначе yellow
            Color tileColor = (cost <= 1) ? firstColor : secondColor;

            tilemap.SetTileFlags(tilePos, TileFlags.None);
            tilemap.SetColor(tilePos, tileColor);
            //Debug.Log($"Изменяем цвет tile {tilePos} на {tileColor}");
            highlightedTiles.Add(tilePos);
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для подсветки области атаки
    public void HighlightAttackArea(Vector3 unitPosition, AttackAbility attackAbility)
    {
        if (debug) Debug.Log("Подсвечены доступные для атаки tile-ы");
        if (debug) Debug.Log("unitPosition - " + unitPosition);

        Vector3Int unitTilePosition = grid.WorldToCell(unitPosition); unitTilePosition.z = 0;
        if (debug) Debug.Log("unitTilePosition - " + unitTilePosition);

        foreach (Vector3Int tilePos in _reachableTiles.GetReachableTilesForMoving(unitTilePosition, attackAbility.range))
        {
            
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    public void HighlightArea(Vector3 worldPosition, int radius)
    {
        HighlightArea(worldPosition, radius, Color.black);
    }

    public void HighlightArea(Vector3 worldPosition, int radius, Color color)
    {
        Vector3Int centerTile = grid.WorldToCell(worldPosition);
        centerTile.z = 0;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int tilePos = centerTile + offset;

                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                Debug.Log($"{tilemap.HasTile(tilePos)}");
                if (distance <= radius && tilemap.HasTile(tilePos)) // Проверяем наличие тайла
                {
                    tilemap.SetTileFlags(tilePos, TileFlags.None);
                    tilemap.SetColor(tilePos, color);
                    Color appliedColor = tilemap.GetColor(tilePos);
                    Debug.Log($"Цвет тайла {tilePos}: {appliedColor}");
                    highlightedTiles.Add(tilePos);
                }
            }
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Сбрасываем подсветку
    public void ClearHighlight()
    {
        foreach (Vector3Int tilePos in highlightedTiles)
        {
            tilemap.SetColor(tilePos, defaultColor); // Возвращаем цвет по умолчанию
            tilemap.SetTileFlags(tilePos, TileFlags.LockColor);
        }

        highlightedTiles.Clear();

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

}
