using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static GameStateManager;

public class InputHandler : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Dependencies")]
    [SerializeField] private GameStateManager _gameStateManager;

    public event System.Action<Vector2> OnLeftClick;
    public event System.Action OnSwap;
    public event System.Action OnCancel;

    [SerializeField] private LayerMask _interactableLayers;

    public void Boot(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    void Update()
    {
        if (_gameStateManager.CurrentGameState == GameStateManager.GameState.GamePause) return;
        HandleLeftClick();
        HandleSwap();
        HandleCancel();
    }

    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (debug) Debug.Log("Клик ПКМ");

            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckPosition(mousePos, out GameObject none);
            Debug.Log($"object - {none}");
            OnLeftClick?.Invoke(mousePos);
        }
    }

    private void HandleSwap()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (debug) Debug.Log("Нажата клавиша tab");
            OnSwap?.Invoke();
        }
    }

    private void HandleCancel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (debug) Debug.Log("Нажата клавиша esc");
            OnCancel?.Invoke();
        }
    }

    // Получение tag-а объекта расположенного в position
    public bool CheckTagAtPosition(Vector2 position, string tag, out GameObject hitObject)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            position,
            Vector2.zero,
            Mathf.Infinity,
            _interactableLayers
        );

        hitObject = hit.collider?.gameObject;
        return hitObject != null && hitObject.CompareTag(tag);
    }

    public bool CheckPosition(Vector2 position, out GameObject hitObject)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            position,
            Vector2.zero,
            Mathf.Infinity,
            _interactableLayers
        );

        hitObject = hit.collider?.gameObject;
        return hitObject != null;
    }
}