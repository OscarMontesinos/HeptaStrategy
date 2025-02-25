using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObelisk : PjBase
{
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public int h1Area;
    public float h1Dmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Area;
    public float h2Dmg;

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
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h1Dmg));
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
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h2Dmg));
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
    }
    public override void ManageHabIndicators()
    {
        switch (habSelected)
        {
            default:
                GameManager.Instance.UnselectAll();
                break;

            case 1:
                HabSelectArea(HabTargetType.enemy, h1Range, h1Area, transform.position);
                break;

            case 2:
                HabSelectArea(HabTargetType.enemy, h2Area, transform.position);
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
                return "Lanzamiento de rocas";
            case 2:
                return "Desprendimiento";
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
                return "Levanta rocas del terreno y las deja caer sobre sus objetivos infligiendo " + CalculateSinergy(h1Dmg).ToString("F0") + " de daño";
            case 2:
                return "Lanza a su alrededor fragmentos de su cuerpo como forma de autodefensa causando " + CalculateSinergy(h2Dmg).ToString("F0") + " de daño a los objetivos";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
}
