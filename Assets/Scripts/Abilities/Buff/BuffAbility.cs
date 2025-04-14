using UnityEngine;

[CreateAssetMenu(fileName = "NewBuffAbility", menuName = "Ability/Buff Ability")]
public class BuffAbility : Ability
{
    [Header("Buff Settings")]
    public StatusEffect[] buffEffects;

    public override bool CanExecute(Unit caster, Entity target, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        // Проверяем, что цель является Unit и что у неё другая команда
        if (target is Unit targetUnit)
        {
            return base.CanExecute(caster, target, reachableTiles, highlightArea) &&
               target != null &&
               caster.TeamIndex == targetUnit.TeamIndex;
        }
        else return false;
    }

    public override void Execute(Unit caster, Entity target, HighlightAreaController highlightArea)
    {
        caster.currentActionPoint -= staminaCost;

        foreach (var effect in buffEffects)
        {
            target.ApplyEffect(effect);
        }

        Debug.Log($"{caster.name} применил {abilityName} на {target.name} и усилил его! ✨");
    }
}
