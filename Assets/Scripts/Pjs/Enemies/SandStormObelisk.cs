using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStormObelisk : PjBase
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
    public int h2Range;
    public int h2Area;
    public float h2Dmg;
    PjBase linkedPj;
    Buff currentBuff;
    public int h3Turn;
    public int h3Range;
    public StatsToBuff h3StatsToChange;

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
                    case 3:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this)
                                {
                                    activated = true;

                                    if (linkedPj)
                                    {
                                        currentBuff.StartCoroutine(Die());
                                    }

                                    Buff buff = target.gameObject.AddComponent<Buff>();
                                    buff.NormalSetUp(this, target, h3StatsToChange, 0, null, false);
                                    target.buffList.Add(buff);
                                    linkedPj = target;
                                    currentBuff = buff;
                                }
                            }
                        }

                        if (activated)
                        {
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

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.none,0, linkedPj);
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
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn)
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
                HabSelectSingle(HabTargetType.enemy, h1Range, h1Area, transform.position);
                break;

            case 2:
                HabSelectCustom(HabTargetType.enemy, h2Range, h2Area, transform.position);
                break;

            case 3:
                HabSelectSingle(HabTargetType.ally, h3Range, transform.position);
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
                return "Lanza un pulso de viento que explota y daña " + CalculateSinergy(h1Dmg).ToString("F0") + " a sus enemigos";
            case 2:
                return "Invoca una tormenta de arena que daña " + CalculateSinergy(h2Dmg).ToString("F0") + " a los enemigos atrapados en su interior," +
                       " la tormenta también se puede invocar en la posición del aliado vinculado con Vínculo tormentoso";
            case 3:
                return "Se vincula con un aliado, perdiendo un vínculo anterior si lo hubiera, concediendo al aliado" +
                       " velocidad " + CalculateControl(h3StatsToChange.spd).ToString("F0") + ", movimiento " + h3StatsToChange.movement + " y" +
                       " resistencia mágica" + CalculateControl(h3StatsToChange.mRes).ToString("F0");
            case 4:
                return "";
        }
    }

    public void HabSelectCustom(HabTargetType habTargetType, int range, int area, Vector2 originPos)
    {
        singleIndicator.SetActive(true);
        areaIndicator.SetActive(true);
        areaIndicator.transform.position = UtilsClass.GetMouseWorldPosition();
        singleIndicator.transform.localScale = new Vector3(range * 2 + 1, range * 2 + 1, range * 2 + 1);
        areaIndicator.transform.localScale = new Vector3(area, area, area);

        GameManager.Instance.UnselectAll();
        Vector2 cell = GameManager.Instance.selectedCell;


        if (CheckRange(originPos, cell, range) || (GameManager.Instance.selectedPj == linkedPj && linkedPj != null))
        {
            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(cell, new Vector2(area, area), 0, GameManager.Instance.unitLayer);
            PjBase pj;


            foreach (Collider2D enemyColl in enemiesHit)
            {
                pj = enemyColl.GetComponent<PjBase>();
                if (pj.team != team)
                {
                    pj.HSelect(true);
                }

            }
        }
        else
        {
            areaIndicator.SetActive(false);
        }

    }
}
