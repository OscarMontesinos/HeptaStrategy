using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int lvl = 1;
    public int turn = 5;
    public float mHp;
    [HideInInspector]
    public float hp;
    [HideInInspector]
    public float shield;
    public float sinergy;
    public float strength;
    public float control;
    public float fDef;
    public float mDef;
    public float spd;
    public int movement;
}
