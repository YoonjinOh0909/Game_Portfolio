using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : BoxTrigger.cs
    * Desc : 상자가 위 아래로 겹치거나 플레이어가 올라갔을 때 하나의 더미로 인식하는 기능 구현
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    *
*/
public class BoxTrigger : MonoBehaviour
{    
    private Box parent;
    private Transform parentTransform;
    private Transform CommonObjs;

    private int previousLayer;
    private Vector3 startPos = new Vector3 (0f,1.2f,0f);

    private void Start() {
        InitState();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.transform.CompareTag("Player")){
            int layerMask = (1 << LayerMask.NameToLayer("Ignore Raycast")) + (1 << LayerMask.NameToLayer("OutBoxBlue")) + (1 << LayerMask.NameToLayer("OutBoxRed")) + (1 << LayerMask.NameToLayer("OutBoxGreen")) + (1 << LayerMask.NameToLayer("OutBoxWhite")) + (1 << LayerMask.NameToLayer("OutBoxYellow"));

            RaycastHit2D hit = Physics2D.Raycast(transform.position- startPos, -Vector2.up, 0.05f, ~layerMask);

            if (hit.collider == null) 
            {
                DebugX.Log("상자 아래 Obj Null");
            }else{
                parent.FreezeY();
            }
            
            parentTransform = other.transform.parent;
            other.transform.SetParent(transform);

        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.transform.CompareTag("Player")){
            parentTransform = other.transform.parent;
            other.transform.SetParent(transform);            
        }
            
        if (other.gameObject.layer == LayerMask.NameToLayer("BOX"))
        {
            parentTransform = other.transform.parent.gameObject.transform.parent;
            other.transform.parent.gameObject.transform.SetParent(transform);
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
            
        if (other.gameObject.layer == LayerMask.NameToLayer("BOX"))
        {
            parentTransform = other.transform.parent.gameObject.transform.parent;
            other.transform.parent.gameObject.transform.SetParent(transform);
        }

    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.transform.CompareTag("Player")){
            parent.NonFreezeY();
            other.transform.SetParent(CommonObjs);            
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("BOX"))
        {
            other.transform.SetParent(CommonObjs);
        }
       
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("BOX"))
        {
            other.transform.parent.gameObject.transform.SetParent(CommonObjs);
        }
    }

    private void InitState(){
        parent = this.gameObject.transform.parent.gameObject.GetComponent<Box>();

        GameObject CommonObj = GameObject.FindGameObjectWithTag("Player_Position");

        if(CommonObj != null){
            CommonObjs = CommonObj.transform;
        }else{
            CommonObj = GameObject.Find("CommonObjects");
            if(CommonObj != null){
                CommonObjs = CommonObj.transform;
            }
        }      
    }
    
}
