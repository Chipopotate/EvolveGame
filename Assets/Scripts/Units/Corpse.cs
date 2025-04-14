using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Entity
{
    public int XPreward;

    public void SetCorpseReward(int XP)
    {
        XPreward = XP;
    }
}
