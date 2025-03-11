using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : OnOffBlockGround.cs
    * Desc : On/Off 블록의 현재 상태를 반영하는 기능.
    *
    & Functions 
    &   [public]
    &   : SetGroundOnOff(int)                    - OnOffBlock.cs에서 호출 후 변경 사항 적용.
    &
    *
*/

public class OnOffBlockGround : MonoBehaviour
{
    [SerializeField]
    int onOffState;

    [SerializeField]
    private GameObject[] groundChilds = new GameObject[2];

    public void SetGroundOnOff(int temp){
        bool active = (temp == onOffState ? true : false);
        groundChilds[0].SetActive(active);
        groundChilds[1].SetActive(!active);
    }
}
