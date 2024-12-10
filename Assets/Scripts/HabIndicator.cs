using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabIndicator : MonoBehaviour
{
    public enum CdType
    {
        none, cd, maxR, unactive
    }

    public TextMeshProUGUI cdText;
    public TextMeshProUGUI maxRText;
    public GameObject maxRIcon;
    public GameObject darkerIcon;
    public GameObject specialIcon;
    public void UpdateHab(CdType cdType, int cd, bool activateSpecialIcon)
    {
        specialIcon.SetActive(activateSpecialIcon);
        switch (cdType)
        {
            case CdType.none:
                darkerIcon.SetActive(false);
                maxRIcon.SetActive(false);
                cdText.text = "";
                break;

            case CdType.cd:
                maxRIcon.SetActive(false);
                if (cd <= 0)
                {
                    darkerIcon.SetActive(false);
                    cdText.text = "";
                }
                else
                {
                    darkerIcon.SetActive(true);
                    cdText.text = cd.ToString();
                }
                break;

            case CdType.maxR: 
                maxRIcon.SetActive(true);
                cdText.text = "";
                maxRText.text = cd.ToString();
                if (cd <= 0)
                {
                    darkerIcon.SetActive(true);
                }
                else
                {
                    darkerIcon.SetActive(false);
                }
                break;
                case CdType.unactive:
                maxRIcon.SetActive(false);
                cdText.text = ""; 
                darkerIcon.SetActive(true);
                break;
        }
    }
}
