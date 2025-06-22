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

    private void Update()
    {
        if (!user)
        {
            StartCoroutine(Die());
        }
    }

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
        this.statsToChange.extraMRes = user.CalculateControl(statsToChange.mRes);
        this.statsToChange.extraFRes = user.CalculateControl(statsToChange.fRes);
        this.statsToChange.extraSpd = user.CalculateControl(statsToChange.spd);
        this.statsToChange.regen = user.CalculateControl(statsToChange.regen);
        this.statsToChange.extraCon = user.CalculateControl(statsToChange.con);
        this.statsToChange.movement = statsToChange.movement;
        if (target.turno)
        {
            target.GetHealed(target, this.statsToChange.regen, true);
        }

        if (isDebuff)
        {
            this.statsToChange.pot = -this.statsToChange.pot;
            this.statsToChange.extraCon = -this.statsToChange.extraCon;
            this.statsToChange.res = -this.statsToChange.res;
            this.statsToChange.extraMRes = -this.statsToChange.mRes;
            this.statsToChange.extraFRes = -this.statsToChange.fRes;
            this.statsToChange.extraSpd = -this.statsToChange.spd;
            this.statsToChange.movement = -this.statsToChange.movement;
            this.statsToChange.regen = -this.statsToChange.regen;
        }
        target.stats.pot += this.statsToChange.pot;
        target.stats.extraCon += this.statsToChange.extraCon;
        target.stats.res += this.statsToChange.res;
        target.stats.extraMRes += this.statsToChange.extraMRes;
        target.stats.extraFRes += this.statsToChange.extraFRes;
        target.stats.extraSpd += this.statsToChange.extraSpd;
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
        target.stats.extraCon -= statsToChange.extraCon;
        target.stats.res -= statsToChange.res;
        target.stats.extraFRes -= statsToChange.extraFRes;
        target.stats.extraMRes -= statsToChange.extraMRes;
        target.stats.extraSpd -= statsToChange.extraSpd;
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
