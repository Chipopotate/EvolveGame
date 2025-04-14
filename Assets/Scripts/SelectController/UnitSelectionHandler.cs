using UnityEngine;

public class UnitSelectionHandler : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")] 
    [SerializeField] private UnitActionHandler _actionHandler;
    [SerializeField] private HighlightAreaController _highlightArea;
    [SerializeField] private AbilityHotbar _attackHotbar;

    [Header("Variables")]
    public Unit selectedUnit;

    public void Boot(UnitActionHandler actionHandler, HighlightAreaController highlightArea)
    {
        _actionHandler = actionHandler;
        _highlightArea = highlightArea;

        if (_attackHotbar == null) Debug.LogError("Отсутствует ссылка на компонент!", this);

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    // Метод для выделения unit-а
    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        if (debug) Debug.Log("Выбран объект - " + selectedUnit.transform.gameObject.name);

        unit.SetStatus(Unit.ObjectStatus.Selectable);

        _attackHotbar.SetupHotbar(unit);
        _highlightArea.HighlightMovementArea(unit.transform.position, unit.currentActionPoint, unit.currentSpeed);
    }

    // Метод снятия выделения с unit-а
    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            if (debug) Debug.Log("Убрано выделение с " + selectedUnit.name);

            if (selectedUnit.currentActionPoint > 0)
            {
                selectedUnit.SetStatus(Unit.ObjectStatus.Available);
            }
            else selectedUnit.SetStatus(Unit.ObjectStatus.Unavailable);
        }

        selectedUnit = null;
        _actionHandler.selectedAbility = null;

        _attackHotbar.Hide();
        _highlightArea.ClearHighlight();
    }
}