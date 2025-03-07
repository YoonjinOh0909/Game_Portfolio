using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : SpecialPumpkin.cs
    * Desc : 특수 상태 정보를 저장할 수 있는 구조체.
    *
*/
public struct SpecialPumpkin{
    public bool isSpecial;
    public float specialTime;
    public int prevColor;
}