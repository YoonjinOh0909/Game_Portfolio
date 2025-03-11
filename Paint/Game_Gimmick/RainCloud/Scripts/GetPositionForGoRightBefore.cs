using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : GetPositionForGoRightBefore.cs
    * Desc : 뒤로가기(Undo) 기능을 위해 플레이어가 들어오기 직전 위치와 색상 저장 기능 구현.
    *
    & Functions 
    &   [public]
    &   : GetPlayerPosition()                    - 플레이어가 들어온 위치 return
    &   : GetPrePlayerColor()                    - 플레이어의 직전 색상 return
    *
*/

public class GetPositionForGoRightBefore : MonoBehaviour
{
    [SerializeField]
    private Vector3 playerPosition;
    int prePlayerColor;

    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag=="Player"){
            playerPosition = other.gameObject.transform.position;
            prePlayerColor = other.GetComponent<Player>().ColorGetter();
        }
    }

    public Vector3 GetPlayerPosition(){
        return playerPosition;
    }

    public int GetPrePlayerColor(){
        return prePlayerColor;
    }
}
