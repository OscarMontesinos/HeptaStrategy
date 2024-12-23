using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarLight : Buff
{
    Zafir zafir;
    float regen;
    public void NormalSetUp(Zafir user, PjBase target, StatsToBuff statsToChange, int duration, GameObject particleFx, bool isDebuff)
    {
        zafir = user;
        if(target.team != user.team)
        {
            statsToChange.res = 0;
            regen = user.CalculateControl(statsToChange.regen);
            statsToChange.regen = 0;
            user.stats.regen += regen;
            user.GetHealed(user, regen, true);
            debuff = true;
            duration++;
        }
        else
        {
            statsToChange.regen = 0;
        }
        base.NormalSetUp(user, target, statsToChange, duration, particleFx, isDebuff);
        zafir.pList.Add(target);
        
    }

    public override IEnumerator Die()
    {
        zafir.pList.Remove(target);
        user.stats.regen -= regen;
        return base.Die();
    }
}
