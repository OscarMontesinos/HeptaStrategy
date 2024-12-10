using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nadine : PjBase
{
    public enum Weapon
    {
        pistol, rifle, shotgun
    }
    Weapon pWeapon;
    public int h1MaxRTimes;
    public int h1MaxRTimesPistol;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1TurnPistol;
    public int h1Range;
    public float h1Dmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Range;
    bool h2Active;
    public float h2Dmg;
    public int h3MaxRTimes;
    int h3CurrentTimes;
    public int h3Turn;
    public int h3Range;
    public float h3Dmg;
    public int h4MaxRTimes;
    int h4CurrentTimes;
    public int h4Turn;
    public int h4Range;
    public float h4Dmg;

    public override void Start()
    {
        base.Start();
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
                                        case Weapon.pistol:
                                            DealDmg(target, DmgType.magical, CalculateStrength(h1Dmg));
                                            h1CurrentTimes--;
                                            stats.turn -= h1TurnPistol;
                                                break;

                                        case Weapon.rifle:
                                            DealDmg(target, DmgType.magical, CalculateStrength(h3Dmg));
                                            h1CurrentTimes--;
                                            stats.turn -= h1Turn;
                                            break;

                                        case Weapon.shotgun:
                                            DealDmg(target, DmgType.magical, CalculateStrength(h4Dmg));
                                            h1CurrentTimes--;
                                            stats.turn -= h1Turn;
                                            break;

                                    }
                                    if (h2Active)
                                    {
                                        DealDmg(target, DmgType.magical, CalculateStrength(h2Dmg));
                                        h2Active = false;
                                    }
                                }
                            }
                        }

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;

                    case 2:
                        transform.position = UtilsClass.GetMouseWorldPosition();
                        h2Active = true;
                        h2CurrentCd = h2Cd + 1;
                        stats.turn -= h2Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;

                    case 3:
                        if (pWeapon == Weapon.pistol)
                        {
                            pWeapon = Weapon.rifle;
                            h1CurrentTimes = h1MaxRTimes;
                        }
                        else
                        {
                            pWeapon = Weapon.pistol;
                            h1CurrentTimes = h1MaxRTimesPistol;
                        }
                        h3CurrentTimes--;
                        stats.turn -= h3Turn;

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 4:
                        if (pWeapon == Weapon.pistol)
                        {
                            pWeapon = Weapon.shotgun;
                            h1CurrentTimes = h1MaxRTimes;
                        }
                        else
                        {
                            pWeapon = Weapon.pistol;
                            h1CurrentTimes = h1MaxRTimesPistol;
                        }
                        h4CurrentTimes--;
                        stats.turn -= h4Turn;

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
            case Weapon.pistol:
                h1CurrentTimes = h1MaxRTimesPistol;
                break;
            default:
                h1CurrentTimes = h1MaxRTimes;
                break;
        }

        if (h2CurrentCd > 0)
        {
            h2CurrentCd--;
        }

        h3CurrentTimes = h3MaxRTimes;

        h4CurrentTimes = h4MaxRTimes;

    }
    public override void ManageHabCDsUI()
    {
        bool rifle = false;
        bool shotgun = false;

        switch (pWeapon)
        {
            case Weapon.rifle:
                rifle = true;
                break;
            case Weapon.shotgun:
                shotgun = true;
                break;
        }

        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, h2Active);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.maxR, h3CurrentTimes, rifle);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.maxR, h4CurrentTimes, shotgun);
    }
    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switch (pWeapon)
            {
                case Weapon.pistol:
                    if (stats.turn >= h1TurnPistol && h1CurrentTimes > 0)
                    {
                        SelectHab(1);
                    }
                    break;

                default:
                    if (stats.turn >= h1Turn && h1CurrentTimes > 0)
                    {
                        SelectHab(1);
                    }
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentCd <= 0)
        {
            SelectHab(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && h3CurrentTimes > 0)
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
                switch (pWeapon)
                {
                    case Weapon.pistol:
                        HabSelectSingle(HabTargetType.enemy, h1Range, transform.position);
                        break;

                    case Weapon.rifle:
                        HabSelectSingle(HabTargetType.enemy, h3Range, transform.position);
                        break;

                    case Weapon.shotgun:
                        HabSelectSingle(HabTargetType.enemy, h4Range, transform.position);
                        break;
                }
                break;

            case 2:
                HabSelectSingle(HabTargetType.enemy, h2Range, transform.position);
                break;

            case 3:
                HabSelectArea(HabTargetType.ally, 1, transform.position);
                break;

            case 4:
                HabSelectArea(HabTargetType.ally, 1, transform.position);
                break;
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Armada hasta los dientes";
            case 1:
                return "Disparo a presión";
            case 2:
                return "Voltereta";
            case 3:
                return "Rifle";
            case 4:
                return "Escopeta";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "\nNadine tiene tres armas. Al atacar con el ataque básico usa el arma equipada y cambia las propiedades del ataque.";
            case 1:
                return "Golpea con el arma equipada actual\n\n" +
                       "Pistola:\nRango: " + h1Range +" Daño: " + CalculateStrength(h1Dmg).ToString("F0") + "\n\n" +
                       "Rifle:\nRango: " + h3Range + " Daño: " + CalculateStrength(h3Dmg).ToString("F0") + "\n\n" +
                       "Escopeta:\nRango: " + h4Range + " Daño: " + CalculateStrength(h4Dmg).ToString("F0");
            case 2:
                return "Rueda por el suelo moviéndose y potenciando su siguiente tiro por " + CalculateStrength(h2Dmg).ToString("F0") + " de daño físico adicional";
            case 3:
                return "Equipa o desequipa su rifle";
            case 4:
                return "Equipa o desequipa su escopeta";
        }
    }
}
