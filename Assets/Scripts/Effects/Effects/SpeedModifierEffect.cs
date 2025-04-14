using UnityEngine;

[CreateAssetMenu(fileName = "SpeedModifierEfffect", menuName = "Status Effects/SpeedModifier")]
public class SpeedModifierEfffect : StatusEffect
{
    public int speedBoost = 1; // Значение, на которое увеличивается скорость цели при применении эффекта

    // Метод, вызываемый при применении эффекта к цели
    public override void OnApply(StatusEffectInstance instance)
    {
        // Проверяем, что цель эффекта является объектом типа Unit
        if (instance.target is Unit unit)
        {
            // Проверяем, что эффект ещё не был применён, используя кастомный флаг IsApplied
            if (!instance.IsApplied)
            {
                unit.currentSpeed += speedBoost; // Изменяем скорость цели, увеличивая её на значение speedBoost
                if (unit.currentSpeed == 0) unit.currentSpeed = 1; // Устанавливаем минимальную скорость, если она равна нулю

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
            // Проверяем, что эффект был ранее применён, чтобы вернуть скорость в нормальное состояние
            if (instance.IsApplied)
            {
                unit.currentSpeed = unit.baseSpeed; // Возвращаем скорость цели к её страндартному значению
            }
        }
    }
}
