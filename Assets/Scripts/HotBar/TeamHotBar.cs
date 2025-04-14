using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

public class TeamHotbar : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("UI Elements")]
    [SerializeField] private GameObject unitButtonPrefab;
    [SerializeField] private Transform buttonsParent;

    [Header("Settings")]
    [SerializeField] private int maxUnits = 10;
    [SerializeField] private Sprite defaultUnitIcon;

    [Header("Links")]
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private SelectionController _selectionController;
    [SerializeField] private CameraMove _camera;

    private List<Button> unitButtons = new List<Button>();
    private List<Image> unitIcons = new List<Image>();
    private List<TextMeshProUGUI> unitNames = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> unitLevels = new List<TextMeshProUGUI>();

    public void Boot(TurnManager turnManager, SelectionController selectionController, CameraMove camera)
    {
        _turnManager = turnManager;

        _turnManager.Boot(this);

        _selectionController = selectionController;

        _camera = camera;

        InitializePool();

        if (debug) Debug.Log(this.GetType().Name + " инициализирован!");
    }

    // Метод инициализации кнопок 
    private void InitializePool()
    {
        for (int i = 0; i < maxUnits; i++)
        {
            GameObject btnGO = Instantiate(unitButtonPrefab, buttonsParent);

            // Получаем компоненты
            Button btn = btnGO.GetComponent<Button>();
            unitButtons.Add(btn);

            Transform iconTransform = btnGO.transform.Find("Image");
            Transform nameTransform = btnGO.transform.Find("Name");
            Transform levelTransform = btnGO.transform.Find("Level");

            // Кэшируем ссылки
            unitIcons.Add(iconTransform?.GetComponent<Image>());
            unitNames.Add(nameTransform?.GetComponent<TextMeshProUGUI>());
            unitLevels.Add(levelTransform?.GetComponent<TextMeshProUGUI>());

            btnGO.SetActive(false);
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    public void UpdateUnitDisplay(List<Unit> activeUnits)
    {
        ClearUnits();

        // Обновляем кнопки в зависимости от активных юнитов
        for (int i = 0; i < activeUnits.Count && i < maxUnits; i++)
        {
            Unit unit = activeUnits[i];
            if (unit == null) continue;

            SetupButton(i, unit);
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    private void SetupButton(int index, Unit unit)
    {
        GameObject btnGO = unitButtons[index].gameObject;
        btnGO.SetActive(true);

        // Иконка
        if (unitIcons[index] != null)
        {
            unitIcons[index].sprite = unit.entityIcon ?? defaultUnitIcon;
            unitIcons[index].preserveAspect = true;
        }

        // Имя
        if (unitNames[index] != null)
            unitNames[index].text = unit.entityName;

        // Уровень
        if (unitLevels[index] != null && unit is Monster monster)
            unitLevels[index].text = $"Lv.{monster.LVL}";
        else if (unitLevels[index] != null)
            unitLevels[index].text = "";

        // Обработчик клика
        unitButtons[index].onClick.RemoveAllListeners();
        unitButtons[index].onClick.AddListener(() =>
        {
            if (unit.currentActionPoint > 0)
            {
                _selectionController.HandleCancel();
                _selectionController.TrySelectUnit(unit.transform.position);
            }
            else Debug.Log("Unit has no action points!");

            _camera.CenterCameraOnObject(unit.gameObject);
        });

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    private void ClearUnits()
    {
        foreach (var btn in unitButtons)
            btn?.gameObject.SetActive(false);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
