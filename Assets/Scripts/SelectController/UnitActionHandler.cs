using UnityEditor;
using UnityEngine;

public class UnitActionHandler : BaseManager
{
    [Header("Debug")] 
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private HighlightAreaController _highlightArea;
    [SerializeField] private ReachableTilesController _reachableTiles;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private UnitSelectionHandler _selectionHandler;

    public Ability selectedAbility;

    public void Boot(UnitSelectionHandler selectionHandler, HighlightAreaController highlightArea, MovementController movementController, SelectionController selectionController, ReachableTilesController reachableTiles)
    {
        _highlightArea = highlightArea;
        _reachableTiles = reachableTiles;
        _movementController = movementController;
        _selectionController = selectionController;
        _selectionHandler = selectionHandler;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    public void HandleMovement(Vector3 targetPosition, Unit selectedUnit)
    {
        if (selectedUnit == null) return;

        Debug.Log("HandleMovement");
        Debug.Log($"range - {selectedUnit.currentSpeed * selectedUnit.baseActionPoint}", selectedUnit);

        GameObject unitObj = selectedUnit.transform.gameObject;
        _movementController.MoveObject(unitObj, targetPosition, selectedUnit.currentSpeed, selectedUnit.currentActionPoint);
        _selectionController.HandleCancel();
    }

    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
        _highlightArea.ClearHighlight();
        ability.PreviewArea(_selectionHandler.selectedUnit, _highlightArea, _reachableTiles);
        if (debug) Debug.Log("Выбрана атака: " + ability.abilityName);
    }
}
