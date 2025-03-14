using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
    * File : Box.cs
    * Desc : Box가 덜컹거리지 않고 부드럽게 이동할 수 있게 제한을 두는 기능 구현
    *
    & Functions 
    &   [public]
    &   : FreezeY()                     - 상자가 가만히 있거나 좌우로 움직일 때 Y의 움직임을 없애주는 기능
    &   : NonFreezeY()                  - 상자가 떨어질 때 Y 고정 해제 기능
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    *
*/
public class Box : MonoBehaviour
{
    Rigidbody2D m_Rigidbody;

    void Start()
    {
        InitState();
    }
    private void InitState(){
        m_Rigidbody = this.GetComponent<Rigidbody2D>();

        //Y축 고정과 Z축 고정
        m_Rigidbody.constraints = RigidbodyConstraints2D.None; 
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    public void  FreezeY(){ 
        m_Rigidbody = m_Rigidbody ?? this.GetComponent<Rigidbody2D>();
        m_Rigidbody.constraints = ~RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    } 

    public void NonFreezeY(){ 
        m_Rigidbody = m_Rigidbody ?? this.GetComponent<Rigidbody2D>();
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation; //Z 축 고정
    }
}
