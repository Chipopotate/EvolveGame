using UnityEngine;

[CreateAssetMenu(fileName = "ActionPointModifierEfffect", menuName = "Status Effects/ActionPointModifier")]
public class ActionPointModifierEfffect : StatusEffect
{
    public int actionPointChange = 1; // Кол-во очков действия, на которое изменяется текущий запас очков действия цели при применении эффекта

    // Метод, вызываемый при применении эффекта к цели
    public override void OnApply(StatusEffectInstance instance)
    {
        // Проверяем, что цель эффекта является объектом типа Unit
        if (instance.target is Unit unit)
        {
            // Проверяем, что эффект ещё не был применён, используя кастомный флаг IsApplied
            if (!instance.IsApplied)
            {
                unit.currentActionPoint += actionPointChange; // Изменяем запас очков действия цели, увеличивая его на значение actionPointChange

                instance.IsApplied = true; // Устанавливаем флаг IsApplied, чтобы предотвратить повторное применение
            }
        }
    }

    // Метод, вызываемый на каждом тике эффекта
    public override void OnTick(StatusEffectInstance instance)
    {
        // Уменьшаем оставшуюся длительность эффекта
        instance.currentDuration--;
    }

    // Метод, вызываемый при удалении эффекта
    public override void OnRemove(StatusEffectInstance instance)
    {
        // Проверяем, что цель эффекта является объектом типа Unit
        if (instance.target is Unit unit)
        {
            // Проверяем, что эффект был ранее применён, чтобы вернуть в нормальное состояние
            if (instance.IsApplied)
            {
                unit.currentActionPoint = unit.baseActionPoint; // Возвращаем атаку цели к её страндартному значению
            }
        }
    }
}
