using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Status Effects/Poison")]
public class PoisonEffect : StatusEffect
{
    public int baseDamage = 1; // Базовый урон, который эффект наносит за тик на один стак

    public override void OnApply(StatusEffectInstance instance)
    {

    }

    public override void OnTick(StatusEffectInstance instance)
    {
        // Рассчитываем общий урон как произведение базового урона и количества стаков
        int totalDamage = baseDamage * instance.currentStacks;

        // Проверяем, является ли цель эффекта объектом типа Unit
        if (instance.target is Unit unit)
        {
            unit.TakeDamage(totalDamage); // Наносим рассчитанный урон цели
        }

        // Уменьшаем оставшуюся длительность эффекта
        instance.currentDuration--;
    }

    public override void OnRemove(StatusEffectInstance instance)
    {

    }
}
