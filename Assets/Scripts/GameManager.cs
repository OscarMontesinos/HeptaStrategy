using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
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
        float x = UtilsClass.GetMouseWorldPosition().x;
        float y = UtilsClass.GetMouseWorldPosition().y;

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
        selectedCell = new Vector2 (x, y);
    }
    void StartGame()
    {
        pjList = new List<PjBase>(FindObjectsOfType<PjBase>());

        CalculateTurnOrder();

        StartTurn();
    }

    void CalculateTurnOrder()
    {
        int i1 = 0;
        int i2 = 0;
        while(i1 < pjList.Count-1)
        {
            while(i2 < pjList.Count-2)
            {
                if (pjList[i2 + 1].stats.spd > pjList[i2].stats.spd)
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
        actualTurn++;

        if(actualTurn >= pjList.Count)
        {
            actualTurn = 0;
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
        if (pjList.IndexOf(target) < actualTurn)
        {
            actualTurn--;
        }
        pjList.Remove(target);
        Destroy(target.gameObject);
    }
}
