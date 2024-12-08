using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public PjBase target;
    public int duration;
    [HideInInspector]
    public bool untimed;
    public Stats statsToChange = new Stats();
    float spdThreshold = 15;
    public GameObject particleFx;
    float regen;
    public bool debuff;

    public virtual void NormalSetUp(PjBase user, PjBase target, StatsToBuff statsToChange, int duration, GameObject particleFx, bool isDebuff)
    {
        debuff = isDebuff;
        this.user = user;
        this.target = target;
        this.duration = duration;
        if (duration == 0)
        {
            untimed = true;
        }


        this.statsToChange.pot = user.CalculateControl(statsToChange.pot);
        this.statsToChange.res = user.CalculateControl(statsToChange.res);
        this.statsToChange.spd = user.CalculateControl(statsToChange.spd);

        if (isDebuff)
        {
            this.statsToChange.pot = -this.statsToChange.pot;
            this.statsToChange.res = -this.statsToChange.res;
            this.statsToChange.spd = -this.statsToChange.spd;
            this.statsToChange.movement = -this.statsToChange.movement;
        }


        target.stats.strength += this.statsToChange.pot;
        target.stats.sinergy += this.statsToChange.pot;
        target.stats.fRes += this.statsToChange.res;
        target.stats.mRes += this.statsToChange.res;
        target.stats.spd += this.statsToChange.spd;
        target.stats.movement += this.statsToChange.movement;


        if (particleFx)
        {
            this.particleFx = Instantiate(particleFx, target.transform);
        }

        target.UpdateUI();
    }

    public void Tick()
    {
        if (!untimed)
        {
        duration--;
            if(duration <= 0)
            {
               StartCoroutine(Die());
            }
        }
    }

    public virtual IEnumerator Die()
    {
        yield return null;
        if (particleFx)
        {
            Destroy(particleFx);
        }

        target.stats.strength -= statsToChange.pot;
        target.stats.sinergy -= statsToChange.pot;
        target.stats.spd -= statsToChange.spd;
        target.stats.fRes -= statsToChange.res;
        target.stats.mRes -= statsToChange.res;

        target.UpdateUI();

        user.buffList.Remove(this);
        Destroy(this);
    }
}
