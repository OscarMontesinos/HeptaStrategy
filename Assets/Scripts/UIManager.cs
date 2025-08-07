using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject UIPj;
    bool isActive = false;
    PjBase pj;
    public TextMeshProUGUI turnoTxt;
    public TextMeshProUGUI statsTxt;
    public TextMeshProUGUI habNameTxt;
    public TextMeshProUGUI habDescriptionTxt;
    public TextMeshProUGUI habTurnTxt;
    public TextMeshProUGUI nameTxt;

    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Slider shieldBar;

    public HabIndicator habIndicator1;
    public HabIndicator habIndicator2;
    public HabIndicator habIndicator3;
    public HabIndicator habIndicator4;

    public GameObject synergyIndicator;
    public GameObject strengthIndicator;
    public GameObject controlIndicator;
    public GameObject prIndicator;
    public GameObject mrIndicator;
    public GameObject spdIndicator;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && pj.turno)
        {
            UIPj.SetActive(!isActive);
            isActive = !isActive;
        }
        if (Input.GetMouseButtonDown(1) && GameManager.Instance.pjList[GameManager.Instance.actualTurn].habSelected == 0)
        {
            Collider2D collider = Physics2D.OverlapCircle(UtilsClass.GetMouseWorldPosition(), 0.5f);
            if (collider)
            {
                SetPj(collider.gameObject.GetComponent<PjBase>());
            }
        }
    }
    public void SetPj(PjBase pj)
    {
        this.pj = pj;
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (pj)
        {
            nameTxt.text = pj.name;
            if (!pj.turno)
            {
                UIPj.SetActive(false);
                isActive = false;
            }
            turnoTxt.text = pj.stats.turn.ToString("F0");
            if (pj.stats.shield > 0)
            {
                hpText.text = pj.stats.hp.ToString("F0") + " (" + pj.stats.shield.ToString("F0") + ") / " +pj.stats.mHp.ToString("F0");
            }
            else
            {
                hpText.text = pj.stats.hp.ToString("F0") + " / " + pj.stats.mHp.ToString("F0");
            }

            statsTxt.text = pj.CalculateSinergy(100).ToString("F0") + "\n" +
                            pj.CalculateStrength(100).ToString("F0") + "\n" +
                            pj.CalculateControl(100).ToString("F0") + "\n" +
                            (pj.CalculateFRes()).ToString("F0") + "\n" +
                            (pj.CalculateMRes()).ToString("F0") + "\n" +
                            (pj.stats.spd + pj.stats.extraSpd).ToString("F0") + "\n" +
                            (pj.CalculateMov()).ToString("F0");

            if(pj.stats.pot > 0)
            {
                synergyIndicator.transform.GetChild(0).gameObject.SetActive(true);
                strengthIndicator.transform.GetChild(0).gameObject.SetActive(true);
                synergyIndicator.transform.GetChild(1).gameObject.SetActive(false);
                strengthIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (pj.stats.pot < 0)
            {
                synergyIndicator.transform.GetChild(0).gameObject.SetActive(false);
                strengthIndicator.transform.GetChild(0).gameObject.SetActive(false);
                synergyIndicator.transform.GetChild(1).gameObject.SetActive(true);
                strengthIndicator.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                synergyIndicator.transform.GetChild(0).gameObject.SetActive(false);
                strengthIndicator.transform.GetChild(0).gameObject.SetActive(false);
                synergyIndicator.transform.GetChild(1).gameObject.SetActive(false);
                strengthIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (pj.stats.res + pj.stats.extraMRes > 0)
            {
                mrIndicator.transform.GetChild(0).gameObject.SetActive(true);
                mrIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (pj.stats.res + pj.stats.extraMRes < 0)
            {
                mrIndicator.transform.GetChild(0).gameObject.SetActive(false);
                mrIndicator.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                mrIndicator.transform.GetChild(0).gameObject.SetActive(false);
                mrIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (pj.stats.res + pj.stats.extraFRes > 0)
            {
                prIndicator.transform.GetChild(0).gameObject.SetActive(true);
                prIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (pj.stats.res + pj.stats.extraFRes < 0)
            {
                prIndicator.transform.GetChild(0).gameObject.SetActive(false);
                prIndicator.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                prIndicator.transform.GetChild(0).gameObject.SetActive(false);
                prIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (pj.stats.extraSpd > 0)
            {
                spdIndicator.transform.GetChild(0).gameObject.SetActive(true);
                spdIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (pj.stats.extraSpd < 0)
            {
                spdIndicator.transform.GetChild(0).gameObject.SetActive(false);
                spdIndicator.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                spdIndicator.transform.GetChild(0).gameObject.SetActive(false);
                spdIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }

            habNameTxt.text = pj.GetHabName(pj.habSelected);
            habTurnTxt.text = pj.GetHabTurn(pj.habSelected);
            habDescriptionTxt.text = pj.GetHabDescription(pj.habSelected);

            hpBar.maxValue = pj.stats.mHp;
            hpBar.value = pj.stats.hp;
            shieldBar.maxValue = pj.stats.mHp;
            shieldBar.value = pj.stats.shield;
        }
    }

    public void AddMov()
    {
        if(pj.stats.turn > 0)
        {
            pj.stats.turn--;
            pj.stats.extraMov++;
        }
    }
}
