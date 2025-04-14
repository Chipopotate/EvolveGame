using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

// Класс для отображения активных эффектов на панели (в UI) для конкретной сущности
public class StatusEffectsBar : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 0.5f, 0); // Смещение панели над сущностью
    [SerializeField] private GameObject effectIconPrefab; // Префаб иконки эффекта
    [SerializeField] private Transform iconsParent; // Родительский объект для размещения иконок

    // Словарь для хранения активных иконок эффектов (шаблон эффекта -> объект иконки)
    private Dictionary<StatusEffect, GameObject> activeIcons = new Dictionary<StatusEffect, GameObject>();
    private Transform target; // Целевой объект, к которому привязана панель

    // Инициализация панели для конкретной сущности
    public void Initialize(Entity entity)
    {
        target = entity.transform; // Устанавливаем цель как объект сущности
        transform.position = target.position + worldOffset; // Устанавливаем смещение панели над целью

        // Делаем панель дочерним объектом цели
        transform.SetParent(target);
        transform.localPosition = worldOffset; // Устанавливаем локальное смещение относительно цели
    }

    // Обновляем активные эффекты и отображаем их на панели
    public void UpdateEffects(List<StatusEffectInstance> effects)
    {
        // Получаем уникальные эффекты (шаблоны), которые активны на данный момент
        var uniqueEffects = effects.Select(e => e.template).Distinct().ToList();

        // Удаляем иконки для эффектов, которые больше не активны
        List<StatusEffect> keysToRemove = new List<StatusEffect>();
        foreach (var key in activeIcons.Keys)
        {
            // Если эффект больше не активен, удаляем соответствующую иконку
            if (!uniqueEffects.Contains(key))
            {
                Destroy(activeIcons[key]); // Уничтожаем объект иконки
                keysToRemove.Add(key); // Добавляем в список на удаление из словаря
            }
        }

        // Удаляем из словаря записи для неактивных эффектов
        foreach (var key in keysToRemove)
        {
            activeIcons.Remove(key);
        }

        // Добавляем новые иконки для эффектов, которые ещё не отображены
        foreach (var effectTemplate in uniqueEffects)
        {
            if (!activeIcons.ContainsKey(effectTemplate))
            {
                GameObject newIcon = Instantiate(effectIconPrefab, iconsParent); // Создаём новую иконку
                newIcon.GetComponent<Image>().sprite = effectTemplate.icon; // Устанавливаем изображение из шаблона эффекта
                activeIcons.Add(effectTemplate, newIcon); // Добавляем иконку в словарь
            }
        }
    }
}
