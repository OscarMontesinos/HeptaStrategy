using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.DebugUI;

public class PjBase : MonoBehaviour
{
    public int team;
    public Stats stats;
    [HideInInspector]
    public bool turno;
    [HideInInspector]
    public int habSelected;
    public GameObject turnIndicator;
    public GameObject hSelectedIndicator;
    public Slider hpBar;
    public Slider shieldBar;
    public TextMeshProUGUI hpText;
    [HideInInspector]
    public bool hSelected;

    public GameObject singleIndicator;
    public GameObject areaIndicator;
    public GameObject extensionIndicator;

    public enum DmgType
    {
        none, magical, phisical
    }
    public enum HabSelectionType
    {
        none, single
    }
    public enum HabTargetType
    {
        none, ally, enemy, both
    }

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            habSelected = 0;
            GameManager.Instance.UnselectAll();
            DisableIndicators();
        }
    }


    public virtual void Start()
    {
        CalculateStats();
        GameManager.Instance.pjList.Add(this);
        stats.hp = stats.mHp;
        hpBar.maxValue = stats.mHp;
        shieldBar.maxValue = stats.mHp;
        UpdateUI();
    }

    public void UpdateUI()
    {
        hpBar.value = stats.hp;
        shieldBar.value = stats.shield;
        if (stats.shield > 0)
        {
            hpText.text = stats.hp.ToString("F0") + " (" + stats.shield.ToString("F0") + ")";
        }
        else
        {
            hpText.text = stats.hp.ToString("F0");
        }
    }

    void CalculateStats()
    {
        stats.mHp = stats.mHp * 7.5f;
        stats.sinergy = stats.sinergy * 2.5f;
        stats.strength = stats.strength * 2.5f;
        stats.control = stats.control * 2.5f;
        stats.fDef = stats.fDef * 2.5f;
        stats.mDef = stats.mDef * 2.5f;
        stats.spd = stats.spd * 2.5f;

        float lvlMultiplier = (float)(stats.lvl + 5) / 40;

        stats.mHp *= lvlMultiplier;
        stats.sinergy *= lvlMultiplier;
        stats.strength *= lvlMultiplier;
        stats.control *= lvlMultiplier;
        stats.fDef *= lvlMultiplier;
        stats.mDef *= lvlMultiplier;
        stats.spd *= lvlMultiplier;


        stats.mHp = MathF.Truncate(stats.mHp);
        stats.sinergy = MathF.Truncate(stats.sinergy);
        stats.strength = MathF.Truncate(stats.strength);
        stats.control = MathF.Truncate(stats.control);
        stats.fDef = MathF.Truncate(stats.fDef);
        stats.mDef = MathF.Truncate(stats.mDef);
        stats.spd = MathF.Truncate(stats.spd);

    }

    private void OnMouseEnter()
    {
        GameManager.Instance.selectedPj = this;
    }
    private void OnMouseExit()
    {
        if (GameManager.Instance.selectedPj == this)
        {
            GameManager.Instance.selectedPj = null;
        }
    }

    public virtual void GetTurn()
    {
        turno = true;
        turnIndicator.SetActive(true);
        stats.turn = 5;
    }

    public virtual void EndTurn()
    {
        turno = false;
        turnIndicator.SetActive(false);
    }

    public void SelectHab(int habSelected)
    {
        DisableIndicators();
       this.habSelected = habSelected;
    }

    public virtual float CalculateSinergy(float value)
    {
        return value * stats.sinergy / 100;
    }
    public virtual float CalculateStrength(float value)
    {
        return value * stats.strength / 100;
    }
    public virtual float CalculateControl(float value)
    {
        return value * stats.control / 100;
    }

    public void DealDmg(PjBase target, DmgType dmgType, float dmgAmount)
    {
        target.TakeDmg(this, dmgType, dmgAmount);
    }

    public virtual void TakeDmg(PjBase user, DmgType dmgType, float value)
    {
        float calculo = 0;

        if (dmgType == DmgType.magical)
        {
            calculo = stats.mDef;
        }
        else
        {
            calculo = stats.fDef;
        }

        if (calculo < 0)
        {
            calculo = 0;
        }

        value *= 1 - (calculo / (calculo + 40 + 1 * user.stats.lvl));
        float originalValue = value;
        while (stats.shield > 0 && value > 0)
        {
            Shield chosenShield = null;
            foreach (Shield shield in GetComponents<Shield>())
            {
                if (chosenShield == null || shield.duration < chosenShield.duration && shield.shieldAmount > 0)
                {
                    chosenShield = shield;
                }
            }
            value = chosenShield.ChangeShieldAmount(-value);

        }

        /*if(value != originalValue)
        {
            originalValue -= value;
            DamageText sText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
            sText.textColor = Color.white;
            sText.damageText.text = originalValue.ToString("F0");
        }*/

        stats.hp -= value;
        if (stats.hp <= 0)
        {
            StartCoroutine(Die());
        }
        if (hpBar != null)
        {
            UpdateUI();
        }
    }

    public void HSelect(bool select)
    {
        hSelected = select;
        hSelectedIndicator.SetActive(select);
    }

    public void DisableIndicators()
    {
        singleIndicator.SetActive(false);
        areaIndicator.SetActive(false);
        extensionIndicator.SetActive(false);
    }

    public void HabSelectSingle(HabTargetType habTargetType, int range, Vector2 originPos)
    {
        singleIndicator.SetActive(true);
        singleIndicator.transform.localScale = new Vector3(range * 2 + 1, range * 2 + 1, range * 2 + 1);

        GameManager.Instance.UnselectAll();
        PjBase targetPj = GameManager.Instance.selectedPj;


        if (targetPj && CheckRange(originPos, targetPj.transform.position, range))
        {
            switch (habTargetType)
            {
                case HabTargetType.none:
                    break;

                case HabTargetType.enemy:
                    if (targetPj.team != team)
                    {
                        targetPj.HSelect(true);
                    }
                    break;

                case HabTargetType.ally:
                    if (targetPj.team == team)
                    {
                        targetPj.HSelect(true);
                    }
                    break;
                case HabTargetType.both:
                    targetPj.HSelect(true);
                    break;
            }
        }

    }
    public void HabSelectArea(HabTargetType habTargetType, int range, int area, Vector2 originPos)
    {
        singleIndicator.SetActive(true);
        areaIndicator.SetActive(true);
        areaIndicator.transform.position = UtilsClass.GetMouseWorldPosition();
        singleIndicator.transform.localScale = new Vector3(range * 2 + 1, range * 2 + 1, range * 2 + 1);
        areaIndicator.transform.localScale = new Vector3(area, area, area);

        GameManager.Instance.UnselectAll();
        Vector2 cell = GameManager.Instance.selectedCell;


        if (CheckRange(originPos, cell, range))
        {
            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(cell, new Vector2(area, area), 0, GameManager.Instance.unitLayer);
            PjBase pj;

            switch (habTargetType)
            {
                case HabTargetType.none:
                    break;

                case HabTargetType.enemy:
                    foreach (Collider2D enemyColl in enemiesHit)
                    {
                        pj = enemyColl.GetComponent<PjBase>();
                        if (pj.team != team)
                        {
                            pj.HSelect(true);
                        }

                    }
                    break;

                case HabTargetType.ally:
                    foreach (Collider2D enemyColl in enemiesHit)
                    {
                        pj = enemyColl.GetComponent<PjBase>();
                        if (pj.team == team)
                        {
                            pj.HSelect(true);
                        }
                    }
                    break;
                case HabTargetType.both:
                    foreach (Collider2D enemyColl in enemiesHit)
                    {
                        pj = enemyColl.GetComponent<PjBase>();
                        pj.HSelect(true);
                    }
                    break;
            }
        }
        else
        {
            areaIndicator.SetActive(false);
        }

    }
    public void HabSelectExtension(HabTargetType habTargetType, int range, int wide, Vector2 originPos)
    {
        extensionIndicator.SetActive(true);
        extensionIndicator.transform.position = originPos;
        extensionIndicator.transform.up = (Vector2)UtilsClass.GetMouseWorldPosition() - originPos;
        extensionIndicator.transform.GetChild(0).localScale = new Vector3(wide, range, 1);

        GameManager.Instance.UnselectAll();
        Vector2 cell = GameManager.Instance.selectedCell;


        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(extensionIndicator.transform.GetChild(0).GetChild(0).position, new Vector2(wide, range), extensionIndicator.transform.localEulerAngles.z, GameManager.Instance.unitLayer);
        PjBase pj;

        switch (habTargetType)
        {
            case HabTargetType.none:
                break;

            case HabTargetType.enemy:
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    pj = enemyColl.GetComponent<PjBase>();
                    if (pj.team != team)
                    {
                        pj.HSelect(true);
                    }
                }
                break;

            case HabTargetType.ally:
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    pj = enemyColl.GetComponent<PjBase>();
                    if (pj.team == team)
                    {
                        pj.HSelect(true);
                    }
                }
                break;
            case HabTargetType.both:
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    pj = enemyColl.GetComponent<PjBase>();
                    pj.HSelect(true);
                }
                break;
        }

    }

    public bool CheckRange(Vector2 originPos, Vector2 targetPos, int range)
    {
        float distX = MathF.Abs(targetPos.x - originPos.x);
        float distY = MathF.Abs(targetPos.y - originPos.y);
        return (distX <= range && distY <= range);
    }

    public IEnumerator Die()
    {
        yield return null;
        GameManager.Instance.Kill(this);
    }

}