using System.Collections.Generic;
using UnityEngine;

// Класс для управления эффектами, применяемыми к сущности 
public class EntityEffectManager
{
    // Свойство со списком активных эффектов с доступом только для чтения извне
    public List<StatusEffectInstance> activeEffects { get; private set; }

    // Ссылка на владельца эффектов (например, игрок или враг)
    private Entity _owner;

    // Событие, уведомляющее о любых изменениях эффектов на сущности
    public event System.Action<Entity> OnEffectsChanged;

    // Конструктор: инициализирует менеджер эффектов и привязывает его к сущности-владельцу
    public EntityEffectManager(Entity owner)
    {
        activeEffects = new List<StatusEffectInstance>();
        this._owner = owner;
    }

    // Метод для применения нового эффекта к сущности
    public void ApplyEffect(StatusEffect effect, Entity entity)
    {
        // Проверяем, существует ли уже эффект такого типа
        StatusEffectInstance existing = activeEffects.Find(e => e.template == effect);

        if (existing != null) // Если эффект уже активен
        {
            // Если эффект стекается, добавляем один стак (до максимального количества)
            if (existing.currentStacks <= effect.maxStacks)
            {
                existing.currentStacks = Mathf.Min(existing.currentStacks + 1, effect.maxStacks);
            }

            // Обновляем длительность существующего эффекта
            existing.currentDuration = effect.baseDuration;
        }
        else // Если эффект ещё не активен, создаём новый экземпляр и применяем его
        {
            StatusEffectInstance newInstance = effect.CreateInstance(entity);
            activeEffects.Add(newInstance); // Добавляем эффект в список активных
            effect.OnApply(newInstance); // Вызываем метод применения эффекта
        }

        // Уведомляем слушателей об изменении эффектов
        OnEffectsChanged?.Invoke(_owner);
    }

    // Метод для обработки всех активных эффектов на ход
    public void ProcessEffects()
    {
        bool changed = false; // Флаг, указывающий, были ли изменения в списке эффектов

        // Проходим по списку эффектов в обратном порядке (для безопасного удаления)
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectInstance instance = activeEffects[i];

            // Вызываем метод "OnTick" для текущего эффекта
            instance.template.OnTick(instance);

            // Проверяем, истекла ли длительность эффекта
            if (instance.currentDuration <= 0)
            {
                // Удаляем эффект, вызывая "OnRemove", и убираем его из списка
                instance.template.OnRemove(instance);
                activeEffects.RemoveAt(i);
                changed = true;
            }
        }

        // Если были изменения, уведомляем об этом
        if (changed)
        {
            OnEffectsChanged?.Invoke(_owner);
        }
    }

    // Метод для удаления всех активных эффектов
    public void ClearAllEffects()
    {
        // Вызываем "OnRemove" для каждого активного эффекта
        foreach (var effect in activeEffects)
        {
            effect.template.OnRemove(effect);
        }

        // Очищаем список эффектов
        activeEffects.Clear();

        // Уведомляем слушателей об изменениях
        OnEffectsChanged?.Invoke(_owner);
    }
}
