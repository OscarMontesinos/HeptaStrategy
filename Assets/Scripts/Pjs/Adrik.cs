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
    public int h1Flames;
    public int h1Turn;
    public int h1Range;
    public int h1Area;
    public float h1Dmg;
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
        return value * (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy) / 100;
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
                                else  if (target.hSelected && target == this)
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
                        if(pAmount > pMAmount)
                        {
                            pAmount = pMAmount;
                        }
                        flamesBar.value = pAmount;
                        flamesText.text = (Mathf.Lerp(pMinMultiplier, pMaxMultiplier, Mathf.InverseLerp(0, pMAmount, pAmount)) * stats.sinergy).ToString("F0");
                        stats.turn -= h4Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                }
            }




            if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn)
            {
                SelectHab(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && pAmount >= h2Flames)
            {
                SelectHab(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && pAmount >= h3Flames && h3ActualSun == null)
            {
                SelectHab(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && pAmount >= h4Flames)
            {
                SelectHab(4);
            }

            switch (habSelected)
            {
                default:
                    GameManager.Instance.UnselectAll();
                    break;

                case 1:
                    if(GameManager.Instance.selectedCell == new Vector2(transform.position.x, transform.position.y))
                    {
                        DisableIndicators();
                        HabSelectSingle(HabTargetType.ally,h1Range,transform.position);
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
    }
}
