using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Zafir : PjBase
{
    public enum Weapon
    {
        spear, shield, gloves
    }
    Weapon pWeapon;
    public GameObject pFx;
    public int pDuration;
    [HideInInspector]
    public List<PjBase> pList = new List<PjBase>();
    public StatsToBuff pStatsToChange;
    public int h1MaxRTimesSpear;
    public int h1MaxRTimesShield;
    public int h1MaxRTimesGloves;
    int h1CurrentTimes;
    public int h1TurnSpear;
    public int h1TurnShield;
    public int h1TurnGloves;
    public int h1RangeSpear;
    public int h1RangeShield;
    public int h1RangeGloves;
    public float h1DmgSpear;
    public float h1DmgShield;
    public float h1DmgGloves;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Range;
    public int h2Wide;
    public float h2Dmg;
    public int h3Cd;
    public int h3ExtraCd;
    int h3CurrentCd;
    bool h3Active;
    public int h3Turn;
    public int h3ExtraTurn;
    public int h3Area;
    public float h3ShieldAmount;
    public int h3ShieldDuration;
    public int h3Range;
    public float h3Dmg;
    public float h3ResBuff;
    float h3ActualResBuff;
    public int h4Cd;
    int h4CurrentCd;
    public int h4Turn;
    public float h4Dmg;
    public float h4Heal;

    public override void GetTurn()
    {
        h3Active = false;
        base.GetTurn();
    }
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
                                    switch (pWeapon)
                                    {
                                        case Weapon.spear:
                                            DealDmg(target, DmgType.phisical, CalculateStrength(h1DmgSpear));
                                            h1CurrentTimes--;
                                            stats.turn -= h1TurnSpear;
                                            break;

                                        case Weapon.shield:
                                            DealDmg(target, DmgType.phisical, CalculateStrength(h1DmgShield));
                                            h1CurrentTimes--;
                                            stats.turn -= h1TurnShield;
                                            break;

                                        case Weapon.gloves:
                                            DealDmg(target, DmgType.phisical, CalculateStrength(h1DmgGloves));
                                            h1CurrentTimes--;
                                            stats.turn -= h1TurnGloves;
                                            break;

                                    }
                                }
                            }
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
                                    if (target.team == team)
                                    {
                                        SetBuff(target);
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.magical, CalculateStrength(h2Dmg));
                                    }
                                }
                            }
                        }

                        if (activated)
                        {
                            stats.res -= h3ActualResBuff;
                            h3ActualResBuff = 0;
                            if (pWeapon == Weapon.gloves)
                            {
                                h1CurrentTimes--;
                            }
                            pWeapon = Weapon.spear;
                            h2CurrentCd = h2Cd + 1;
                            stats.turn -= h2Turn;
                        }

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 3:
                        bool extraActivated = false;
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected)
                                {
                                    activated = true;
                                    if (!h3Active)
                                    {
                                        activated = true;
                                        Shield shield = target.gameObject.AddComponent<Shield>();
                                        shield.ShieldSetUp(this, target, CalculateControl(h3ShieldAmount), h3ShieldDuration, null);
                                        target.buffList.Add(shield);
                                        SetBuff(target);
                                    }
                                    else
                                    {
                                        Stun(target);
                                        DealDmg(target, DmgType.magical, CalculateStrength(h3Dmg));
                                        extraActivated = true;

                                        Vector2 dir = target.transform.position - transform.position;
                                        transform.position = target.transform.position;
                                        transform.Translate(-dir.normalized);
                                    }
                                }
                            }
                        }

                        if (activated)
                        {
                            if (!h3Active)
                            {
                                h3Active = true;
                            }
                            else
                            {
                                h3Active = false;
                            }
                            if(pWeapon == Weapon.gloves)
                            {
                                h1CurrentTimes--;
                            }
                            pWeapon = Weapon.shield;
                            h3ActualResBuff = CalculateControl(h3ResBuff);
                            stats.res += h3ActualResBuff;
                            h3CurrentCd = h3Cd + 1;
                            stats.turn -= h3Turn;
                            if (extraActivated)
                            {
                                h3CurrentCd += h3ExtraCd;
                                stats.turn -= h3ExtraTurn;
                            }
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
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    if (target.team == team)
                                    {
                                        Heal(target, CalculateControl(h4Heal));
                                        SetBuff(target);
                                    }
                                    else
                                    {
                                        DealDmg(target, DmgType.magical, CalculateStrength(h4Dmg));
                                    }
                                }
                            }
                        }


                        if (activated)
                        {
                            stats.res -= h3ActualResBuff;
                            h3ActualResBuff = 0;
                            pWeapon = Weapon.gloves;
                            h1CurrentTimes++;
                            h4CurrentCd = h4Cd + 1;
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

    public override void ManageHabCDs()
    {
        switch (pWeapon)
        {
            case Weapon.spear:
                h1CurrentTimes = h1MaxRTimesSpear;
                break;
            case Weapon.shield:
                h1CurrentTimes = h1MaxRTimesShield;
                break;
            case Weapon.gloves:
                h1CurrentTimes = h1MaxRTimesGloves;
                break;
        }

        if (h2CurrentCd > 0)
        {
            h2CurrentCd--;
        }

        if (h3CurrentCd > 0)
        {
            h3CurrentCd--;
        }

        if (h4CurrentCd > 0)
        {
            h4CurrentCd--;
        }

    }
    public override void ManageHabCDsUI()
    {
        bool spear = false;
        bool shield = false;
        bool gloves = false;

        switch (pWeapon)
        {
            case Weapon.spear:
                spear = true;
                break;
            case Weapon.shield:
                shield = true;
                break;
            case Weapon.gloves:
                gloves = true;
                break;
        }


        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, spear);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, shield);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.cd, h4CurrentCd, gloves);
    }
    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switch (pWeapon)
            {
                case Weapon.spear:
                    if (stats.turn >= h1TurnSpear && h1CurrentTimes > 0)
                    {
                        SelectHab(1, h1TurnSpear);
                    }
                    break;
                case Weapon.shield:
                    if (stats.turn >= h1TurnShield && h1CurrentTimes > 0)
                    {
                        SelectHab(1, h1TurnShield);
                    }
                    break;
                case Weapon.gloves:
                    if (stats.turn >= h1TurnGloves && h1CurrentTimes > 0)
                    {
                        SelectHab(1, h1TurnGloves);
                    }
                    break;

            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentCd <= 0)
        {
            SelectHab(2, h2Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && ((stats.turn >= h3Turn && h3CurrentCd <= 0) || (h3Active && stats.turn >= h3ExtraTurn)))
        {
            SelectHab(3, h3Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && h4CurrentCd <= 0)
        {
            SelectHab(4, h4Turn);
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
                switch (pWeapon)
                {
                    case Weapon.spear:
                        HabSelectSingle(HabTargetType.enemy, h1RangeSpear, transform.position);
                        break;

                    case Weapon.shield:
                        HabSelectSingle(HabTargetType.enemy, h1RangeShield, transform.position);
                        break;

                    case Weapon.gloves:
                        HabSelectSingle(HabTargetType.enemy, h1RangeGloves, transform.position);
                        break;
                }
                break;

            case 2:
                HabSelectExtension(HabTargetType.both, h2Range, h2Wide, transform.position);
                break;

            case 3:
                if (!h3Active)
                {
                    HabSelectArea(HabTargetType.ally, h3Area, transform.position);
                }
                else
                {
                    HabSelectSingle(HabTargetType.enemy, h3Range, transform.position);
                }
                break;

            case 4:
                HabSelectZafir();
                break;
        }

    }

    public override void DealDmg(PjBase target, DmgType dmgType, float dmgAmount)
    {
        SetBuff(target);
        base.DealDmg(target, dmgType, dmgAmount);
    }

    void SetBuff(PjBase target)
    {
        if (target != null && target != this)
        {
            StatsToBuff stats = new StatsToBuff();
            stats.regen = pStatsToChange.regen;
            stats.res = pStatsToChange.res;
            if (!pList.Contains(target))
            {
                SolarLight buff = target.gameObject.AddComponent<SolarLight>();
                buff.NormalSetUp(this, target, stats, pDuration, pFx, false);
                target.buffList.Add(buff);
            }
            else
            {
                target.GetComponent<SolarLight>().duration = pDuration;
            }
        }
    }

    public void HabSelectZafir()
    {
        GameManager.Instance.UnselectAll();
        foreach (PjBase pj in pList)
        {
            if (pj != null)
            {
                pj.HSelect(true);
            }
        }
    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Guerrero solar";
            case 1:
                return "Afinidad con las armas";
            case 2:
                return "Lanza solar";
            case 3:
                return "Escudo dorado";
            case 4:
                return "Liberación solar";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Las habilidades de Zafir otorgan luz solar durante " + pDuration + " rondas a todas las unidades que afecte con ellas.\n" +
                       "Los aliados obtienen un bono de protección de " + CalculateControl(pStatsToChange.res).ToString("F0") + " " +
                       "Los enemigos otorgan a Zafir " + CalculateControl(pStatsToChange.res) + " " +
                       "de regeneración por enemigo. \nEl arma de Zafir cambia tras usar una habilidad modificando el ataque básico de diferentes formas";
            case 1:
                return "Golpea con el arma equipada actual\n\n" +
                       "Lanza : \nTurno: " + h1TurnSpear + " Rango: " + h1RangeSpear + " Daño " + CalculateStrength(h1DmgSpear).ToString("F0") + "\n\n" +
                       "Escudo : \nTurno: " + h1TurnShield + " Rango: " + h1RangeShield + " Daño " + CalculateStrength(h1DmgShield).ToString("F0") + "\n" +
                       "Otorga un bonus de " + CalculateControl(h3ResBuff).ToString("F0") + " a las resistencias \n\n" +
                       "Guanteletes : \nTurno: " + h1TurnGloves + " Rango: " + h1RangeGloves + " Daño " + CalculateStrength(h1DmgGloves).ToString("F0") + "\n\n";
            case 2:
                return "Invoca lanzas luminosas en cono que hacen " + CalculateStrength(h2Dmg).ToString("F0") + " de daño mágico";
            case 3:
                return "Lanza un pulso mágico que otorga un escudo de "+ CalculateControl(h3ShieldAmount).ToString("F0") +" a los aliados alrededor, " +
                       "puede reactivar la habilidad para abalanzarse a un enemigo y lo aturdirlo, causando " + CalculateStrength(h3Dmg).ToString("F0") + " de daño mágico";
            case 4:
                return "Activa todas las marcas solares haciéndolas explotar en un pulso que cura " + CalculateControl(h4Heal).ToString("F0") + " a los aliados " +
                       "o daña "+ CalculateStrength(h4Dmg).ToString("F0") +" a los enemigos";
        }
    }
}
