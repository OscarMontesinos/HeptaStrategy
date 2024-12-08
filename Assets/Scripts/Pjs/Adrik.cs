using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.GraphicsBuffer;

public class Adrik : PjBase
{
    public Slider flamesBar;
    public TextMeshProUGUI flamesText;
    public int pMAmount;
    int pAmount;
    public float pMaxMultiplier;
    public float pMinMultiplier;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Flames;
    public int h1Turn;
    public int h1Range;
    public int h1Area;
    public float h1Dmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Flames;
    public int h2Turn;
    public int h2Range;
    public int h2Wide;
    public float h2Dmg;
    public GameObject h3Sun;
    GameObject h3ActualSun;
    public int h3Flames;
    public int h3Turn;
    public int h3Range;
    public int h3Area;
    public float h3Dmg;
    float h3ActualDmg;
    public int h4Cd;
    int h4CurrentCd;
    public int h4Turn;
    public int h4Range;
    public int h4Flames;

    public override void Start()
    {
        base.Start();
        pAmount = pMAmount;
        flamesBar.maxValue = pMAmount;
        flamesBar.value = pAmount;
        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
    }
    public override float CalculateSinergy(float value)
    {
        return value * (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * (stats.sinergy + stats.pot)) / 100;
    }
    public override void GetTurn()
    {
        base.GetTurn();
        if (h3ActualSun)
        {
            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(h3ActualSun.transform.position, new Vector2(h3Area, h3Area), 0, GameManager.Instance.unitLayer);
            PjBase pj;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                pj = enemyColl.GetComponent<PjBase>();
                if (pj != null && pj.team != team)
                {
                    DealDmg(pj, DmgType.magical, CalculateSinergy(h3ActualDmg));
                }
            }
        }
    }
    public override void Update()
    {
        base.Update();

        if (turno)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (habSelected)
                {
                    default:
                        break;
                    case 1:
                        bool isAttack = false;
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this && pAmount >= h1Flames)
                                {
                                    isAttack = true;
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h1Dmg));
                                }
                                else if (target.hSelected && target == this)
                                {
                                    pAmount = pMAmount;
                                    stats.turn -= h1Turn;
                                    if (h3ActualSun)
                                    {
                                        Destroy(h3ActualSun);
                                    }
                                }
                            }
                        }
                        if (isAttack)
                        {
                            pAmount -= h1Flames;
                            stats.turn -= h1Turn;
                        }
                        flamesBar.value = pAmount;
                        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
                        habSelected = 0;
                        h1CurrentTimes--;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 2:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected)
                                {
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h2Dmg));
                                }
                            }
                        }

                        pAmount -= h2Flames;
                        flamesBar.value = pAmount;
                        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
                        stats.turn -= h2Turn;
                        habSelected = 0;
                        h2CurrentCd = h2Cd + 1;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 3:
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected)
                                {
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h3Dmg));
                                }
                            }
                        }
                        h3ActualDmg = CalculateSinergy(h3Dmg);
                        h3ActualSun = Instantiate(h3Sun, UtilsClass.GetMouseWorldPosition(), new Quaternion(0, 0, 0, 0));

                        pAmount -= h3Flames;
                        flamesBar.value = pAmount;
                        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
                        stats.turn -= h3Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 4:
                        transform.position = UtilsClass.GetMouseWorldPosition();
                        pAmount += h4Flames;
                        if (pAmount > pMAmount)
                        {
                            pAmount = pMAmount;
                        }
                        flamesBar.value = pAmount;
                        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
                        stats.turn -= h4Turn;
                        habSelected = 0;
                        h4CurrentCd = h4Cd + 1;
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

        if(h2CurrentCd > 0)
        {
            h2CurrentCd--;
        }

        if(h4CurrentCd > 0)
        {
            h4CurrentCd--;
        }
    }

    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd);

        if (h3ActualSun) UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.unactive, 0);
        else UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.none, 0);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.cd, h4CurrentCd);
    }

    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes >0)
        {
            SelectHab(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && pAmount >= h2Flames && h2CurrentCd <= 0)
        {
            SelectHab(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && pAmount >= h3Flames && h3ActualSun == null)
        {
            SelectHab(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && pAmount >= h4Flames && h4CurrentCd <= 0)
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
                if (GameManager.Instance.selectedCell == new Vector2(transform.position.x, transform.position.y))
                {
                    DisableIndicators();
                    HabSelectSingle(HabTargetType.ally, h1Range, transform.position);
                }
                else
                {
                    HabSelectArea(HabTargetType.enemy, h1Range, h1Area, transform.position);
                }
                break;

            case 2:
                HabSelectExtension(HabTargetType.enemy, h2Range, h2Wide, transform.position);
                break;

            case 3:
                HabSelectArea(HabTargetType.enemy, h3Range, h3Area, transform.position);
                break;

            case 4:
                HabSelectSingle(HabTargetType.none, h4Range, transform.position);
                break;
        }
    }


    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Pozo ígneo";
            case 1:
                return "Piromancia dracónica";
            case 2:
                return "Luciérnaga danzante";
            case 3:
                return "Sol en miniatura";
            case 4:
                return "Vuelo del dragon";

        }
    }


    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Adrik utiliza sus reservas mágicas para atacar, perdiendo sinergia elemental  cuando usa sus habilidades. " +
                       "Las llamas que conjura arden con fuerza, Adrik puede usar su ataque básico en sí mismo para absorber las llamas y llenar su pozo, " +
                       "con el pozo al máximo su sinergia total es de " + (stats.sinergy * pMaxMultiplier).ToString("F0") + ", al mínimo es de " + (stats.sinergy * pMinMultiplier).ToString("F0") + ". " +
                       "El pozo tiene un valor total de " + pMAmount + ".";

            case 1:
                return "Lanza una bola de fuego que explota y causa " + CalculateSinergy(h1Dmg).ToString("F0") + " de daño mágico";

            case 2:
                return "Lanza una llama danzante al aire que avanza lanzando chispas ígneas al aire que explotan haciendo " + CalculateSinergy(h2Dmg).ToString("F0") + " de daño mágico";

            case 3:
                return "Crea un sol en miniatura que emana calor ardiente. " +
                       "Al invocarlo y cada vez que empieza el turno de Adrik causa " + CalculateSinergy(h3Dmg).ToString("F0") + " de daño mágico a sus alrededores,  " +
                       "el modificador de Pozo ígneo se aplica al invocarse y no cambia nunca. " +
                       "Al rellenar el pozo también absorbe el sol.";
            case 4:
                return "Vuela a una dirección próxima recuperando " + h4Flames + " de llamas del pozo.\r\n";

        }
    }
}
