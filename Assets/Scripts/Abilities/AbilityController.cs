using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AbilityController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [SerializeField] private ReachableTilesController _reachableTiles;
    [SerializeField] private HighlightAreaController _highlightArea;

    [Header("Dependencies")]
    [SerializeField] private MapManager _mapManager;

    public void Boot(MapManager mapManager, SelectionController selectionController, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        selectionController.Boot(this);

        _reachableTiles = reachableTiles;
        _highlightArea = highlightArea;
        _mapManager = mapManager;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    public void ExecuteAbility(Unit caster, Ability ability, Entity target, Vector2 mousePosition)
    {
        if (ability.targetMode == AbilityTargetMode.Target)
        {
            if (!ability.CanExecute(caster, target, _reachableTiles, _highlightArea))
            {
                Debug.Log("Цель недоступна!");
                return;
            }

            ability.Execute(caster, target, _highlightArea);
        }
        else if (ability.targetMode == AbilityTargetMode.Tile)
        {
            Vector3Int gridPos = _mapManager.GetTilePositionFromWorldPosition(mousePosition); // или другая логика перевода
            Vector3 Pos = _mapManager.GetTileCenterWorld(gridPos);
            if (!ability.CanExecute(caster, gridPos, _reachableTiles, _highlightArea))
            {
                Debug.Log("Тайл недоступен!");
                return;
            }

            ability.Execute(caster, Pos, _highlightArea);
        }

        caster.currentActionPoint -= ability.staminaCost;
    }
}