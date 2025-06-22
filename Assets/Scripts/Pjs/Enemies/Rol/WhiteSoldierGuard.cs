using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PjBase;
using static TableTopUtils;

public class WhiteSoldierGuard : PjBase
{
    public GameObject pMarker;
    public float pProtAmount;
    public float pArea;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public int h2MaxRTimes;
    int h2CurrentTimes;
    public int h2Turn;
    public int h2Range;
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
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));
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
                                if (target != null)
                                {
                                    if (target.hSelected && target != this)
                                    {
                                        activated = true;
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h2Dmg));
                                    }
                                }
                            }
                        }

                        if (activated)
                        {
                            h2CurrentTimes--;
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
    
    int GetSoldierCount()
    {
        int soldierCount = 0;
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(GameManager.Instance.GetCell(transform.position), new Vector2(pArea*2, pArea*2), 0, GameManager.Instance.unitLayer);
        PjBase pj;

        foreach (Collider2D enemyColl in enemiesHit)
        {
            pj = enemyColl.GetComponent<PjBase>();
            if ((pj.GetComponent<WhiteSoldierGuard>() || pj.GetComponent<WhiteSoldierLeader>()) && pj != this)
            {
                soldierCount++;
            }
        }
        return soldierCount;
    }

    public override float CalculateFRes()
    {
        return base.CalculateFRes() + CalculateControl(GetSoldierCount() * pProtAmount);
    }

    public override float CalculateMRes()
    {
        return base.CalculateFRes() + CalculateControl(GetSoldierCount() * pProtAmount);
    }

    public override void GetTurn()
    {
        pMarker.SetActive(true);
        base.GetTurn();
    }

    public override void EndTurn()
    {
        pMarker.SetActive(false);
        base.EndTurn();
    }

    public override void ManageHabCDs()
    {
        h1CurrentTimes = h1MaxRTimes;

        h2CurrentTimes = h2MaxRTimes;
    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);
    }

    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes > 0)
        {
            SelectHab(1, h1Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h1CurrentTimes > 0)
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
                return "Formación coordinada";
            case 1:
                return "Golpe de espada";
            case 2:
                return "Ballesta ligera";
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
                return "Obtiene " + CalculateControl(pProtAmount).ToString("F0") + " de resistencias por cada soldado blanco cerca de él";
            case 1:
                return "Lanza un espadazo a un objetivo infligiendo " + CalculateStrength(h1Dmg).ToString("F0") + " de daño";
            case 2:
                return "Dispara a un enemigo con su ballesta causando " + CalculateSinergy(h2Dmg).ToString("F0") + " de daño a los objetivo";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
}
