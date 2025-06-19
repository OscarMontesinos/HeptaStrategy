using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mary : PjBase
{
    public enum SongParts
    {
        opening, verse, chorus
    }
    int actualPart;
    public List<SongParts> songStructure;
    public float oAShieldAmount;
    public int oAShieldDuration;
    public float oEDmg;
    public StatsToBuff vAStatsToChange;
    public int vAPotDuration;
    public float vEDmg;
    public StatsToBuff vEStatsToChange;
    public int vEPotDuration;
    public float cAHeal;
    public float cEDmg;
    public int h1MaxRTimes;
    int h1CurrentTimes;
    public int h1Turn;
    public int h1Range;
    public int h2MaxRTimes;
    int h2CurrentTimes;
    public int h2Turn;
    public int h2Area;
    public int h3MaxRTimes;
    int h3CurrentTimes;
    public int h3Turn;
    public int h3Range;
    public int h3Wide;
    public int h4MaxRTimes;
    int h4CurrentTimes;
    public int h4Turn;
    public override void Update()
    {
        base.Update();

        if (turno)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (PjBase target in GameManager.Instance.pjList)
                {
                    if (target != null)
                    {
                        if (target.hSelected && target != this)
                        {
                            if(target.team == team)
                            {
                                switch (songStructure[actualPart])
                                {
                                    case SongParts.opening:
                                        Shield shield = target.gameObject.AddComponent<Shield>();
                                        shield.ShieldSetUp(this, target, CalculateControl(oAShieldAmount), oAShieldDuration, null);
                                        target.buffList.Add(shield);
                                        break;
                                    case SongParts.verse:
                                        StatsToBuff statsToChange = vAStatsToChange;
                                        if(target == this)
                                        {
                                            statsToChange.con = statsToChange.pot;
                                        }
                                        Buff buff = target.gameObject.AddComponent<Buff>();
                                        buff.NormalSetUp(this, target, vAStatsToChange, vAPotDuration, null, false);
                                        target.buffList.Add(buff);
                                        break;
                                    case SongParts.chorus:
                                        Heal(target, CalculateControl(cAHeal));
                                        break;

                                }
                            }
                            else
                            {
                                switch (songStructure[actualPart])
                                {
                                    case SongParts.opening:
                                        Stun(target);
                                        DealDmg(target, DmgType.magical, CalculateSinergy(oEDmg));
                                        break;
                                    case SongParts.verse:
                                        DealDmg(target, DmgType.magical, CalculateSinergy(vEDmg));
                                        Buff buff = target.gameObject.AddComponent<Buff>();
                                        buff.NormalSetUp(this, target, vEStatsToChange, vEPotDuration, null, false);
                                        target.buffList.Add(buff);
                                        break;
                                    case SongParts.chorus:

                                        DealDmg(target, DmgType.magical, CalculateSinergy(cEDmg));
                                        break;

                                }
                            }
                        }
                    }
                }
                switch (habSelected)
                {
                    default:
                        break;
                    case 1:
                        actualPart++;
                        if (actualPart > songStructure.Count)
                        {
                            actualPart = 0;
                        }
                        h1CurrentTimes--;
                        stats.turn -= h1Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 2:
                        actualPart++;
                        if (actualPart >= songStructure.Count)
                        {
                            actualPart = 0;
                        }
                        h2CurrentTimes--;
                        stats.turn -= h2Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 3:
                        actualPart++;
                        if (actualPart > songStructure.Count)
                        {
                            actualPart = 0;
                        }
                        h3CurrentTimes--;
                        stats.turn -= h3Turn;
                        habSelected = 0;
                        GameManager.Instance.UnselectAll();
                        DisableIndicators();
                        break;
                    case 4:
                        actualPart++;
                        if (actualPart > songStructure.Count)
                        {
                            actualPart = 0;
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
        h1CurrentTimes = h1MaxRTimes;

        h2CurrentTimes = h2MaxRTimes;

        h3CurrentTimes = h3MaxRTimes;

        h4CurrentTimes = h4MaxRTimes;

    }
    public override void ManageHabCDsUI()
    {
        UIManager.Instance.habIndicator1.UpdateHab(HabIndicator.CdType.maxR, h1CurrentTimes, false);

        UIManager.Instance.habIndicator2.UpdateHab(HabIndicator.CdType.maxR, h2CurrentTimes, false);

        UIManager.Instance.habIndicator3.UpdateHab(HabIndicator.CdType.maxR, h3CurrentTimes, false);

        UIManager.Instance.habIndicator4.UpdateHab(HabIndicator.CdType.maxR, h4CurrentTimes, false);
    }
    public override void ManageHabInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.turn >= h1Turn && h1CurrentTimes > 0)
        {
            SelectHab(1, h1Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && stats.turn >= h2Turn && h2CurrentTimes > 0)
        {
            SelectHab(2, h2Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && stats.turn >= h3Turn && h3CurrentTimes > 0)
        {
            SelectHab(3, h3Turn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && stats.turn >= h4Turn && h4CurrentTimes > 0)
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
                HabSelectSingle(HabTargetType.both, h1Range, transform.position);
                break;

            case 2:
                HabSelectArea(HabTargetType.ally, h2Area, transform.position);
                break;

            case 3:
                HabSelectExtension(HabTargetType.enemy, h3Range, h3Wide, transform.position);
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
        string partText;
        switch (songStructure[actualPart])
        {
            case SongParts.opening:
                partText = "Opening\nAliados: Otorga un escudo de " + CalculateControl(oAShieldAmount).ToString("F0") + " que dura " + oAShieldDuration + " rondas\n" +
                    "Enemigos: Aturde a los enemigos y les inflige " + CalculateSinergy(oEDmg) + " de daño";
                break;
            case SongParts.verse:
                partText = "Estrofa\nAliados: Otorga un potenciador a los aliados de " + CalculateControl(vAStatsToChange.pot).ToString("F0") + " que dura " + vAPotDuration +
                    " rondas, si los aliados están extenuados purifica el debufo en su lugar. " +
                    "Si Mary se bufa a sí misma también aumenta su control, pero solo puede tener un bufo activo de este tipo.\n" +
                    "Enemigos: Daña a los objetivos por " + CalculateSinergy(vEDmg) + " y los extenúa un " + CalculateControl(vEStatsToChange.pot).ToString("F0") + " durante 2 rondas";
                break;
            case SongParts.chorus:
                partText = "Estribillo\nAliados: Otorga una curación de " + CalculateControl(cAHeal).ToString("F0") + " a los aliados\n" +
                    "Enemigos: Daña a los objetivos por " + CalculateSinergy(cEDmg);
                break;
            default:
                partText = "";
                break;
        }
        switch (hab)
        {
            default:
                return "Mary toca una cancion durante la batalla, cada vez que se usa una habilidad la canción avanza así como sus efectos.\n\nParte actual:\n" + partText;
            case 1:
                return "Toca para un solo enemigo o aliado, afectándole con la parte actual de la canción\n\nParte actual:\n" + partText;
            case 2:
                return "Toca alrededor suyo afectando a sus aliados, afectándoles con la parte actual de la canción\n\nParte actual:\n" + partText;
            case 3:
                return "Toca para los enemigos delante de ella, afectándoles con la parte actual de la canción\n\nParte actual:\n" + partText;
            case 4:
                return "Toca para sí misma, afectándose con la parte actual de la canción\n\nParte actual:\n" + partText;
        }
    }
}
