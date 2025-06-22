using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TableTopUtils
{
    public static Vector2 Normalize(Vector2 vector)
    {
        if(vector.x > 0)
        {
            vector.x = 1;
        }
        else if (vector.x < 0)
        {
            vector.x = -1;
        }
        else
        {
            vector.x = 0;
        }

        if (vector.y > 0)
        {
            vector.y = 1;
        }
        else if (vector.y < 0)
        {
            vector.y = -1;
        }
        else
        {
            vector.y = 0;
        }

        return vector;
    }
}
