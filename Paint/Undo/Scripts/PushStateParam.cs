using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : PushStateParam.cs
    * Desc : GorightBefore.cs 내 CommonPushState(PushStateParam) 함수에서 사용.
    *        Push 할 때, 상황에 따라 필요한 정보들만 파라미터로 넘긴다.
    *
*/
public class PushStateParam
{
    // Push 상황 확인
    public PushState? tmpPushstate {get; set;}

    // 특수 상태 확인
    public bool? markerSpecialStatus {get; set;}

    // 상호작용 오브젝트
    public GameObject interactedObj {get; set;}

    // 상호작용 당시 플레이어 정보
    public Vector3? playerposition {get; set;}
    public int? prePlayerColor {get; set;}

    // 가지고 있는 물감(아이템) 정보
    public Vector3? markerPos {get; set;}
    public Vector3? markerRot {get; set;}
    public int? preMarkerColor {get; set;}
    public int? tempColorindex {get; set;}

    // 상호작용한 물통 정보
    public int? prevBowlColor {get; set;}
    public bool? prevbowlSpecialStatus {get; set;}
    
}

