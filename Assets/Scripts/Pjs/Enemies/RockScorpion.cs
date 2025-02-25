using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScorpion : PjBase
{
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Range;
    public int h2Wide;
    public StatsToBuff h2StatsToChange;
    public int h2Duration;
    public float h2Dmg;
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Range;
    public float h3Dmg;

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
                                    DealDmg(target, DmgType.magical, CalculateStrength(h1Dmg));
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
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h2Dmg));

                                    Buff debuff = target.gameObject.AddComponent<Buff>();
                                    debuff.NormalSetUp(this, target, h2StatsToChange, h2Duration, null, true);
                                    target.buffList.Add(debuff);

                                    Vector2 dir = UtilsClass.GetMouseWorldPosition() - transform.position;
                                    transform.Translate(dir.normalized * (h2Range - 1));
                                }
                            }
                        }

                        if (activated)
                        {
                            h2CurrentCd = h2Cd + 1;
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
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    if (target.stats.res < 0)
                                    {
                                        DealDmg(target, DmgType.magical, CalculateStrength(h3Dmg * 2));
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.magical, CalculateStrength(h3Dmg));
                                    }
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

        if (h2CurrentCd > 0)
        {
            h2CurrentCd--;
        }
    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);
    }

    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes > 0)
        {
            SelectHab(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentCd <= 0)
        {
            SelectHab(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && h3CurrentCd <= 0)
        {
            SelectHab(3);
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
                HabSelectExtension(HabTargetType.enemy, h2Range, h2Wide, transform.position);
                break;

            case 3:
                HabSelectSingle(HabTargetType.enemy, h3Range, transform.position);
                break;
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "";
            case 1:
                return "Maleficio de la mordida";
            case 2:
                return "Ritual impuro";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "";
            case 1:
                return "Lanza golpes con sus tenazas que causan " + CalculateStrength(h1Dmg).ToString("F0") + " de daño";
            case 2:
                return "Avanza a través de sus enemigos dañandolos por " + CalculateStrength(h2Dmg).ToString("F0") + " y ralentizándolos";
            case 3:
                return "Lanza un golpe punzante con su aguijón que causa " + CalculateStrength(h3Dmg).ToString("F0") + " de daño, " + CalculateStrength(h3Dmg).ToString("F0") + " si el objetivo está debilitado";
            case 4:
                return "";
        }
    }
}
