using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MovementController : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Links")]
    [SerializeField] private Grid grid; 
    [SerializeField] private Tilemap tilemap; 
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private ReachableTilesController _reachableTiles; 
    [SerializeField] private HighlightAreaController _highlightArea;
    [SerializeField] private FogManager _fogManager;

    public void Boot(
        FogManager fogManager,
        MapManager mapManager,
        TileManager tileManager,
        ReachableTilesController reachableTiles,
        HighlightAreaController highlightArea)
    {
        _fogManager = fogManager;
        _reachableTiles = reachableTiles;
        
        _highlightArea = highlightArea;
        _mapManager = mapManager;
        _tileManager = tileManager;
    }

    public override void Boot()
    {
        tilemap = _mapManager.mainTilemap;
        grid = _mapManager.grid;
    }

    public void Boot(SelectionController selectionController)
    {
        _selectionController = selectionController;
        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    // Метод перемещения объекта
    public void MoveObject(GameObject obj, Vector3 mouseWorldPosition, int unitSpeed, int unitActionPoint)
    {
        // Преобразование координат
        Vector3Int startTile = _mapManager.GetTilePositionFromWorldPosition(obj.transform.position);
        Vector3Int targetTile = _mapManager.GetTilePositionFromWorldPosition(mouseWorldPosition); 
        Vector3 targetCenter = _mapManager.GetTileCenterWorld(targetTile); // Центр клетки

        List<Vector3Int> reachebleTiles = _reachableTiles.GetReachableTilesForMoving(startTile, unitSpeed * unitActionPoint);

        // Проверка досягаемости
        if (!reachebleTiles.Contains(targetTile))
        {
            Debug.Log("Клетка назначения находится вне достижимой области!");
            ClearHighlight();
            return;
        }

        MoveToTile(obj, targetCenter); // Перемещение unit-а

        // Обработка очков действия и тумана войны
        ProcessMove(obj, startTile, targetTile, unitSpeed);

        ClearHighlight();

        _tileManager.SetTileOccupied(startTile, false);
        _tileManager.SetTileOccupied(targetTile, true);
    }

    private void MoveToTile(GameObject obj, Vector3 targetCenter)
    {
        obj.transform.position = new Vector3(targetCenter.x, targetCenter.y, -1);
        if (debug) Debug.Log($"Объект {obj.name} перемещен в {targetCenter}");
    }

    private void ClearHighlight()
    {
        _highlightArea.ClearHighlight();
        if (debug) Debug.Log("Подсветка отключена");
    }

    private void ProcessMove(GameObject obj, Vector3Int startTile, Vector3Int targetTile, int unitSpeed)
    {
        Unit unit = obj.GetComponent<Unit>();
        if (unit == null) return;

        // Расчёт стоимости перемещения
        int tileDistance = Mathf.Abs(targetTile.x - startTile.x) + Mathf.Abs(targetTile.y - startTile.y);
        int cost = Mathf.CeilToInt((float)tileDistance / unitSpeed);

        // Списание очков действия
        unit.currentActionPoint -= cost;
        if (debug) Debug.Log($"Стоимость перемещения: {cost}. Осталось очков действия: {unit.currentActionPoint}");

        _selectionController.HandleCancel();

        // Обновление тумана войны
        _fogManager.UpdateFog(unit._teamType);
    } 
}
