using UnityEngine;

[CreateAssetMenu(fileName = "FreezEffect", menuName = "Status Effects/FreezEffect")]
public class FreezEffect : StatusEffect
{
    public override void OnApply(StatusEffectInstance instance)
    {

    }

    public override void OnTick(StatusEffectInstance instance)
    {
        // Проверяем, что цель эффекта является объектом Unit
        if (instance.target is Unit unit)
        {
            unit.currentActionPoint = 0; // Сбрасываем очки действия юнита на 0, так как он оглушён
        }

        // Уменьшаем оставшуюся длительность эффекта
        instance.currentDuration--;
    }

    public override void OnRemove(StatusEffectInstance instance)
    {

    }
}
