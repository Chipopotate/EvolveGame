using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Reflection;

public class LevelUpMenu : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("UI Elements")]
    [SerializeField] private GameObject abilityButtonPrefab; // Префаб кнопки для способностей
    [SerializeField] private Transform buttonsParent; // Родительский объект для кнопок способностей
    [SerializeField] private TextMeshProUGUI titleText; // Текст заголовка меню

    [Header("Dependencies")]
    [SerializeField] private SelectionController _selectionController; 
    [SerializeField] private GameStateManager _gameStateManager;

    [SerializeField] private Monster _currentMonster; // Текущее "существо", которое получило уровень
    [SerializeField] private List<Ability> _availableAbilities; // Доступные способности для выбора

    public void Boot(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Boot(SelectionController selectionController)
    {
        _selectionController = selectionController;
    }

    // Метод для показа меню с доступными способностями
    public void Show(Monster monster, List<Ability> availableAbilities)
    {
        //_selectionController?.SetState(SelectionController.State.LevelUpMenuOpen); // Устанавливаем состояние контроллера выбора

        _gameStateManager.SetState(GameStateManager.GameState.GamePause); // Устанавливаем состояние игры на паузу

        _currentMonster = monster; // Устанавливаем текущее существо
        _availableAbilities = availableAbilities; // Устанавливаем доступные способности
        gameObject.SetActive(true); // Включаем меню
        UpdateUI(); // Обновляем интерфейс

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для обновления пользовательского интерфейса меню
    private void UpdateUI()
    {
        ClearButtons(); // Уничтожаем старые кнопки
        CreateButtons(); // Создаем новые кнопки

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для очистки старых кнопок
    private void ClearButtons()
    {
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject); // Уничтожаем каждую дочернюю кнопку
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для создания новых кнопок
    private void CreateButtons()
    {
        foreach (var ability in _availableAbilities)
        {
            GameObject btnGO = Instantiate(abilityButtonPrefab, buttonsParent); // Создаем кнопку
            Button button = btnGO.GetComponent<Button>(); // Получаем компонент кнопки

            // Настройка изображения и текста кнопки
            Image icon = btnGO.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI nameText = btnGO.transform.Find("ID").GetComponent<TextMeshProUGUI>();

            icon.sprite = ability.icon; // Устанавливаем иконку способности
            nameText.text = ability.abilityName; // Устанавливаем название способности

            // Добавляем обработчик нажатия на кнопку
            button.onClick.AddListener(() => OnAbilitySelected(ability));
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для обработки выбора способности
    private void OnAbilitySelected(Ability selectedAbility)
    {
        _currentMonster.AddAbility(selectedAbility); // Добавляем выбранную способность существу
        Hide(); // Скрываем меню

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод для скрытия меню
    public void Hide()
    {
        _selectionController.SetState(SelectionController.State.Idle); // Устанавливаем состояние контроллера выбора
        ClearButtons(); // Очищаем кнопки
        gameObject.SetActive(false); // Закрываем окно меню

        _gameStateManager.RestorePreviousState(); // Восстанавливаем предыдущее состояние игры

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }
}
