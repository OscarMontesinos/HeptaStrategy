using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PjBase;
using static TableTopUtils;

public class BlackSoldierAssassin : PjBase
{
    public GameObject pMarker;
    public float pMovAmount;
    public float pArea;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public float h1PotDmg;
    public int h2Cd;
    int h2CurrentCd;
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
                                    if(GetSoldierCount() == 0)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1PotDmg));
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));
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
                                        
                                        Vector2 dir2 = target.transform.position - transform.position;
                                        transform.position = target.transform.position;
                                        transform.Translate(dir2.normalized);
                                    }
                                }
                            }
                        }

                        if (activated)
                        {
                            h2CurrentCd = h2Cd;
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
            if (pj.GetComponent<BlackSoldierAssassin>() && pj != this)
            {
                soldierCount++;
            }
        }
        return soldierCount;
    }

    public override float CalculateMov()
    {
        if (GetSoldierCount() > 0)
        {
            return base.CalculateMov();
        }
        else
        {
            return base.CalculateMov() + pMovAmount;
        }
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
                return "Formación coordinada";
            case 1:
                return "Puñalada oportunista";
            case 2:
                return "Asalto";
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
                return "Si no tiene cerca de él a otro soldado negro obtiene " + CalculateControl(pMovAmount).ToString("F0") + " puntos de movimiento";
            case 1:
                return "Lanza una cuchillada con su daga que inflige " + CalculateStrength(h1Dmg).ToString("F0") + " de daño físico si formación coordinada esta" +
                    " activada hace " + CalculateStrength(h1PotDmg).ToString("F0") + " de daño extra";
            case 2:
                return "Salta y ataca a un enemigo posicionándose a su espalda haciendo " + CalculateSinergy(h2Dmg).ToString("F0") + " de daño";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
}
