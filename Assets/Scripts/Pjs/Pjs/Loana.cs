using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Loana : PjBase
{
    Shield pShield;
    bool pRenew;
    public float pAmount;
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
    public int h2Wide;
    public float h2Dmg;
    public float h2ExtraDmg;
    public StatsToBuff h2StatsToChange;
    public int h2Duration;
    public int h3Cd;
    int h3CurrentCd;
    public int h3Turn;
    public int h3Area;
    public int h3Duration;
    public GameObject h3Bubble;
    Invocation h3CurrentBubble;
    public int h4Cd;
    int h4CurrentCd;
    public int h4Turn;
    [HideInInspector]
    public bool h4Pasive;
    [HideInInspector]
    public bool h4Stun;
    public StatsToBuff h4StatsToChange;
    public int h4Duration;
    public override void Start()
    {
        base.Start();
        pShield = gameObject.AddComponent<Shield>();
        pShield.ShieldSetUp(this,this,CalculateControl(pAmount),0,null);
        buffList.Add(pShield);
    }

    public override void TakeDmg(PjBase user, DmgType dmgType, float value)
    {
        base.TakeDmg(user, dmgType, value);
        pRenew = false;
    }
    public override void GetTurn()
    {
        base.GetTurn();
        if (pRenew)
        {
            if(pShield != null)
            {
                pShield.SetShieldAmount(CalculateControl(pAmount));
            }
            else
            {
                pShield = gameObject.AddComponent<Shield>();
                pShield.ShieldSetUp(this, this, CalculateControl(pAmount), 0, null);
                buffList.Add(pShield);
            }
        }
    }
    public override void EndTurn()
    {
        base.EndTurn();
        pRenew = true;
        h4Stun = false;
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
                        foreach (PjBase target in GameManager.Instance.pjList)
                        {
                            if (target != null)
                            {
                                if (target.hSelected && target != this)
                                {
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h1Dmg));
                                    if (h4Pasive)
                                    {
                                        h4Pasive = false;
                                        if (h4Stun)
                                        {
                                            h4Stun = false;
                                            Stun(target);
                                        }
                                        DealDmg(target, DmgType.magical, CalculateSinergy(h1ExtraDmg));
                                    }
                                }
                            }
                        }
                        h1CurrentTimes--;
                        stats.turn -= h1Turn;
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
                                    Buff debuff = target.gameObject.AddComponent<Buff>();
                                    debuff.NormalSetUp(this, target, h2StatsToChange, h2Duration, null,true);
                                    target.buffList.Add(debuff);
                                    DealDmg(target, DmgType.magical, CalculateSinergy(h2Dmg));
                                    if (h4Pasive)
                                    {
                                        h4Pasive = false;
                                        h4Stun = false;
                                        DealDmg(target, DmgType.magical, CalculateSinergy(h2ExtraDmg));
                                    }
                                }
                            }
                        }
                        h2CurrentCd = h2Cd + 1;
                        stats.turn -= h2Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 3:
                        h3CurrentBubble = Instantiate(h3Bubble,transform.position,transform.rotation).GetComponent<Invocation>();
                        h3CurrentBubble.NormalSetUp(this, h3Duration);
                        invocationList.Add(h3CurrentBubble);
                        h3CurrentCd = h3Cd + 1;
                        stats.turn -= h3Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 4:
                        Buff buff = gameObject.AddComponent<Buff>();
                        buff.NormalSetUp(this, this, h4StatsToChange, h4Duration, null,false);
                        buffList.Add(buff);
                        h4Pasive = true;
                        h4Stun = true;
                        h4CurrentCd = h4Cd + 1;
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
        if (h4Stun)
        {
            UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes,true);
        }
        else
        {
            UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);
        }

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.cd, h2CurrentCd, false);

        if (h3CurrentBubble)
        {
            UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentBubble.duration, true); 
        }
        else
        {
            UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.cd, h3CurrentCd, false);
        }

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.cd, h4CurrentCd, false);
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
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && h4CurrentCd <= 0)
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
                HabSelectSingle(HabTargetType.enemy, h1Range, transform.position);
                break;

            case 2:
                HabSelectExtension(HabTargetType.enemy, h2Range, h2Wide, transform.position);
                break;

            case 3:
                HabSelectArea(HabTargetType.none, 0, h3Area, transform.position);
                break;

            case 4:
                HabSelectArea(HabTargetType.ally, 0, transform.position);
                break;
        }
    }

    public override string GetHabName(int hab)
    {
        switch (hab)
        {
            default:
                return "Armadura oceánica";
            case 1:
                return "Filo de plata";
            case 2:
                return "Onda acuática";
            case 3:
                return "Burbuja protectora";
            case 4:
                return "Hoja y coraza";
        }
    }
    public override string GetHabDescription(int hab)
    {
        switch (hab)
        {
            default:
                return "Loana comienza con un escudo igual a " + CalculateControl(pAmount).ToString("F0") + " si durante 1 ronda no recibe daño lo vuelve a obtener.";
            case 1:
                return "Ataca con su espada infligiendo " + CalculateSinergy(h1Dmg).ToString("F0") + " de daño mágico. " +
                       "Hace " + CalculateSinergy(h1ExtraDmg).ToString("F0") + " de daño mágico adicional si es potenciado por Hoja y coraza.";
            case 2:
                return "Lanza una onda de agua haciendo un tajo con la espada causa " + CalculateSinergy(h2Dmg).ToString("F0") + " " +
                       "y extenúa " + CalculateControl(h2StatsToChange.pot).ToString("F0") + " a los enemigos afectados.";
            case 3:
                return "Conjura una burbuja que crece hasta convertirse en una cúpula que bloquea los proyectiles enemigos.";
            case 4:
                return "Alza su escudo obteniendo un bono de resistencias de " + CalculateControl(h4StatsToChange.res).ToString("F0") + " hasta el siguiente turno " +
                       "y potencia su siguiente ataque básico o onda acuática, el ataque básico aturde si realiza el ataque el mismo turno que se activa la habilidad.";
        }
    }
}
