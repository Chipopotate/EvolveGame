using UnityEngine;

[CreateAssetMenu(fileName = "EatingAbility", menuName = "Ability/Eating Ability")]
public class Eating : Ability
{
    public override bool CanExecute(Unit caster, Entity target, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        Debug.Log($"Eating!");
        Debug.Log($"{target is Corpse}");
        if (target is Corpse && base.CanExecute(caster, target, reachableTiles, highlightArea))
        {
            Debug.Log("Можно скушать");
            return true;
        }
        else 
        {
            Debug.Log("Нельзя скушать");
            return false;
        }
    }

    public override void Execute(Unit caster, Entity target, HighlightAreaController highlightArea)
    {
        if (target is Corpse corpse)
        {
            Debug.Log("еда!");
            if (caster is Monster monster)
            {
                Debug.Log("монстр!");
                monster.TakeXP(corpse.XPreward);
                Destroy(target.gameObject);
            }
        }
    }
}
