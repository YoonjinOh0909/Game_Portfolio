using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : GameState.cs
    * Desc : 플레이어의 정보를 저장할 수 있는 구조체.
    *
*/
public struct GameState{
    //1.플레이어 위치
    public Vector3 playerPos;

    //1-2. 플레이어 회전
    public Vector3 playerRot;

    //2. 플레이어 색상
    public int playerColor;

    //3. 상호작용 전 물감을 가지고 있었는가?
    //3-1. 어떤 물감을 가지고 있는가?
    public PreMarkerState preMarker;

    //4. 상호작용 전 붓을 가지고 있었는가?
    public bool hadBrush;


    //5. 무엇이랑 상호작용 했는가
    public whichInteractionObjectState interacctedObj;

    //6. 시간 특수 상태에 따른 player 상태 추가
    public SpecialPumpkin preSpecialPumpkin;
}