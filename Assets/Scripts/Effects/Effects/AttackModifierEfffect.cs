using UnityEngine;

[CreateAssetMenu(fileName = "AttackModifierEfffect", menuName = "Status Effects/AttackModifier")]
public class AttackModifierEfffect : StatusEffect
{
    public int attackChange = 1; // Значение, на которое увеличивается атака цели при применении эффекта

    // Метод, вызываемый при применении эффекта к цели
    public override void OnApply(StatusEffectInstance instance)
    {
        // Проверяем, что цель эффекта является объектом типа Unit
        if (instance.target is Unit unit)
        {
            // Проверяем, что эффект ещё не был применён, используя кастомный флаг IsApplied
            if (!instance.IsApplied)
            {
                unit.attackBoost += attackChange; // Изменяем атаку цели, увеличивая её на значение attackBoost

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
                unit.attackBoost = 0; // Возвращаем атаку цели к её страндартному значению
            }
        }
    }
}
