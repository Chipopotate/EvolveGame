using System.Collections.Generic;
using UnityEngine;

// Менеджер для отображения панелей эффектов на сущностях
public class EffectBarManager : BaseManager
{
    [SerializeField] private GameObject effectBarPrefab; // Префаб панели эффектов (Effect Bar) для отображения в UI
    private List<StatusEffectInstance> activeEffects; // Список активных эффектов для текущей сущности

    // Словарь для хранения панелей эффектов, привязанных к сущностям
    private Dictionary<Entity, StatusEffectsBar> activeBars = new Dictionary<Entity, StatusEffectsBar>();

    // Метод для регистрации новой сущности, чтобы отслеживать её эффекты
    public void RegisterEntity(Entity entity)
    {
        // Подписываемся на событие изменения эффектов у сущности
        entity.effectManager.OnEffectsChanged += OnEntityEffectsChanged;

        // Ручное обновление панели эффектов сразу после регистрации
        OnEntityEffectsChanged(entity);
    }

    // Метод, вызываемый при изменении эффектов на сущности
    private void OnEntityEffectsChanged(Entity entity)
    {
        // Получаем список активных эффектов у сущности
        activeEffects = entity.effectManager.activeEffects;
        bool hasEffects = activeEffects.Count > 0; // Проверяем, есть ли активные эффекты

        // Если есть эффекты, но панель ещё не создана, создаём новую
        if (hasEffects && !activeBars.ContainsKey(entity))
        {
            CreateNewBar(entity);
        }

        // Если эффектов нет, но панель существует, удаляем её
        else if (!hasEffects && activeBars.ContainsKey(entity))
        {
            RemoveBar(entity);
        }

        // Если панель уже существует, обновляем её с текущими эффектами
        if (activeBars.TryGetValue(entity, out StatusEffectsBar bar))
        {
            bar.UpdateEffects(activeEffects); // Обновляем эффекты в панели
        }
    }

    // Метод для создания новой панели эффектов для сущности
    private void CreateNewBar(Entity entity)
    {
        GameObject newBar = Instantiate(effectBarPrefab); // Создаём экземпляр панели
        StatusEffectsBar barComponent = newBar.GetComponent<StatusEffectsBar>(); // Получаем компонент StatusEffectsBar
        barComponent.Initialize(entity); // Инициализируем панель для сущности
        Debug.Log("New bar created for " + entity.entityName, entity); // Логируем создание панели
        activeBars.Add(entity, barComponent); // Добавляем панель в словарь
    }

    // Метод для удаления панели эффектов сущности
    private void RemoveBar(Entity entity)
    {
        // Проверяем, есть ли панель у данной сущности
        if (activeBars.TryGetValue(entity, out StatusEffectsBar bar))
        {
            Destroy(bar.gameObject); // Удаляем объект панели
            activeBars.Remove(entity); // Убираем запись из словаря
        }
    }
}
