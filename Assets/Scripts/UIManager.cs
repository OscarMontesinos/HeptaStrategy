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
    bool isActive = true;
    PjBase pj;
    public TextMeshProUGUI turnoTxt;
    public TextMeshProUGUI statsTxt;
    public TextMeshProUGUI habNameTxt;
    public TextMeshProUGUI habDescriptionTxt;

    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Slider shieldBar;

    public HabIndicator habIndicator1;
    public HabIndicator habIndicator2;
    public HabIndicator habIndicator3;
    public HabIndicator habIndicator4;
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            UIPj.SetActive(!isActive);
            isActive = !isActive;
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
                            pj.stats.fRes.ToString("F0") + "\n" +
                            pj.stats.mRes.ToString("F0") + "\n" +
                            pj.stats.spd.ToString("F0");
            habNameTxt.text = pj.GetHabName(pj.habSelected);
            habDescriptionTxt.text = pj.GetHabDescription(pj.habSelected);

            hpBar.maxValue = pj.stats.mHp;
            hpBar.value = pj.stats.hp;
            shieldBar.maxValue = pj.stats.mHp;
            shieldBar.value = pj.stats.shield;
        }
    }
}
