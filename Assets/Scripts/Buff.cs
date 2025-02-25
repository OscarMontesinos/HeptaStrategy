using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
        this.statsToChange.mRes = user.CalculateControl(statsToChange.mRes);
        this.statsToChange.fRes = user.CalculateControl(statsToChange.fRes);
        this.statsToChange.spd = user.CalculateControl(statsToChange.spd);
        this.statsToChange.regen = user.CalculateControl(statsToChange.regen);
        this.statsToChange.movement = statsToChange.movement;
        if (target.turno)
        {
            target.GetHealed(target, this.statsToChange.regen, true);
        }

        if (isDebuff)
        {
            this.statsToChange.pot = -this.statsToChange.pot;
            this.statsToChange.res = -this.statsToChange.res;
            this.statsToChange.mRes = -this.statsToChange.mRes;
            this.statsToChange.fRes = -this.statsToChange.fRes;
            this.statsToChange.spd = -this.statsToChange.spd;
            this.statsToChange.movement = -this.statsToChange.movement;
            this.statsToChange.regen = -this.statsToChange.regen;
        }
        target.stats.pot += this.statsToChange.pot;
        target.stats.res += this.statsToChange.res;
        target.stats.mRes += this.statsToChange.mRes;
        target.stats.fRes += this.statsToChange.fRes;
        target.stats.spd += this.statsToChange.spd;
        target.stats.movement += this.statsToChange.movement;
        target.stats.regen += this.statsToChange.regen;


        if (particleFx)
        {
            this.particleFx = Instantiate(particleFx, target.transform);
        }

        target.UpdateUI();
    }

    public virtual void Tick()
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
    public virtual void Reset()
    {
        target.stats.pot -= statsToChange.pot;
        target.stats.res -= statsToChange.res;
        target.stats.strength -= statsToChange.pot;
        target.stats.sinergy -= statsToChange.pot;
        target.stats.spd -= statsToChange.spd;
        target.stats.fRes -= statsToChange.res;
        target.stats.fRes -= statsToChange.fRes;
        target.stats.mRes -= statsToChange.res;
        target.stats.mRes -= statsToChange.mRes;
        target.stats.movement -= statsToChange.movement;
        target.stats.regen -= statsToChange.regen;
    }

    public virtual IEnumerator Die()
    {
        yield return null;
        if (particleFx)
        {
            Destroy(particleFx);
        }

        Reset();

        target.UpdateUI();

        target.buffList.Remove(this);
        Destroy(this);
    }
}
