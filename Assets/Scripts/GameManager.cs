using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PjBase>pjList = new List<PjBase>();
    public int actualTurn;
    public PjBase selectedPj;
    public Vector2 selectedCell;
    [HideInInspector]
    public GameState gameState;
    public LayerMask unitLayer;

    public Color32 team0Color;
    public Color32 team1Color;
    public enum GameState
    {
        none, planning, selectingTargets
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        selectedCell = GetCell(UtilsClass.GetMouseWorldPosition().x, UtilsClass.GetMouseWorldPosition().y);
    }
    public Vector2 GetCell(Vector2 pos)
    {
        return GetCell(pos.x, pos.y);
    }
    public Vector2 GetCell(float x, float y)
    {
        if (x % 1 > 0.5f)
        {
            x = x - (x % 1) + 1;
        }
        else if (x % 1 < -0.5f)
        {
            x = x - (x % 1) - 1;
        }
        else
        {
            x = x - (x % 1);
        }

        if (y % 1 > 0.5f)
        {
            y = y - (y % 1) + 1;
        }
        else if (y % 1 < -0.5f)
        {
            y = y - (y % 1) - 1;
        }
        else
        {
            y = y - (y % 1);
        }

        return new Vector2(x, y);
    }
    void StartGame()
    {
        pjList = new List<PjBase>(FindObjectsOfType<PjBase>());

        SetTeamColors();

        foreach (PjBase pj in pjList)
        {
            pj.hasTurn = true;
        }

        CalculateTurnOrder();

        StartTurn();
    }

    void SetTeamColors()
    {
        foreach (PjBase pj in pjList)
        {
            if(pj.team == 0)
            {
                pj.teamIndicator.color = team0Color;
            }
            else
            {
                pj.teamIndicator.color = team1Color;
            }
        }
    }

    void CalculateTurnOrder()
    {
        int i1 = 0;
        int i2 = 0;
        while(i1 <= pjList.Count-1)
        {
            while(i2 < pjList.Count-1)
            {
                if (pjList[i2].hasTurn && pjList[i2 + 1].stats.spd > pjList[i2].stats.spd)
                {
                    PjBase pj = pjList[i2 + 1];
                    pjList[i2 + 1] = pjList[i2];
                    pjList[i2] = pj;
                }

                i2++;
            }

            i2 = 0;
        i1++;
        }
    }

    public void NextTurn()
    {
        pjList[actualTurn].EndTurn();

        CalculateTurnOrder();
        actualTurn = 0;

        while (!pjList[actualTurn].hasTurn && actualTurn < pjList.Count)
        {
            actualTurn++;
            if (actualTurn >= pjList.Count)
            {
                break;
            }
        }

        if(actualTurn >= pjList.Count)
        {
            actualTurn = 0;

            foreach (PjBase pj in pjList)
            {
                pj.hasTurn = true;
            }
        }

        StartTurn();
    }

    void StartTurn()
    {
        pjList[actualTurn].GetTurn();
    }

    public void UnselectAll()
    {
        foreach (PjBase pj in pjList)
        {
            if (pj != null)
            {
                pj.HSelect(false);
            }
        }

    }
    public void Kill(PjBase target)
    {
        if (pjList.IndexOf(target) <= actualTurn && pjList.IndexOf(target) >= 0)
        {
            actualTurn--;
        }
        else if (target.turno)
        {
            NextTurn();
        }
        pjList.Remove(target);

        Destroy(target.gameObject);
    }
}
