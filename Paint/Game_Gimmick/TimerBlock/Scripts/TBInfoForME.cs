using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
    * File : TBInfoForME.cs
    * Desc : 타이머 블록의 정보를 저장할 수 있는 클래스.
    *
*/

[Serializable]
public class TBInfoForME
{
    public float animTimeInfo = 0f;
    public float brokenTimeInfo = 0f;
    public float animTimeFormotionInfo =0f;
    public float brokenTimeFormotionInfo =0f;
    public bool isTouchedInfo =false;
    public bool isBrokenInfo = false;
}
