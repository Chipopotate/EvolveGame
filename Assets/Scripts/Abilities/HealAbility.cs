using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "HealAbility", menuName = "Ability/Heal Ability")]
public class HealAbility : Ability
{
    [Header("Heal Settings")]
    public int healAmount;

    public override bool CanExecute(Unit caster, Entity target, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        if (target is Unit targetUnit)
        {
            if (!base.CanExecute(caster, target, reachableTiles, highlightArea)) return false;
            return target != null &&
                   caster.TeamIndex == targetUnit.TeamIndex &&
                   reachableTiles.IsTargetInRange(caster.transform.position, target.transform.position, range);
        }
        else return false;
    }

    public override void Execute(Unit caster, Entity target, HighlightAreaController highlightArea)
    {
        if (target is Unit targetUnit)
        {
            caster.currentActionPoint -= staminaCost;
            targetUnit.Heal(healAmount);
        }
        // Ёффекты лечени€
    }
}