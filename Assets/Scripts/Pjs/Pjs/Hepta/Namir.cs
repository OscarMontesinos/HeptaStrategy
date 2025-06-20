using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Namir : PjBase
{
    public TextMeshProUGUI pText;
    int pCharges;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public float h1ExtraDmg;
    public int h2Cd;
    int h2CurrentCd;
    public int h2Turn;
    public int h2Range;
    public float h2Dmg;
    public float h2ExtraDmg;
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Range;
    public int h3Wide;
    public float h3Dmg;
    public float h3ExtraDmg;
    public GameObject h4Indicator;
    public int h4Cd;
    int h4CurrentCd;
    public int h4Turn;
    bool h4Active;
    public int h4Range;
    public int h4ExtraMovement;
    public int h4ExtraPassiveCharges;

    public override void Start()
    {
        base.Start();
        stats.movement += h4ExtraMovement;
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
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));
                                    if (h4Active)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateControl(h1ExtraDmg));
                                    }
                                    if(pCharges > 1)
                                    {
                                        pCharges -= 2;
                                        DealDmg(target, DmgType.phisical, CalculateStrength(h1Dmg));
                                        if (h4Active)
                                        {
                                            DealDmg(target, DmgType.phisical, CalculateControl(h1ExtraDmg));
                                        }
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
                                if (target.hSelected && target != this)
                                {
                                    activated = true;
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h2Dmg));
                                    if (h4Active)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateControl(h2ExtraDmg));
                                    }

                                    Vector2 dir2 = target.transform.position - transform.position;
                                    transform.position = target.transform.position;
                                    transform.Translate(-dir2.normalized);
                                }
                            }
                        }

                        if (activated)
                        {
                            pCharges++;
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
                                    DealDmg(target, DmgType.phisical, CalculateStrength(h3Dmg));
                                    if (h4Active)
                                    {
                                        DealDmg(target, DmgType.phisical, CalculateControl(h3ExtraDmg));
                                    }

                                    
                                }
                            }
                        }

                        if (activated)
                        {
                            pCharges++;
                        }

                        Vector2 dir = UtilsClass.GetMouseWorldPosition() - transform.position;
                        transform.Translate(dir.normalized * (h3Range + 1));
                        
                            h3CurrentCd = h3Cd + 1;
                            stats.turn -= h3Turn;

                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 4:
                        if (h4Active)
                        {
                            foreach (PjBase target in GameManager.Instance.pjList)
                            {
                                if (target != null)
                                {
                                    if (target.hSelected && target != this)
                                    {
                                        activated = true;
                                        Stun(target);
                                        h4Active = false;
                                        stats.movement += h4ExtraMovement;
                                    }
                                }
                            }
                        }
                        else
                        {
                            activated = true;
                            h4Active = true;
                            stats.movement -= h4ExtraMovement;
                            pCharges += h4ExtraPassiveCharges;
                        }
                        
                        h4Indicator.SetActive(h4Active);

                        if (activated)
                        {
                            if (h4Active)
                            { 
                                h4CurrentCd = h4Cd;
                            }
                            else
                            {
                                h4CurrentCd = h4Cd + 1;
                            }
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
        h1CurrentTimes = h1MaxRTimes;

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
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.cd, h4CurrentCd, h4Active);
    }
    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes> 0)
        {
            SelectHab(1, h1Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentCd <= 0)
        {
            SelectHab(2, h2Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && h3CurrentCd <= 0)
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
                HabSelectSingle(HabTargetType.enemy, h1Range, transform.position);
                break;

            case 2:
                HabSelectSingle(HabTargetType.enemy, h2Range, transform.position);
                break;

            case 3:
                HabSelectExtension(HabTargetType.enemy, h3Range, h3Wide, transform.position);
                break;

            case 4:
                if (h4Active)
                {
                    HabSelectSingle(HabTargetType.enemy, h4Range, transform.position);
                }
                else
                {
                    HabSelectArea(HabTargetType.ally, 0, transform.position);
                }
                break;
        }

    }
    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Colmillo de serpiente";
            case 1:
                return "Tajo desértico";
            case 2:
                return "Cuchilla fugaz";
            case 3:
                return "Abalanzamiento";
            case 4:
                return "Sangre dorada";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Utilizar habilidades golpeando al menos a un enemigo obtiene una carga de colmillo, usar un ataque básico con al menos dos cargas causa un ataque básico doble\n\n" +
                       "Cargas actuales: " + pCharges;
            case 1:
                return "Lanza una cuchillada con su machete haciendo " + CalculateStrength(h1Dmg).ToString("F0") + " de daño físico. " +
                       "Dentro del estado sangre dorada causa " + CalculateControl(h1ExtraDmg).ToString("F0") + " de daño extra";
            case 2:
                return "Salta hacia un enemigo y lo ataca de forma rápida causando " + CalculateStrength(h2Dmg).ToString("F0") +
                       "Dentro del estado sangre dorada causa " + CalculateControl(h2ExtraDmg).ToString("F0") + " de daño extra";
            case 3:
                return "Se lanza cortando a los enemigos causando " + CalculateStrength(h3Dmg).ToString("F0") + " y pasando a través de ellos " +
                       "Dentro del estado sangre dorada causa " + CalculateControl(h3ExtraDmg).ToString("F0") + " de daño extra";
            case 4:
                return "Entra en el estado sangre dorada para obtener más daño en el daño de sus ataques. " +
                       "Al entrar obtiene " + h4ExtraPassiveCharges +" cargas de colmillo. " +
                       "Si ya está en el estado sangre dorada lanza cintas de luz en forma de serpiente a un enemigo aturdiéndolo, saliendo del estado. " +
                       "Si Namir no está en el estado de sangre dorada obtiene un bono de " + h4ExtraMovement + " de movimiento.";
        }
    }

    public override void UpdateUI()
    {
        pText.text = pCharges.ToString();
        base.UpdateUI();
    }
}
