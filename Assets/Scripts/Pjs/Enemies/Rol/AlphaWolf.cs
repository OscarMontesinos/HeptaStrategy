using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaWolf : PjBase
{
    public StatsToBuff pStatsToChange;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public float h1HpPercentage;
    public float h1ExtraDmg;
    Mark actualMark;
    public int h2Turn;
    public int h2Range;
    public StatsToBuff h2StatsToChange;
    public GameObject h2Fx;
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Area;
    public StatsToBuff h3StatsToChange;
    public int h3Duration;

    public override void Start()
    {
        base.Start();
        foreach(PjBase target in GameManager.Instance.pjList)
        {
            Buff buff = target.gameObject.AddComponent<Buff>();
            buff.NormalSetUp(this, target, pStatsToChange, 0, null, false);
            target.buffList.Add(buff);
        }
    }
    public override void Update()
    {
        base.Update();

        if (turno)
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool activated = false;
                switch (habSelected)
                {
                    default:
                        break;
                    case 1:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    if (target.stats.hp < (target.stats.mHp * 40) / 100)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1ExtraDmg));
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));
                                    }


                                    Vector2 dir2 = target.transform.position - transform.position;
                                    transform.position = target.transform.position;
                                    transform.Translate(-dir2.normalized);
                                }
                            }
                        }

                        if (activated)
                        {
                            h1CurrentTimes--;
                            stats.turn -= h1Turn;
                        }
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 2:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    if (actualMark)
                                    {
                                        StartCoroutine(actualMark.Die());
                                    }
                                    Mark buff = target.gameObject.AddComponent<Mark>();
                                    buff.NormalSetUp(this, target, h2StatsToChange, 0, h2Fx, true);
                                    target.buffList.Add(buff);
                                    actualMark = buff;
                                }
                            }
                        }

                        if (activated)
                        {
                            stats.turn -= h2Turn;
                        }

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 3:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.team == team && target != this)
                                {
                                    activated = true;
                                    Buff buff = target.gameObject.AddComponent<Buff>();
                                    buff.NormalSetUp(this, target, h3StatsToChange, h3Duration, null, false);
                                    target.buffList.Add(buff);
                                }
                            }
                        }

                        if (activated)
                        {
                            h3CurrentCd = h3Cd + 1;
                            stats.turn -= h3Turn;
                        }

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                }
            }

            UpdateUI();

            ManageHabCDsUI();

            ManageHabInputs();

            ManageHabIndicators();

        }
    }

    public override void ManageHabCDs()
    {
        h1CurrentTimes = h1MaxRTimes;

        if (h3CurrentCd > 0)
        {
            h3CurrentCd--;
        }
    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, 0, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);
    }

    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes > 0)
        {
            SelectHab(1, h1Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn)
        {
            SelectHab(2, h2Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && h3CurrentCd <= 0)
        {
            SelectHab(3, h3Turn);
        }
    }
    public override void ManageHabIndicators()
    {
        switch (habSelected)
        {
            default:
                GameManager.Instance.UnselectAll();
                break;

            case 1:
                HabSelectSingle(HabTargetType.enemy, h1Range, transform.position);
                break;

            case 2:
                HabSelectSingle(HabTargetType.enemy, h2Range, transform.position);
                break;

            case 3:
                HabSelectArea(HabTargetType.ally, h3Area, transform.position,false);
                break;
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Somos manada";
            case 1:
                return "Mordida fatal";
            case 2:
                return "Marcar presa";
            case 3:
                return "Aullido inspirador";
            case 4:
                return "";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Todos sus aliados obtienen "+ CalculateControl(pStatsToChange.res).ToString("F0") + " de resistencias adicionales cuando esta unidad está luchando";
            case 1:
                return "Muerde al objetivo infligiendo " + CalculateStrength(h1Dmg).ToString("F0") + " de daño, si la vida del enemigo es menor de " + h1HpPercentage + "% " +
                    "inflige " + CalculateStrength(h1Dmg).ToString("F0") + " de daño en su lugar";
            case 2:
                return "Da una orden y marca como presa a un enemigo";
            case 3:
                return "Aúlla para inspirar a su manada potenciando a sus aliados el siguiente turno por" + CalculateControl(h3StatsToChange.pot).ToString("F0");
            case 4:
                return "";
        }
    }
}
