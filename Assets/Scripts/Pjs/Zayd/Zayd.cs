using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static PjBase;

public class Zayd : PjBase
{
    public StatsToBuff pStatsToChange;
    public float pDmg;
    public float pDmgMultiplier;
    public int pDuration;
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
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Range;
    public float h3Dmg;
    public int h4MaxRTimes;
    int h4CurrentTimes;
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
                                    SetDebuff(target);

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
                                    if (target.GetComponent<Deterioration>())
                                    {
                                        SetDebuff(target);
                                    }
                                    SetDebuff(target);

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
                                    SetDebuff(target);

                                    foreach (Buff buff in target.buffList)
                                    {
                                        if (!buff.debuff)
                                        {
                                            buff.StartCoroutine(buff.Die());
                                        }
                                    }

                                    DealDmg(target, DmgType.magical, CalculateSinergy(h3Dmg));
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
                                if (target.hSelected && target != this && target.GetComponent<Deterioration>())
                                {
                                    activated = true;
                                    SetDebuff(target);

                                    DealDmg(target, DmgType.magical, CalculateSinergy(h4Dmg));
                                }
                            }
                        }

                        if (activated)
                        {
                            h4CurrentTimes--;
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


    void SetDebuff(PjBase target)
    {
        if (!target.GetComponent<Deterioration>())
        {
            Buff buff = target.gameObject.AddComponent<Deterioration>();
            buff.NormalSetUp(this, target, pStatsToChange, pDuration, null, true);
            target.buffList.Add(buff);
        }
        else
        {
            target.GetComponent<Deterioration>().NormalSetUp(this, target, pStatsToChange, pDuration, null, true);
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

        h4CurrentTimes = h4MaxRTimes;
    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.maxR, h4CurrentTimes, false);
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
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && h4CurrentTimes > 0)
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
                HabSelectArea(HabTargetType.enemy, h1Range, h1Area, transform.position);
                break;

            case 2:
                HabSelectArea(HabTargetType.enemy, h2Range, h2Area, transform.position);
                break;

            case 3:
                HabSelectSingle(HabTargetType.enemy, h3Range, transform.position);
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
                return "Maldición del deterioro";
            case 1:
                return "Maleficio de la mordida";
            case 2:
                return "Ritual impuro";
            case 3:
                return "Herejía";
            case 4:
                return "Ensañamiento";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Los ataques de Zayd causan Deterioro durante " + pDuration + " rondas, " +
                       "cuando ataca a un enemigo que ya sufre este efecto lo mejora un nivel y reinicia la duración\n\n" +
                       "Nivel 1: Extenúa un " + CalculateControl(pStatsToChange.pot).ToString("F0") +"\n" +
                       "Nivel 2: Aplica un veneno de " + CalculateSinergy(pDmg).ToString("F0") + "\n" +
                       "Nivel 3: Ralentiza " + CalculateControl(pStatsToChange.spd).ToString("F0") + "\n" +
                       "Nivel 4: Debilita un " + CalculateControl(pStatsToChange.res).ToString("F0") + "\n" +
                       "Nivel 5: El veneno hace el doble de daño";
            case 1:
                return "Manda su magia que toma la forma de decenas de serpientes que causan un intenso dolor a todo objetivo con el que entren en contacto " +
                       "haciendo " + CalculateSinergy(h1Dmg).ToString("F0") + " de daño";
            case 2:
                return "Lanza una descarga de poder en una zona que daña " + CalculateSinergy(h2Dmg).ToString("F0") + " a los objetivos, " +
                       "los enemigos que ya tengan Deterioro aumentan sus cargas en dos en lugar de en uno";
            case 3:
                return "Conjura una maldición que daña por " + CalculateSinergy(h3Dmg).ToString("F0") + " y elimina todos los bufos de un enemigo";
            case 4:
                return "Se ensaña con un objetivo afectado por deterioro, causándole daño igual a " + CalculateSinergy(h4Dmg).ToString("F0");
        }
    }
}
