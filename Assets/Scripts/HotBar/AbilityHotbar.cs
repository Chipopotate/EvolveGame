using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AbilityHotbar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    private const int MAX_ATTACKS = 10; // Максимальное количество атак
    private Button[] attackButtons = new Button[MAX_ATTACKS];
    private Image[] attackImages = new Image[MAX_ATTACKS];

    [Header("Links")]
    [SerializeField] SelectionController _selectionController;
    [SerializeField] UnitActionHandler _actionHandler;

    public void Boot(SelectionController selectionController)
    {
        _selectionController = selectionController;

        _actionHandler = _selectionController?.GetComponent<UnitActionHandler>();

        // Создаем кнопки заранее
        for (int i = 0; i < MAX_ATTACKS; i++)
        {
            GameObject btnGO = Instantiate(buttonPrefab, buttonsParent);
            attackButtons[i] = btnGO.GetComponent<Button>();

            // Ищем дочерний объект "Image" и сохраняем его ссылку
            Transform imageTransform = btnGO.transform.Find("Image");
            if (imageTransform != null)attackImages[i] = imageTransform.GetComponent<Image>();
            else Debug.LogError($"Не найден 'Image' в кнопке {btnGO.name}");

            Transform IDTransform = btnGO.transform.Find("ID");
            if (IDTransform != null)
            {
                TextMeshProUGUI text = IDTransform.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = $"{i}";
                }
                else
                {
                    Debug.LogError($"Компонент TextMeshPro не найден на объекте 'ID' в кнопке {btnGO.name}");
                }
            }
            else
            {
                Debug.LogError($"Не найден объект 'ID' в кнопке {btnGO.name}");
            }

            btnGO.SetActive(false); // Пока скрываем кнопки
        }
    }


    // Очищает hotbar (теперь просто скрывает кнопки)
    public void ClearHotbar()
    {
        foreach (var btn in attackButtons) // Перебираем все кнопки
        {
            btn.gameObject.SetActive(false); // Отключаем кнопку
            btn.onClick.RemoveAllListeners(); // Удаляем старые события
        }
    }

    // Настраивает hotbar, активируя нужные кнопки и задавая им параметры
    public void SetupHotbar(Unit unit)
    {
        // Деактивируем все кнопки
        foreach (var btn in attackButtons) btn.gameObject.SetActive(false);

        // Активируем нужное количество кнопок и настраиваем их
        for (int i = 0; i < unit.abilities.Length && i < MAX_ATTACKS; i++)
        {
            Ability ability = unit.abilities[i];

            attackButtons[i].gameObject.SetActive(true);
            attackImages[i].sprite = unit.abilities[i].icon; // Назначаем картинку

            int index = i; // Костыль для лямбды
            attackButtons[i].onClick.RemoveAllListeners();
            attackButtons[i].onClick.AddListener(() =>
            {
                _actionHandler?.SelectAbility(ability);
            });
        }

        gameObject.SetActive(true);
    }

    // Скрывает hotbar
    public void Hide()
    {
        gameObject.SetActive(false);
        ClearHotbar();
    }
}
