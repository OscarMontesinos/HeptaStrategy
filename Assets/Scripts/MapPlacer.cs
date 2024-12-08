using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using Unity.Mathematics;

public class MapPlacer : MonoBehaviour
{
    public bool isPosEditable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //PARA MOVERLO
        //if (Input.GetMouseButton(0)) { transform.position = UtilsClass.GetMouseWorldPosition(); }


        float x;
        float y;

        if (transform.position.x % 1 > 0.5f)
        {
            x = transform.position.x - (transform.position.x % 1) + 1;
        }
        else if (transform.position.x % 1 < -0.5f)
        {
            x = transform.position.x - (transform.position.x % 1) - 1;
        }
        else
        {
            x = transform.position.x - (transform.position.x % 1);
        }

        if (transform.position.y % 1 > 0.5f)
        {
            y = transform.position.y - (transform.position.y % 1) + 1;
        }
        else if (transform.position.y % 1 < -0.5f)
        {
            y = transform.position.y - (transform.position.y % 1) - 1;
        }
        else
        {
            y = transform.position.y - (transform.position.y % 1);
        }

        transform.position = new Vector3(x, y);
    }

    private void OnMouseDrag()
    {
        if (isPosEditable)
        { 
            transform.position = UtilsClass.GetMouseWorldPosition();
        }
    }
}
