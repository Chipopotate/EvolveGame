using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private TeamManager.TeamType _currentTeam;
    public int СurrentTeamIndex => (int)_currentTeam; // Индекс текущаей команды

    [Header("Dependencies")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private TeamManager _teamManager;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private FogManager _fogManager;
    [SerializeField] private TeamHotbar _teamHotBar;
    [SerializeField] private CameraMove _camera;

    public void Boot(TeamManager teamManager, FogManager fogManager, GameStateManager gameStateManager)
    {
        _teamManager = teamManager;
        ValidateComponent(_teamManager);
        _fogManager = fogManager;
        ValidateComponent(_fogManager);
        _gameStateManager = gameStateManager;
        ValidateAsset(_gameStateManager);
    }

    public void Boot(SelectionController selectionController)
    {
        _selectionController = selectionController;
    }

    public void Boot(CameraMove camera)
    {
        _camera = camera;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован!");
    }

    public void Boot(TeamHotbar teamHotbar)
    {
        _teamHotBar = teamHotbar;
        if (_teamManager == null) // Проверка: Существует ли ссылка на компонент
        {
            Debug.LogError("Отсутсвует ссылка на компонент!", this);
            return;
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    public void SetFogManager(FogManager fogManager)
    {
        _fogManager = fogManager;
        if (_teamManager == null) // Проверка: Существует ли ссылка на компонент
        {
            Debug.LogError("Отсутсвует ссылка на компонент!", this);
            return;
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод, вызываемый кнопкой "SkipTurn"
    public void SkipTurn()
    {
        if (_gameStateManager.CurrentGameState == GameStateManager.GameState.GamePause) return;

        EndTurn();  // Завершаем текущий ход

        // Переключаем команду по порядку
        _currentTeam = _teamManager.GetNextTeam(_currentTeam);

        StartTurn();  // Начинаем новый ход

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для начала нового хода
    private void StartTurn()
    {
        if (debug) Debug.Log($"Команда {_currentTeam} начинает свой ход.");

        // Определяем unit-ов активной команды
        List<Unit> activeTeam = _teamManager.GetTeamByIndex(_currentTeam);

        Debug.Log($"Длинна активной команды - {activeTeam.Count}");
        
        if (activeTeam.Count == 0)
        {
            SkipTurn();
            return;
        }

        Debug.Log($"Длинна активной команды - {activeTeam[0]}");

        // Проверка для нейтральной команды
        if (_currentTeam == TeamManager.TeamType.Neutrals)
        {
            //StartCoroutine(HandleNeutralTurn(activeTeam));
            SkipTurn();
            return;
        }

        _camera.CenterCameraOnObject(activeTeam[0].gameObject);

        // Обновляем hotbar активной команды
        _teamHotBar.UpdateUnitDisplay(activeTeam);

        // Выполняем действия для каждого unit-а команды
        foreach (Unit unit in activeTeam)
        {
            unit.SetStatus(Unit.ObjectStatus.Available);

            unit.SwitchVisibility(true); // Делаем видимым
            
            //if (!unit.HasEffect<StunEffect>())
            unit.ResetActionPoint(); // Обновляем стамину (actionPoint) для каждого unit-a активной команды

            unit.ProcessEffects(); // Делаем видимым
        }

        // Обновляем туман 
        _fogManager.UpdateFog(_currentTeam);

        // Выполняем действия для каждого unit-а для вражеской команды
        if (_currentTeam == TeamManager.TeamType.Monsters)
        {
            foreach (Unit unit in _teamManager.GetTeamByIndex(TeamManager.TeamType.Hunters))
            {
                unit.SetStatus(Unit.ObjectStatus.Enemy);
            }
        }

        // Выполняем действия для каждого unit-а для вражеской команды
        if (_currentTeam == TeamManager.TeamType.Hunters)
        {
            foreach (Unit unit in _teamManager.GetTeamByIndex(TeamManager.TeamType.Monsters))
            {
                unit.SetStatus(Unit.ObjectStatus.Enemy);
            }
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для завершения текущего хода
    private void EndTurn()
    {
        if (debug) Debug.Log($"Команда {_currentTeam} пропускает ход.");

        _selectionController.HandleCancel(); // Снимаем выделение

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }
}
