using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public PjBase target;
    public float duration;
    [HideInInspector]
    public bool untimed;
    public Stats statsToChange;
    float spdThreshold = 15;
    public GameObject particleFx;
    float regen;

    public virtual void NormalSetUp(PjBase user, PjBase target, Stats statsToChange,float duration, GameObject particleFx)
    {
        this.user = user;
        this.target = target;
        this.duration = duration;
        if (duration == 0)
        {
            untimed = true;
        }
        this.statsToChange = statsToChange;

        target.stats.strength += this.statsToChange.strength;
        target.stats.sinergy += this.statsToChange.sinergy;
        target.stats.control += this.statsToChange.control;
        target.stats.fDef += this.statsToChange.fDef;
        target.stats.mDef += this.statsToChange.mDef;

        if (this.statsToChange.spd > 0)
        {
            if (this.statsToChange.spd <= user.stats.control / spdThreshold)
            {
                target.stats.spd += this.statsToChange.spd;
            }
            else
            {
                this.statsToChange.spd = user.stats.control / spdThreshold;
                target.stats.spd += this.statsToChange.spd;
            }
        }
        else if (this.statsToChange.spd < 0)
        {
            if (this.statsToChange.spd <= user.stats.control / -spdThreshold)
            {
                target.stats.spd += this.statsToChange.spd;
            }
            else
            {
                this.statsToChange.spd = user.stats.control / -spdThreshold;
                target.stats.spd += this.statsToChange.spd;
            }
        }

        if (particleFx)
        {
            this.particleFx = Instantiate(particleFx, target.transform);
        }

    }

    public virtual void Die()
    {
        if (particleFx)
        {
            Destroy(particleFx);
        }

        target.stats.strength -= statsToChange.strength;
        target.stats.sinergy -= statsToChange.sinergy;
        target.stats.control -= statsToChange.control;
        target.stats.spd -= statsToChange.spd;
        target.stats.fDef -= statsToChange.fDef;
        target.stats.mDef -= statsToChange.mDef;
        Destroy(this);
    }
}
