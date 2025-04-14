using UnityEngine;

[System.Serializable] // Атрибут позволяет сериализовать класс для отображения в инспекторе Unity
public class StatusEffectInstance
{
    public StatusEffect template; // Шаблон эффекта, используемый для хранения общей логики и параметров
    public Entity target; // Цель, на которую наложен эффект 
    public int currentDuration; // Оставшееся время действия эффекта 
    public int currentStacks; // Текущее количество стаков эффекта 
    public bool IsApplied = false; // Флаг, предотвращающий повторное применение эффекта без необходимости

    // Конструктор класса для инициализации экземпляра статуса
    public StatusEffectInstance(StatusEffect effect, Entity entity)
    {
        template = effect; // Сохраняем ссылку на шаблон эффекта (например, BleedingEffect)
        target = entity; // Определяем сущность, на которую применяется эффект
        currentDuration = effect.baseDuration; // Устанавливаем базовую длительность эффекта из его шаблона
        currentStacks = 1; // Начальное количество стаков эффекта 
    }
}
