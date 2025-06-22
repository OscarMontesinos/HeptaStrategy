using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : PjBase
{
    public float pMultiplier;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Range;
    public int h2RangeDash;
    public float h2Dmg;

    public override void Update()
    {
        base.Update();

        if (turno)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float pMultiplier = 1;
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
                                    foreach (Buff buff in target.buffList)
                                    {
                                        if (buff.GetComponent<Mark>())
                                        {
                                            pMultiplier += this.pMultiplier * 0.01f;
                                        }
                                    }
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));

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
                                    foreach (Buff buff in target.buffList)
                                    {
                                        if (buff.GetComponent<Mark>())
                                        {
                                            pMultiplier += this.pMultiplier * 0.01f;
                                        }
                                    }
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h2Dmg));

                                    Vector2 dir2 = target.transform.position - transform.position;
                                    transform.position = target.transform.position;
                                    transform.position = GameManager.Instance.GetCell((dir2*h2RangeDash) + (Vector2)transform.position);
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
            SelectHab(1, h1Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentCd <= 0)
        {
            SelectHab(2, h2Turn);
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
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Caza en manada";
            case 1:
                return "Mordida";
            case 2:
                return "Despedazar";
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
                return "Muerde al objetivo infligiendo " + CalculateStrength(h1Dmg).ToString("F0") + " de daño";
            case 2:
                return "Lanza a su alrededor fragmentos de su cuerpo como forma de autodefensa causando " + CalculateSinergy(h2Dmg).ToString("F0") + " de daño a los objetivos";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
}
