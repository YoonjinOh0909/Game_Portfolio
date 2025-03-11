using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : TimeBroken_Anim.cs
    * Desc : 타이머 블록의 상태에 따른 애니메이션을 조절.
    *
    & Functions 
    &   [public]
    &   : SetTBInfo(TBInfoForME)        - 타이머 블록의 TBInfoForME 설정
    &   : GetTBInfo()                   - TBInfoForME 변수 리턴
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    &   : CheckAnimTime()               - 타이버 블록이 사라지고, 나타나는 애니메이션 시간 계산
    *
*/

public class TimeBroken_Anim : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private float animTime;

    [SerializeField]
    private float brokenTime;

    [SerializeField]
    private float animTimeFormotion;

    [SerializeField]
    private float brokenTimeFormotion;

    [SerializeField]
    private bool isTouched;

    [SerializeField]
    private bool isBroken;
    
    [SerializeField]
    private Animator thisAnimator;

    private void OnEnable() {
        InitState();       
    }

    void FixedUpdate() {
        CheckAnimTime();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player") || (12 < other.gameObject.layer && other.gameObject.layer < 18)){
            // 12 < Layer < 18은 상자 객체를 뜻한다.
            if(!isTouched){
                isTouched = true;
                isBroken = false;
                anim.SetBool("isTouch", isTouched);
            }
            
        }
    }

    private void InitState(){
        isTouched = false;
        isBroken = false;
        animTime = 0f;
        animTimeFormotion = 0f;
        brokenTime = 0f;
        brokenTimeFormotion = 0f;
        bool ColorBlind = DataController.Instance.gameData.isColorFilterAssistant;
        anim?.SetBool("CB",ColorBlind);    
    }

    private void CheckAnimTime(){
        if(isTouched){
            if(isBroken){
                brokenTime += Time.deltaTime;
                brokenTimeFormotion = brokenTime/2.25f;
                if(brokenTime> 2.24f){
                    brokenTimeFormotion = 0.98f;
                    isTouched = false;
                    isBroken = false;
                    anim.SetBool("isTouch", isTouched);
                    anim.SetBool("isBroken", isBroken);
                    animTime = 0f;
                    brokenTime = 0f;
                }
                anim.SetFloat("BrokenTime", brokenTimeFormotion);
                brokenTimeFormotion = 0f;
            }else{
                animTime += Time.deltaTime;
                animTimeFormotion = animTime/2.25f;
                if(animTime> 2.24f){
                    animTimeFormotion = 0.98f;
                    isBroken = true;
                    anim.SetBool("isTouch", isTouched);
                    anim.SetBool("isBroken", isBroken);
                    animTime = 0f;
                    brokenTime = 0f;
                }
                anim.SetFloat("DelayTime", animTimeFormotion);
                animTimeFormotion = 0f;
            }
        }
    }
    public void SetTBInfo(TBInfoForME setTemp){
        animTime = setTemp.animTimeInfo;
        brokenTime = setTemp.brokenTimeInfo;
        animTimeFormotion = setTemp.animTimeFormotionInfo;
        brokenTimeFormotion = setTemp.brokenTimeFormotionInfo;
        isTouched = setTemp.isTouchedInfo;
        isBroken = setTemp.isBrokenInfo;
        anim.SetBool("isTouch", isTouched);
        anim.SetBool("isBroken", isBroken);
    }

    public TBInfoForME GetTBInfo(){
        TBInfoForME temp = new TBInfoForME();
        temp.animTimeInfo = animTime;
        temp.brokenTimeInfo = brokenTime;
        temp.animTimeFormotionInfo = animTimeFormotion;
        temp.brokenTimeFormotionInfo = brokenTimeFormotion;
        temp.isTouchedInfo = isTouched;
        temp.isBrokenInfo = isBroken;

        return temp;
    }
}
