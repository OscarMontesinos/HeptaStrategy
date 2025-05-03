using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockColossus : PjBase
{
    bool pActive = true;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg1;
    public float h1Dmg2;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Duration;
    public float h2Amount;
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Area;
    public float h3Dmg;
    public StatsToBuff h3StatsToChange;
    public int h3Duration;
    public int h4Cd;
    int h4CurrentCd;
    public int h4Turn;
    public int h4Range;
    public float h4Dmg;

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
                                    if (h1CurrentTimes > 1)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg1));
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg2));
                                    }
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

                        Shield shield = gameObject.AddComponent<Shield>();
                        shield.ShieldSetUp(this, this, CalculateControl(h2Amount), h2Duration, null);
                        buffList.Add(shield);

                        h2CurrentCd = h2Cd + 1;
                        stats.turn -= h2Turn;

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

                                    Buff debuff = target.gameObject.AddComponent<Buff>();
                                    debuff.NormalSetUp(this, target, h3StatsToChange, h3Duration, null, true);
                                    target.buffList.Add(debuff);

                                    DealDmg(target, DmgType.phisical, CalculateStrength(h3Dmg));
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
                    case 4:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h4Dmg));
                                    Stun(target);
                                }
                            }
                        }

                        if (activated)
                        {
                            h4CurrentCd = h4Cd + 1;
                            stats.turn -= h4Turn;
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

    public override void GetStunned()
    {
        if (pActive)
        {
            pActive = false;
        }
        else
        {
            base.GetStunned();
        }
    }

    public override void ManageHabCDs()
    {
        h1CurrentTimes = h1MaxRTimes;

        if (h2CurrentCd > 0)
        {
            h2CurrentCd--;
        }

        if (h3CurrentCd > 0)
        {
            h3CurrentCd--;
        }

        if (h4CurrentCd > 0)
        {
            h4CurrentCd--;
        }
    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, h1CurrentTimes < 1);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.cd, h4CurrentCd, false);
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
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && h4CurrentCd <= 0)
        {
            SelectHab(4);
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
                HabSelectArea(HabTargetType.ally, 0, transform.position);
                break;

            case 3:
                HabSelectArea(HabTargetType.enemy, h3Area, transform.position);
                break;

            case 4:
                HabSelectSingle(HabTargetType.enemy, h4Range, transform.position);
                break;
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Armadura tectónica";
            case 1:
                return "Golpe colosal";
            case 2:
                return "Fortificación";
            case 3:
                return "Quebrar";
            case 4:
                return "Lanzamiento de roca";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Si es aturdido desactiva esta pasiva en su lugar";
            case 1:
                return "Lanza un puñetazo a un enemigo y lo daña " + CalculateStrength(h1Dmg1).ToString("F0") + ". " +
                       "El segundo golpe en el mismo turno hace " + CalculateStrength(h1Dmg2).ToString("F0") + " de daño en su lugar";
            case 2:
                return "Conjura un escudo y se protege " + CalculateControl(h2Amount).ToString("F0") + " con él";
            case 3:
                return " Da un pisotón lanzando una onda sísmica que debilita a los enemigos " + CalculateControl(h3StatsToChange.res).ToString("F0") + " " +
                       "y los daña " + CalculateStrength(h3Dmg).ToString("F0");
            case 4:
                return "Lanza una roca y aturde a un enemigo, haciendo daño por " + CalculateStrength(h4Dmg).ToString("F0");
        }
    }
}
