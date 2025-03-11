using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : LazerLine.cs
    * Desc : LineRenderer를 이용하여 레이저 표시.
    *
    & Functions 
    &   [public]
    &   : Play(Vector3, Vector3)                     - 시작점과 끝점 사이를 그려지는 기능
    &   : Stop()                                     - 레이저가 작동하지 않을 때 사라지도록 하는 기능
    &   : SetLazerState()                            - 레이저의 상태 설정
    &
    &   [private]
    &   : InitState(MoveObstacleInfo[])              - 변수 초기화
    *
*/
public class LazerLine : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private int lazerState;

    [SerializeField]
    private Material[] material;
    
    void Start()
    {
        InitState();
    }

    private void InitState(){
        lazerState = this.gameObject.transform.parent.GetChild(0).gameObject.GetComponent<LazerDiode>().lazerState;
        if(5 > lazerState || lazerState > 10){
            lazerState = 6;
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; 

        lineRenderer.material = !DataController.Instance.gameData.isColorFilterAssistant ? material[0] : material[1];
    }

    public void Play(Vector3 from, Vector3 to){
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, new Vector3(from.x , from.y, 0));
        lineRenderer.SetPosition(1, new Vector3(to.x, to.y, 0));
    }

    public void Stop(){
        lineRenderer.enabled = false;
    }

    public void SetLazerState(int temp){
        lazerState = temp;
    }
}
