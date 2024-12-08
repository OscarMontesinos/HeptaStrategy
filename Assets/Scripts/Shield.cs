using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    public float shieldAmount;

    public void ShieldSetUp(PjBase user,PjBase target, float shieldAmount, int duration, GameObject particleFx)
    {
        this.user = user;
        this.target = target;
        ChangeShieldAmount(shieldAmount);
        this.duration = duration;
        if(duration == 0)
        {
            untimed = true;
        }
        this.particleFx = particleFx;
        target.UpdateUI();

    }
    public virtual void SetShieldAmount(float value)
    {
        target.stats.shield -= shieldAmount;
        shieldAmount = value;
        target.stats.shield += shieldAmount;
        target.UpdateUI();
    }

    public virtual float ChangeShieldAmount(float value)
    {
        if (value >= -shieldAmount)
        {
            shieldAmount += value;
            target.stats.shield += value;
            value = 0;
        }
        else
        {
            value += shieldAmount;
            shieldAmount = 0;
            target.stats.shield += value;
        }

        if(target.stats.shield < 0)
        {
            target.stats.shield = 0;
        }

        target.UpdateUI();
        return -value;
    }

    public override IEnumerator Die()
    {
        SetShieldAmount(0);
        return base.Die();
    }
}
