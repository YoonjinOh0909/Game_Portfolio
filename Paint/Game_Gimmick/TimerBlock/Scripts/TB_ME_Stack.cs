using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
    * File : TB_ME_Stack.cs
    * Desc : 전체 타이머 블록의 Undo(뒤로가기) 기능 구현
    *
    & Functions 
    &   [public]
    &   : InitTBStack()                - 타이머 블록 스택 초기화. GorightBefore.cs의 InitStartState()에서 실행.
    &   : PushState()                  - 타이머 블록 스택 Push
    &   : PopState()                   - 타이머 블록 스택 Pop
    &
    &   [private]
    &   : SetGRB()                     - Gorightbefore 객체 설정.
    *
*/
public class TB_ME_Stack : MonoBehaviour, GimmickPush
{
    public TBInfoForME[] ary;
    public GameObject[] objectAry;
    public TimeBroken_Anim[] scAry;
    int childCnt;
    public Stack<TBInfoForME[]> stack = new Stack<TBInfoForME[]>();
    private GoRightBefore goRightBefore;

    private void Awake() {
        SetGRB();
    }

    private void SetGRB(){
        goRightBefore = GetComponent<GoRightBefore>();
    }

    public void InitTBStack(){
        objectAry = GameObject.FindGameObjectsWithTag("TimeBrokenME");
        childCnt = objectAry.Length;

        ary = new TBInfoForME[childCnt];
        scAry = new TimeBroken_Anim[childCnt];
        
        goRightBefore.SetisTBinfo(childCnt > 0);
        

        for(int i = 0; i < childCnt; i++) {
            scAry[i] = objectAry[i].transform.GetChild(0).GetComponent<TimeBroken_Anim>();
            ary[i] = scAry[i].GetTBInfo();
        }

        stack.Push(ary);
    }

    
    public void PushState(){
        TBInfoForME[] tempary = new TBInfoForME[childCnt];

        for(int i = 0; i < childCnt; i++) {
            tempary[i] = scAry[i].GetTBInfo();
        }

        stack.Push(tempary);
    }

    public void PopState(){
        childCnt = objectAry.Length;

        if(stack.Count > 1){
            var popstack = stack.Pop();
            TBInfoForME[] temp = new TBInfoForME[popstack.Length];
            temp = popstack;

            for(int i = 0; i < popstack.Length; i++) { 
                scAry[i].SetTBInfo(temp[i]);
            }
            
        }else{
            if(stack.Count != 0){
                var popstack = stack.Peek();
                TBInfoForME[] temp = new TBInfoForME[popstack.Length];
                temp = popstack;

                for(int i=0; i < popstack.Length; i++){ 
                    scAry[i].SetTBInfo(temp[i]);
                }
            
            }
        }
    }

}
