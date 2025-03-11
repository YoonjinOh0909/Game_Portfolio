using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : OnOffBlock.cs
    * Desc : On/Off 블록의 변화를 담당하는 On/Off Button에서 작동하는 코드.
    *        
    *
    & Functions 
    &   [public]
    &   : SetSpriteOnOff(int)                       - 버튼의 이미지 변경 및 애니메이션 실행
    &   : SetOnOffBlockGroundsActive()              - On/Off 블록들을 상태에 따른 변경.
    &   : SetIsonValueForGoRightBefore(int)         - 뒤로가기(Undo)에 따른 상태 변경.
    &   : GetIsOnValue()                            - isOn return
    &   : SetIsOnValue(int)                         - set isOn
    &
    &   [private]
    &   : InitState()                               - 변수 초기화
    &   : ChangeStatusAll()                         - 모든 On/Off Button에 변화한 상태를 저장.
    &   : ChangeIsChangeStatus()                    - IsChangeFalse의 Invoke를 위한 함수
    &   : IsChangeFalse()                           - isChange에 False 저장.
    &   : ChangeAllBtn(int)                         - 모든 On/Off Button에 변화한 상태에 따른 상태 변경.
    *
*/

public class OnOffBlock : MonoBehaviour
{
    [SerializeField]
    private int isOn;

    [SerializeField]
    private GameObject[] switchGrounds;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private OnOffBlock[] allOnOffBtn;

    public bool isChange;
    
    void Start()
    {
        InitState();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !isChange){
            ChangeStatusAll();
            isOn = (isOn == 0 ? 1 : 0);
            ChangeAllBtn(isOn);
            SetOnOffBlockGroundsActive();
        }
    }

    private void InitState(){
        isOn = 0;
        isChange = false;
        switchGrounds = GameObject.FindGameObjectsWithTag("OnOffBlock");
        allOnOffBtn = FindObjectsOfType<OnOffBlock>();
        SetOnOffBlockGroundsActive();
    }

    private void ChangeStatusAll(){
        for(int i = 0; i < allOnOffBtn.Length; i++){
            allOnOffBtn[i].isChange = true;
            allOnOffBtn[i].ChangeIsChangeStatus();
        } 
    }

    private void ChangeIsChangeStatus(){
        Invoke("IsChangeFalse", 0.2f);
    }

    private void IsChangeFalse(){
        isChange = false;
    }
    private void ChangeAllBtn(int isOntemp){
        for(int i = 0; i < allOnOffBtn.Length; i++){
            allOnOffBtn[i].SetSpriteOnOff(isOntemp);
            allOnOffBtn[i].SetIsOnValue(isOntemp);
        }
    }
    public void SetSpriteOnOff(int temp){
        bool active = (temp == 0 ? true : false);
        anim.SetBool("isOn", active);
    }

    public void SetOnOffBlockGroundsActive(){
        foreach(GameObject eachBlock in switchGrounds){
            eachBlock.GetComponent<OnOffBlockGround>()?.SetGroundOnOff(isOn);
        }
    }

    public void SetIsonValueForGoRightBefore(int temp){
        isOn = temp;
        ChangeStatusAll();
        ChangeAllBtn(isOn);
        SetOnOffBlockGroundsActive();
    }
    public int GetIsOnValue(){
        return isOn;
    }

    public void SetIsOnValue(int tempisON){
        isOn = tempisON;
    }

}
