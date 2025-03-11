using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : OnOffBlockInfosc.cs
    * Desc : GimmickPush 인터페이스의 구현 코드
    *        On / Off 블록의 뒤로가기(Undo) 기능 구현
    *
    & Functions 
    &   [public]
    &   : PushState()                       - On/Off 블록 스택 Push
    &   : PopState()                        - On/Off 블록 스택 Pop
    &
    &   [private]
    &   : InitState()                       - 변수 초기화
    *
*/

public class OnOffBlockInfosc : MonoBehaviour, GimmickPush
{
    private OnOffBlock onoffblock;
    public Stack<int> stack = new Stack<int>();
    void Start()
    {
        InitState();
    }

    private void InitState(){
        onoffblock = GetComponent<OnOffBlock>();
        stack.Push(onoffblock.GetIsOnValue());
    }
    public void PushState(){
        int tempVal = onoffblock.GetIsOnValue();
        stack.Push(tempVal);
    }

    public void PopState()
    {
        if(stack.Count > 1)
        {
            var tempState = stack.Pop();
            int temp;
            temp = tempState;
            onoffblock.SetIsonValueForGoRightBefore(temp);
        }
        else
        {
            if(stack.Count != 0)
            {
                var tempState = stack.Peek();
                int temp;
                temp = tempState;            
                onoffblock.SetIsonValueForGoRightBefore(temp);
            }
        }          
    }
}
