using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Loana : PjBase
{
    Shield pShield;
    bool pRenew;
    public float pAmount;
    public int h1Turn;
    public int h1Range;
    public float h1Dmg;
    public float h1ExtraDmg;
    public override void Start()
    {
        base.Start();
        pShield = gameObject.AddComponent<Shield>();
        pShield.ShieldSetUp(this,this,CalculateControl(pAmount),0,null);
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
            pShield.SetShieldAmount(CalculateControl(pAmount));
        }
    }
    public override void EndTurn()
    {
        base.EndTurn();
        pRenew = true;
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
                                }
                            }
                        }
                        stats.turn -= h1Turn;
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

            switch (habSelected)
            {
                default:
                    GameManager.Instance.UnselectAll();
                    break;

                case 1:
                    HabSelectSingle(HabTargetType.enemy, h1Range, transform.position);
                    break;
            }
        }
    }
}
