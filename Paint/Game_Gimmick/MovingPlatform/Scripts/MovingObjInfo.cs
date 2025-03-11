using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : MovingObjInfo.cs
    * Desc : GimmickPush 인터페이스의 구현 코드
    *        구름(MovingPlatform)의 뒤로가기(Undo) 기능 구현
    *
    & Functions 
    &   [public]
    &   : InitMovStack()                                  - MovingPlatform 변수 초기화
    &   : PushState()                                     - MovingPlatform 스택 Push
    &   : PopState()                                      - MovingPlatform 스택 Pop
    &   : PopOnGmToObstacleMovement(MoveObstacleInfo[])   - Pop 된 값으로 상태 적용
    &
    &   [private]
    &   : PushstackMoveInfo(MoveObstacleInfo[])           - 특정 MoveObstacleInfo 배열 push
    &   : ResetmovStack()                                 - stack 초기화
    *
*/
public class MovingObjInfo : MonoBehaviour, GimmickPush
{
    public MoveObstacleInfo[] ary;

    [SerializeField]
    int childCnt;

    [Header("ForMapEditor")]

    [SerializeField]
    private GameObject[] objectAry;

    [SerializeField]
    private GoRightBefore goRightBefore;

    public Stack<MoveObstacleInfo[]> stack = new Stack<MoveObstacleInfo[]>();

    private void Awake() {
        goRightBefore = GetComponent<GoRightBefore>();
    }
    private void Start() {
        InitMovStack();        
    }
    public void InitMovStack(){
        ResetmovStack();
        objectAry = GameObject.FindGameObjectsWithTag("MovingPlatformParent");
        childCnt = objectAry.Length;

        ary = new MoveObstacleInfo[childCnt];
        
        goRightBefore.SetisMovinginfo(childCnt > 0);       

        for(int i=0; i < childCnt; i++){
            ary[i] = objectAry[i].transform.GetChild(0).GetComponent<ObstacleMovement>().GetInitPosReturn();
        }
        PushstackMoveInfo(ary);
    }

    private void PushstackMoveInfo(MoveObstacleInfo[] tempStack){
        stack.Push(tempStack);
    }

    private void ResetmovStack(){
        stack = new Stack<MoveObstacleInfo[]>();
    }

    public void PushState(){
        MoveObstacleInfo[] tempary = new MoveObstacleInfo[childCnt];
        
        for(int i=0; i < childCnt; i++){
            if(objectAry[i] != null && objectAry[i].transform.GetChild(0).TryGetComponent<ObstacleMovement>(out ObstacleMovement obsPush)){
                tempary[i] = obsPush.GetNextPosReturn();
            }
            
        }
        
        PushstackMoveInfo(tempary);
    }

    public void PopState()
    {
        if(stack.Count > 1){
            var kk = stack.Pop();
            MoveObstacleInfo[] temp = new MoveObstacleInfo[kk.Length];
            temp = kk;
            PopOnGmToObstacleMovement(temp);
        }else{
            if(stack.Count != 0){
                var kk = stack.Peek();
                MoveObstacleInfo[] temp = new MoveObstacleInfo[kk.Length];
                temp = kk;
                PopOnGmToObstacleMovement(temp);
            }

        }       
        
    }

     public void PopOnGmToObstacleMovement(MoveObstacleInfo[] tempArrayInfo){
        
        for(int i=0; i < childCnt; i++){
            
            if(objectAry[i] != null && objectAry[i].transform.GetChild(0).TryGetComponent<ObstacleMovement>(out ObstacleMovement obs)){
                obs.SetNextPos(tempArrayInfo[i]);
            }
            
        }
        
    }
}
