using UnityEngine;

[CreateAssetMenu(fileName = "JumpAbility", menuName = "Ability/Jump Ability")]

public class JumpAbility : Ability
{
    [Header("Attack Settings")]
    public int jumpRange;

    public override bool CanExecute(Unit caster, Vector3Int position, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        /*if (position in reachableTiles.GetReachableTilesForMoving(caster.transform.position, range, true))
        {
            return true;
        }*/
        return base.CanExecute(caster, position, reachableTiles, highlightArea);
    }

    public override void Execute(Unit caster, Vector3 position, HighlightAreaController highlightArea)
    {
        caster.transform.position = position;   
    }
}
