using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/*
    * File : PenManager.cs
    * Desc : 플레이를 하면서 사용할 수 있는 낙서 기능 구현.
    *
    & 
    &   [public]
    &   : OnPointerDown(PointerEventData)         - 좌클릭일 때는 생성, 우클릭일 때는 제거
    &   : OnDrag(PointerEventData)                - 좌클릭일 때 끊기지 않고 생성, 우클릭일 때 끊기지 않고 제거
    &   : OnPointerUp()                           - 좌클릭일 때 새로운 라인을 생성.
    &
    &   [private]
    &   : CreateNewLine()                         - 세팅 값에 맞는 라인 생성
    *
*/

public class PenManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Color penColor = Color.black; // 기본 펜 색상은 검정색
    public Color colorPenColor;
    public float penSize = 0.1f; // 기본 펜 크기는 중간 크기
    private LineRenderer currentLine;
    private Vector3 lastPosition;

    [SerializeField]
    private Transform parentObject;

    [SerializeField]
    private GameObject doodleLine;

    private bool isDrag = false;

    [SerializeField]
    private float deleteDistance = 0.1f;

    [SerializeField]
    private float generateDistance = 0.1f;
    void Start()
    {
        CreateNewLine();
    }

    void CreateNewLine()
    {
        var lineGO = Instantiate(doodleLine, parentObject);

        currentLine = lineGO.GetComponent<LineRenderer>();
        currentLine.startWidth = penSize;
        currentLine.endWidth = penSize;

        currentLine.startColor = !DataController.Instance.gameData.isColorFilterAssistant ? colorPenColor : penColor;
        currentLine.endColor = !DataController.Instance.gameData.isColorFilterAssistant ? colorPenColor : penColor;

        currentLine.positionCount = 0; 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!DataController.Instance.gameData.isNowCreateMap && DataController.Instance.gameData.playMode != PlayMode.IDLE){
            if(eventData.button == PointerEventData.InputButton.Left){
                isDrag = false;

                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                currentPosition.z = 0;

                currentLine?.SetPosition(++currentLine.positionCount - 1, currentPosition);
                currentLine?.SetPosition(++currentLine.positionCount - 1, currentPosition);
                
                lastPosition = currentPosition;
                

            }else if(eventData.button == PointerEventData.InputButton.Right && !DataController.Instance.gameData.isSettingPanelOn){
                
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
                mousePos.z = 0;

                int childCnt = parentObject.transform.childCount;

                for (int j = 0; j < childCnt-1; j++)
                {
                    LineRenderer line = parentObject.transform.GetChild(j).gameObject.GetComponent<LineRenderer>();
                    for (int i = 0; i < line.positionCount; i++)
                    {
                        Vector3 linePoint = line.GetPosition(i);
                        if (Vector3.Distance(mousePos, linePoint) < deleteDistance)
                        {
                            Destroy(line.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!DataController.Instance.gameData.isNowCreateMap && DataController.Instance.gameData.playMode != PlayMode.IDLE){
            if(eventData.button == PointerEventData.InputButton.Left && !DataController.Instance.gameData.isSettingPanelOn){
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                currentPosition.z = 0;

                float distance = Vector3.Distance(lastPosition, currentPosition);
                if (distance > generateDistance) // 이동 거리가 일정 이상일 때만 선 추가
                {
                    isDrag = true;
                    
                    currentLine?.SetPosition(++currentLine.positionCount - 1, currentPosition);
                    lastPosition = currentPosition;
                }
            }
            else if(eventData.button == PointerEventData.InputButton.Right && !DataController.Instance.gameData.isSettingPanelOn){
                
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
                mousePos.z = 0;
            
                int childCnt = parentObject.transform.childCount;

                for (int j = 0; j < childCnt-1; j++) 
                {
                    LineRenderer line = parentObject.transform.GetChild(j).gameObject.GetComponent<LineRenderer>();
                    for (int i = 0; i < line.positionCount; i++)
                    {
                        Vector3 linePoint = line.GetPosition(i);
                        if (Vector3.Distance(mousePos, linePoint) < deleteDistance)
                        {
                            Destroy(line.gameObject);
                            return;
                        }
                    }
                }
            }
            else{
                if(isDrag){
                    isDrag = false;
                    LineRenderer destroyTryLine1 = currentLine;
                    CreateNewLine();
                }
                
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!DataController.Instance.gameData.isNowCreateMap && DataController.Instance.gameData.playMode != PlayMode.IDLE){
            if(eventData.button == PointerEventData.InputButton.Left){
                LineRenderer destroyTryLine1 = currentLine;
                CreateNewLine();
                isDrag = false;
            } 
        }
        
    }

}
