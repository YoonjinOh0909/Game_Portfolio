using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : PreMarkerState.cs
    * Desc : 가지고 있떤 물감(아이템) 정보를 저장할 수 있는 구조체.
    *
*/
public struct PreMarkerState{
    public bool hadMarker; //물감을 가지고 있었는가?
    public int whichMarker; //어떤 물감을 가지고 있었는가?
    public bool isSpecial;
}
