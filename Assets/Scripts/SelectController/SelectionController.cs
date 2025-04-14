using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectionController : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private UnitActionHandler _actionHandler;
    [SerializeField] private UnitSelectionHandler _selectionHandler;
    [SerializeField] private AbilityController _abilityController;
    [SerializeField] private GameStateManager _gameStateManager;

    public enum State { Idle, UnitSelected, LevelUpMenuOpen }
    private State _currentState = State.Idle;

    public void Boot(TurnManager turnManager, 
        MovementController movementController, 
        HighlightAreaController highlightArea, 
        LevelUpMenu levelMenu, 
        GameStateManager gameStateManager, 
        ReachableTilesController reachableTilesController)
    {
        _turnManager = turnManager;

        _turnManager.Boot(this);

        movementController.Boot(this);

        _inputHandler = GetComponent<InputHandler>();
        _inputHandler.Boot(gameStateManager);

        _actionHandler = GetComponent<UnitActionHandler>();

        _selectionHandler = GetComponent<UnitSelectionHandler>();

        _actionHandler.Boot(_selectionHandler, highlightArea, movementController, this, reachableTilesController);

        _selectionHandler.Boot(_actionHandler, highlightArea);

        levelMenu.Boot(this);

        // Подписываемся на действия
        _inputHandler.OnLeftClick += HandleClick;
        _inputHandler.OnCancel += HandleCancel;
    }

    public void Boot(AbilityController abilityController)
    {
        _abilityController = abilityController;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    private void HandleClick(Vector2 mousePosition)
    {
        if (_currentState == State.LevelUpMenuOpen) return; // Блокировка ввода

        switch (_currentState)
        {
            case State.Idle:
                TrySelectUnit(mousePosition);
                break;
            case State.UnitSelected:
                HandleUnitAction(mousePosition);
                break;
        }
    }

    private void TrySelectUnit(Vector2 mousePosition)
    {
        // Проверка совподения tag-ов
        if (_inputHandler.CheckTagAtPosition(mousePosition, "Unit", out GameObject unitObject))
        {
            Unit unitComponent = unitObject.GetComponent<Unit>();
            if (unitComponent == null)
            {
                Debug.LogError("У объект " + unitObject.name + " отсутсвует ссылка на компонент!", this);
                return;
            }

            if (unitComponent.TeamIndex == _turnManager.СurrentTeamIndex)
            {
                if (unitComponent.currentActionPoint > 0)
                {
                    _selectionHandler.SelectUnit(unitComponent);
                    _currentState = State.UnitSelected;
                }
                else Debug.Log("У объекта отсутсвуют очки действия");
            }
            else Debug.Log("Нельзя выбрать unit-а другой команды!");
        }

        if (_inputHandler.CheckTagAtPosition(mousePosition, "Ground", out GameObject GroundObject)) Debug.Log("Клик по " + GroundObject.name);
    }

    private void HandleUnitAction(Vector2 mousePosition)
    {
        if (_inputHandler.CheckTagAtPosition(mousePosition, "Ground", out GameObject groundObject) && _actionHandler.selectedAbility == null)
        {
            _actionHandler.HandleMovement(mousePosition, _selectionHandler.selectedUnit);
            HandleCancel();
        }
        else if (_actionHandler.selectedAbility != null)
        {
            var ability = _actionHandler.selectedAbility;
            if (ability.targetMode == AbilityTargetMode.Tile)
            {
                _abilityController.ExecuteAbility(
                    _selectionHandler.selectedUnit,
                    ability,
                    null,
                    mousePosition
                );
                HandleCancel();
            }
            else if (_inputHandler.CheckPosition(mousePosition, out GameObject targetObject))
            {
                Entity target = targetObject.GetComponent<Entity>();
                _abilityController.ExecuteAbility(
                    _selectionHandler.selectedUnit,
                    ability,
                    target,
                    mousePosition
                );
                HandleCancel();
            }
        }
    }

    public void HandleCancel()
    {
        _selectionHandler.DeselectUnit();
        _currentState = State.Idle;
    }

    public void TrySelectUnit(Vector3 unitPosition)
    {
        HandleClick(unitPosition);
    }

    public void SetState(State newState)
    {
        _currentState = newState;
        Debug.Log($"currentS4tatet {newState}");
        if (_currentState == State.LevelUpMenuOpen)
        { 
            HandleCancel();
        }
    }
}

