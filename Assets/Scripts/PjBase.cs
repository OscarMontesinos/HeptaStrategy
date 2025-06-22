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
    public List<Buff> buffList = new List<Buff>();
    [HideInInspector]
    public List<Invocation> invocationList = new List<Invocation>();
    [HideInInspector]
    public bool isStuned;
    [HideInInspector]
    public bool wasJustStuned;
    [HideInInspector]
    public bool turno;
    [HideInInspector]
    public bool hasTurn;
    [HideInInspector]
    public int habSelected;
    public int turnHabSelected;
    public GameObject turnIndicator;
    public GameObject hSelectedIndicator;
    public GameObject stunIndicator;
    public GameObject justStunIndicator;
    public Slider hpBar;
    public Slider shieldBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public SpriteRenderer teamIndicator;
    [HideInInspector]
    public bool hSelected;

    public GameObject singleIndicator;
    public GameObject areaIndicator;
    public GameObject extensionIndicator;
    float indicatorModifier = 1;

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

    [ContextMenu("CreateReferences")]
    public void CreateReferences()
    {
        turnIndicator = transform.GetChild(0).GetChild(2).gameObject;
        hSelectedIndicator = transform.GetChild(0).GetChild(4).gameObject;
        stunIndicator = transform.GetChild(0).GetChild(0).gameObject;
        justStunIndicator = transform.GetChild(0).GetChild(1).gameObject;
        hpBar = transform.GetChild(0).GetChild(3).GetComponent<Slider>();
        shieldBar = transform.GetChild(0).GetChild(5).GetComponent<Slider>();
        hpText = transform.GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(0).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>();
        singleIndicator = transform.GetChild(1).GetChild(0).gameObject;
        areaIndicator = transform.GetChild(1).GetChild(1).gameObject;
        extensionIndicator = transform.GetChild(1).GetChild(2).gameObject;
        teamIndicator = transform.GetChild(3).GetComponent<SpriteRenderer>();
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
        nameText.text = name;
        if(stats.size % 2 == 0)
        {
            indicatorModifier = 2;
        }
        UpdateUI();
    }

    public virtual void UpdateUI()
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
        stunIndicator.SetActive(isStuned);
        justStunIndicator.SetActive(wasJustStuned);

        UIManager.Instance.UpdateUI();
    }

    void CalculateStats()
    {
        stats.mHp = stats.mHp * 6f;
        stats.sinergy = stats.sinergy * 2.5f;
        stats.strength = stats.strength * 2.5f;
        stats.control = stats.control * 2.5f;
        stats.fRes = stats.fRes * 2.5f;
        stats.mRes = stats.mRes * 2.5f;
        stats.spd = stats.spd * 2.5f;

        float lvlMultiplier = (float)(stats.lvl + 5) / 40;

        stats.mHp *= lvlMultiplier;
        stats.sinergy *= lvlMultiplier;
        stats.strength *= lvlMultiplier;
        stats.control *= lvlMultiplier;
        stats.fRes *= lvlMultiplier;
        stats.mRes *= lvlMultiplier;
        stats.spd *= lvlMultiplier;


        stats.mHp = MathF.Truncate(stats.mHp);
        stats.sinergy = MathF.Truncate(stats.sinergy);
        stats.strength = MathF.Truncate(stats.strength);
        stats.control = MathF.Truncate(stats.control);
        stats.fRes = MathF.Truncate(stats.fRes);
        stats.mRes = MathF.Truncate(stats.mRes);
        stats.spd = MathF.Truncate(stats.spd);

    }
    public virtual void ManageHabCDs()
    {

    }
    public virtual void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.unactive, 0, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.unactive, 0, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.unactive, 0, false);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.unactive, 0, false);
    }
    public virtual void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectHab(1,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectHab(2,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectHab(3,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectHab(4,0);
        }
    }
    public virtual void ManageHabIndicators()
    {
        switch (habSelected)
        {
            default:
                GameManager.Instance.UnselectAll();
                break;

            case 1:
                GameManager.Instance.UnselectAll();
                break;

            case 2:
                GameManager.Instance.UnselectAll();
                break;

            case 3:
                GameManager.Instance.UnselectAll();
                break;

            case 4:
                GameManager.Instance.UnselectAll();
                break;
        }

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
        CameraManager.Instance.transform.position = transform.position;
        turno = true;
        ManageHabCDs();
        GetHealed(this, stats.regen, true);
        turnIndicator.SetActive(true);
        stats.turn = 5;
        if (isStuned)
        {
            isStuned = false;
            stats.turn = 0;
            wasJustStuned = true;
        }
        else if (wasJustStuned)
        {
            wasJustStuned = false;
        }
        stats.extraMov = 0;
        UpdateUI();

        foreach (Invocation invocation in invocationList)
        {
            invocation.Tick();
        }

        UIManager.Instance.SetPj(this);
    }

    public virtual void EndTurn()
    {
        foreach (Buff buff in buffList)
        {
            buff.Tick();
        }

        hasTurn = false;
        turno = false;
        turnIndicator.SetActive(false);
    }

    public void SelectHab(int habSelected, int turn)
    {
        DisableIndicators();
        this.habSelected = habSelected;
        turnHabSelected = turn;
    }

    public virtual float CalculateSinergy(float value)
    {
        return value * (stats.sinergy + stats.pot) / 100;
    }
    public virtual float CalculateStrength(float value)
    {
        return value * (stats.strength + stats.pot) / 100;
    }
    public virtual float CalculateControl(float value)
    {
        return value * (stats.control + stats.extraCon) / 100;
    }

    public virtual void DealDmg(PjBase target, DmgType dmgType, float dmgAmount)
    {
        target.TakeDmg(this, dmgType, dmgAmount);
    }

    public virtual void TakeDmg(PjBase user, DmgType dmgType, float value)
    {
        float calculo = 0;

        if (dmgType == DmgType.magical)
        {
            calculo = stats.mRes + stats.extraMRes + stats.res;
        }
        else
        {
            calculo = stats.fRes + stats.extraFRes + stats.res;
        }

        if (calculo < 0)
        {
            calculo = 0;
        }

        value *= 1 - (calculo / (calculo + 40 + 1 * user.stats.lvl));
        if(value <= 1)
        {
            value = 1;
        }
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
    public virtual void Heal(PjBase target, float amount)
    {
        target.GetHealed(this, amount, false);
    }

    public virtual void GetHealed(PjBase user, float amount ,bool isRegen)
    {
        stats.hp += amount;
        if(stats.hp > stats.mHp)
        {
            stats.hp = stats.mHp;
        }

        if (hpBar != null)
        {
            UpdateUI();
        }
    }
    public virtual void Stun(PjBase user)
    {
        if (!user.wasJustStuned)
        {
            user.GetStunned();
        }
    }
    public virtual void GetStunned()
    {
        isStuned = true;
        UpdateUI();
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
        singleIndicator.transform.localScale = new Vector3(range * 2 + indicatorModifier, range * 2 + indicatorModifier, range * 2 + indicatorModifier);

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
    public void HabSelectSingle(HabTargetType habTargetType, int range, int area, Vector2 originPos)
    {
        singleIndicator.SetActive(true);
        areaIndicator.SetActive(true);
        singleIndicator.transform.localScale = new Vector3(range * 2 + indicatorModifier, range * 2 + indicatorModifier, range * 2 + indicatorModifier);
        areaIndicator.transform.position = UtilsClass.GetMouseWorldPosition();
        areaIndicator.transform.localScale = new Vector3(area, area, area);

        GameManager.Instance.UnselectAll();
        PjBase targetPj = GameManager.Instance.selectedPj;
        Vector2 cell = GameManager.Instance.selectedCell;


        if (targetPj && CheckRange(originPos, targetPj.transform.position, range))
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
    public void HabSelectArea(HabTargetType habTargetType, int range, int area, Vector2 originPos)
    {
        singleIndicator.SetActive(true);
        areaIndicator.SetActive(true);
        areaIndicator.transform.position = UtilsClass.GetMouseWorldPosition();
        singleIndicator.transform.localScale = new Vector3(range * 2 + indicatorModifier, range * 2 + indicatorModifier, range * 2 + indicatorModifier);
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
        extensionIndicator.transform.up = GameManager.Instance.selectedCell - originPos;
        extensionIndicator.transform.GetChild(0).localScale = new Vector3(wide + (indicatorModifier - 1), range, 1);

        GameManager.Instance.UnselectAll();
        Vector2 cell = GameManager.Instance.selectedCell;


        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(extensionIndicator.transform.GetChild(0).GetChild(0).position, new Vector2(wide + (indicatorModifier - 1), range), extensionIndicator.transform.localEulerAngles.z, GameManager.Instance.unitLayer);
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
    public void HabSelectArea(HabTargetType habTargetType, int area, Vector2 originPos)
    {
        HabSelectArea(habTargetType, area, originPos, true);
    }

    public void HabSelectArea(HabTargetType habTargetType, int area, Vector2 originPos, bool selectSelf)
    {
        areaIndicator.GetComponent<MapPlacer>().alternativeMethod = GetComponent<MapPlacer>().alternativeMethod;
        areaIndicator.SetActive(true);
        areaIndicator.transform.position = originPos;
        areaIndicator.transform.localScale = new Vector3(area * 2 + indicatorModifier, area * 2 + indicatorModifier, area * 2 + indicatorModifier);

        GameManager.Instance.UnselectAll();

        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(originPos, new Vector2(area * 2 + 1, area * 2 + 1), 0, GameManager.Instance.unitLayer);
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
                    if (pj.team == team && pj != this)
                    {
                        pj.HSelect(true);
                    }
                    else if (selectSelf && pj == this)
                    {
                        pj.HSelect(true);
                    }
                }
                break;
            case HabTargetType.both:
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    pj = enemyColl.GetComponent<PjBase>();
                    if (pj != this)
                    {
                        pj.HSelect(true);
                    }
                    else if (selectSelf && pj == this)
                    {
                        pj.HSelect(true);
                    }
                }
                break;
        }

    }

    public virtual string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "";
            case 1:
                return "";
            case 2:
                return "";
            case 3:
                return "";
            case 4:
                return "";
        }
    }
    public virtual string GetHabTurn(int hab)
    {
        if (hab > 0)
        {
            return "Turn: " + turnHabSelected;
        }
        else
        {
            return "Turn: 0";
        }
    }
    public virtual string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "";
            case 1:
                return "";
            case 2:
                return "";
            case 3:
                return "";
            case 4:
                return "";
        }
    }

    public bool CheckRange(Vector2 originPos, Vector2 targetPos, int range)
    {
        float distX = MathF.Abs(targetPos.x - originPos.x);
        float distY = MathF.Abs(targetPos.y - originPos.y);
        return (distX <= range + (stats.size *0.5f) && distY <= range + (stats.size * 0.5f));
    }

    public IEnumerator Die()
    {
        yield return null;
        GameManager.Instance.Kill(this);
    }

}
