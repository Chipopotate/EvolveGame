using UnityEngine;

//Начальная загрузка
public class Bootstrap : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private EffectBarManager _effectManager;
    [SerializeField] private TeamManager _teamManager;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private FogManager _fogManager;
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private MapGenerationController _tilemapGenerator;
    [SerializeField] private ReachableTilesController _reachedleTiles;
    [SerializeField] private HighlightAreaController _highlightArea;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private AbilityController _abilityController;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private AbilityHotbar _abilityHotbar;
    [SerializeField] private TeamHotbar _teamHotbar;
    [SerializeField] private LevelUpMenu _levelMenu;
    [SerializeField] private CameraMove _cameraController;

    void Awake()
    {
        // Проверяем все зависимости
        if (!BaseManager.ValidateDependencies(this,
            _gameStateManager,
            _levelMenu,
            _effectManager,
            _teamManager,
            _mapManager,
            _tileManager,
            _turnManager,
            _tilemapGenerator,
            _fogManager,
            _reachedleTiles,
            _highlightArea,
            _movementController,
            _abilityController,
            _selectionController,
            _abilityHotbar,
            _teamHotbar,
            _cameraController
            ))
        {
            return; // Прекращаем инициализацию при ошибке
        }

        _gameStateManager.Boot();

        _levelMenu.Boot(_gameStateManager);

        _effectManager.Boot();

        // Инициализация
        _teamManager.Boot(_effectManager, _levelMenu);

        _mapManager.Boot();

        _tileManager.Boot(_mapManager);

        _tilemapGenerator.Boot(_mapManager, _teamManager, _tileManager);

        _fogManager.Boot(_teamManager, _mapManager); 
        _fogManager.Boot();
        
        _turnManager.Boot(_teamManager, _fogManager, _gameStateManager);
        //_turnManager.Boot();

        _reachedleTiles.Boot(_mapManager, _tileManager);
        _reachedleTiles.Boot();

        _highlightArea.Boot(_reachedleTiles, _mapManager);
        _highlightArea.Boot();

        _movementController.Boot(_fogManager, _mapManager, _tileManager, _reachedleTiles, _highlightArea);
        _movementController.Boot();

        _selectionController.Boot(_turnManager, _movementController, _highlightArea, _levelMenu, _gameStateManager, _reachedleTiles);

        _abilityController.Boot(_mapManager, _selectionController, _reachedleTiles, _highlightArea);

        _cameraController.Boot(_turnManager);

        _abilityHotbar.Boot(_selectionController);
        _teamHotbar.Boot(_turnManager, _selectionController, _cameraController);

        _turnManager.SkipTurn();
    }
}
