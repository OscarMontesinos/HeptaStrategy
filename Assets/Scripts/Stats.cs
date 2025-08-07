using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int lvl = 5;
    public int turn = 5;
    public int size = 1;
    public float mHpMultiplier = 1;
    public float mHp;
    [HideInInspector]
    public float hp;
    [HideInInspector]
    public float regen;
    [HideInInspector]
    public float shield;
    public float sinergy;
    public float strength;
    public float control;
    public float fRes;
    public float mRes;
    public float spd;
    public int movement = 4;
    [Header("Buffs")]
    public float pot;
    public float extraCon;
    public float res;
    public float extraFRes;
    [HideInInspector]
    public float extraMRes;
    public float extraSpd;
    public int extraMov;
    public int extraTurn;
}
