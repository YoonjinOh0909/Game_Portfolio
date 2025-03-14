using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : BoxBottomTrigger.cs
    * Desc : 상자 위에 아무것도 없을 때, 부드러운 이동을 위해 작은 콜라이더가 아래에 존재한다.
    *       이를 on, off 하는 기능
    *
*/

public class BoxBottomTrigger : MonoBehaviour
{

    [SerializeField]
    private GameObject bottomCollider;
    
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("OutBox")){
            bottomCollider.SetActive(false);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (12 < other.gameObject.layer && other.gameObject.layer < 18){
            bottomCollider.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (12 < other.gameObject.layer && other.gameObject.layer < 18){
            bottomCollider.SetActive(false);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (12 < other.gameObject.layer && other.gameObject.layer < 18){
            bottomCollider.SetActive(true);
        }        
    }
}
