using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int lvl = 5;
    public int turn = 5;
    public float mHp;
    [HideInInspector]
    public float hp;
    [HideInInspector]
    public float regen;
    [HideInInspector]
    public float pot;
    [HideInInspector]
    public float shield;
    public float sinergy;
    public float strength;
    public float control;
    public float fRes;
    public float mRes;
    [HideInInspector]
    public float res;
    public float spd;
    public int movement = 4;
}
