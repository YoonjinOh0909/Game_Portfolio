using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
    * File : whichInteractionObjectState.cs
    * Desc : 상호 작용한 아이템의 상태 정보를 저장할 수 있는 구조체.
    *
*/
public struct whichInteractionObjectState{
    public GameObject interactedObject;
    public int whichInteractionObjectIndex; //상호작용한 오브젝트의 index
    public PushState pushState;

    public Vector3 interactedObjectPos; //사라지는 객체일 경우 재생성을 위한 위치 정보

    public Vector3 interactedObjectRot;
    public int markerIndex;
    public String interactedObjectName; //비워진 물통일 경우 다시 비워줘야하기 때문에 이름으로 탐색
    public bool isSpecial; 
    
}