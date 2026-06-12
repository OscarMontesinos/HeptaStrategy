using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    public string customName;
    public TextMeshProUGUI nameText;

    // Start is called before the first frame update
    void Start()
    {
        if(customName != "")
        {
            nameText.text = customName;
        }
        else
        {
            nameText.text = name;
        }
    }

}
