using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deterioration : Buff
{
    Zayd zayd;
    public int lvl;
    float dot;
    public override void NormalSetUp(PjBase user, PjBase target, StatsToBuff statsToChange, int duration, GameObject particleFx, bool isDebuff)
    {
        Reset();
        debuff = isDebuff;
        this.user = user;
        zayd = user.GetComponent<Zayd>();
        this.target = target;
        this.duration = duration;
        if (duration == 0)
        {
            untimed = true;
        }


        this.statsToChange.pot = user.CalculateControl(statsToChange.pot);
        this.statsToChange.res = user.CalculateControl(statsToChange.res);
        this.statsToChange.spd = user.CalculateControl(statsToChange.spd);
        this.statsToChange.regen = user.CalculateControl(statsToChange.regen);

        if (isDebuff)
        {
            this.statsToChange.pot = -this.statsToChange.pot;
            this.statsToChange.res = -this.statsToChange.res;
            this.statsToChange.spd = -this.statsToChange.spd;
            this.statsToChange.movement = -this.statsToChange.movement;
            this.statsToChange.regen = -this.statsToChange.regen;
        }


        if (particleFx)
        {
            this.particleFx = Instantiate(particleFx, target.transform);
        }

        lvl++;

        if (lvl >= 1)
        {
            this.particleFx.transform.GetChild(0).gameObject.SetActive(true);
            target.stats.pot += this.statsToChange.pot;
        }
        if (lvl >= 2)
        {
            this.particleFx.transform.GetChild(1).gameObject.SetActive(true);
            dot = user.CalculateControl(zayd.pDmg);
        }
        if (lvl >= 3)
        {
            this.particleFx.transform.GetChild(2).gameObject.SetActive(true);
            target.stats.spd += this.statsToChange.spd;
        }
        if (lvl >= 4)
        {
            this.particleFx.transform.GetChild(3).gameObject.SetActive(true);
            target.stats.res += this.statsToChange.res;
        }
        if (lvl >= 5)
        {
            this.particleFx.transform.GetChild(4).gameObject.SetActive(true);
            dot *= zayd.pDmgMultiplier;
        }


        target.UpdateUI();
    }

    public override void Tick()
    {
        user.DealDmg(target,PjBase.DmgType.magical,dot);
        base.Tick();
    }

    public override void Reset()
    {
        if (lvl >= 1)
        {
            target.stats.pot -= statsToChange.pot;
        }
        if(lvl >= 3)
        {
            target.stats.spd -= statsToChange.spd;
        }
        if (lvl >= 4)
        {
            target.stats.res -= statsToChange.res;
        }
    }
}
