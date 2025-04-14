using UnityEngine;

// Абстрактный класс, представляющий эффект статуса, который можно применять к объектам
public abstract class StatusEffect : ScriptableObject
{
    [Header("Base Settings")] 
    public string effectName; // Название эффекта
    public Sprite icon; // Иконка эффекта
    public bool applyImmediately = false; // Определяет, применяется ли эффект сразу или на следующем ходу
    public int baseDuration = 1; // Длительность эффекта в ходах
    public int maxStacks = 1; // Максимальное количество раз, когда эффект может быть наложен на один объект

    // Метод для создания экземпляра эффекта (может быть переопределён в дочерних классах)
    public virtual StatusEffectInstance CreateInstance(Entity target)
    {
        // Возвращает новый экземпляр эффекта, связанный с целью
        return new StatusEffectInstance(this, target);
    }

    // Абстрактные методы, которые должны быть реализованы в классах-наследниках:

    // Вызывается, когда эффект впервые применяется к цели
    public abstract void OnApply(StatusEffectInstance instance);

    // Вызывается каждый ход, пока эффект активно действует
    public abstract void OnTick(StatusEffectInstance instance);

    // Вызывается, когда эффект завершает своё действие или удаляется досрочно
    public abstract void OnRemove(StatusEffectInstance instance);
}
