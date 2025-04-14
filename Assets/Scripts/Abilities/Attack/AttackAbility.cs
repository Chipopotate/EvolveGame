using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackAbility", menuName = "Ability/Attack Ability")]
public class AttackAbility : Ability
{
    [Header("Attack Settings")]
    public int damage;
    public StatusEffect[] statusEffects; // Эффекты, которые накладываются на цель при атаке

    public override bool CanExecute(Unit caster, Entity target, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        // Проверяем, что цель является Unit и что у неё другая команда
        if (target is Unit targetUnit)
        {
            return base.CanExecute(caster, target, reachableTiles, highlightArea) &&
                   caster.TeamIndex != targetUnit.TeamIndex;
        }

        return base.CanExecute(caster, target, reachableTiles, highlightArea);
    }

    public override void Execute(Unit caster, Entity target, HighlightAreaController highlightArea)
    {
        base.Execute(caster, target, highlightArea);

        // Преобразуем цель в Unit, если она является юнитом
        if (target is Unit unit)
        {
            unit.TakeDamage(damage);
            Debug.Log($"Длина - {statusEffects.Length}");
            if (statusEffects != null)
            {
                foreach (var effect in statusEffects)
                {
                    unit.ApplyEffect(effect); // Применяем эффект к юниту
                }
            }
        }
    }
}