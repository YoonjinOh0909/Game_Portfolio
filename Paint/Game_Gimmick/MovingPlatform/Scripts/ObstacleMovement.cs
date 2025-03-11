using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/*
    * File : ObstacleMovement.cs
    * Desc : 구름(Moving Platform) 왕복 기능 구현
    *
    & Functions 
    &   [public]
    &   : SetStop()                         - Set isStop
    &   : GetInitPosReturn()                - 현재 이동 정보 return
    &   : GetNextPosReturn()                - 다음 이동 정보 return
    &   : SetNextPos(MoveObstacleInfo)      - set 다음 이동 정보
    &
    &   [private]
    &   : InitState()                       - 변수 초기화
    &   : UpdateMove()                      - Platform 움직임 확인.
    &   : Move()                            - start에서 end로 이동
    &   : FindEndPos()                      - MapEditor 특성상 시작 부분과 끝부분의 오브젝트 존재. 따라서 연결된 끝부분 오브젝트를 확인하는 코드
    &   : ChangeDestination()               - 시작과 끝을 바꿔주면서 왕복을 하는 코드
    *
*/

public class ObstacleMovement : MonoBehaviour
{
    [Header("Start to Finish")]
    [SerializeField]
    private Vector3 posStart;

    [SerializeField]
    private Vector3 posArrival;
    
    [SerializeField]
    private Vector3 posNext;
    
    [Header("Object")]

    [SerializeField]
    private Transform CommonObjs;
    private Transform parentTransform;
    private Transform childTransform;
    private Transform arrivalTransform;

    [Header("Options")]
    [SerializeField]
    private float speed;
    public float smoothTime = 0.7f;
    private bool isDelay = true;
    private Vector3 velocity = Vector3.zero;
    public bool DelaySet = true;
    public float delay = 3f;
    private float time;
    private float distance;
    private bool isStop = false;
    public Vector2 dir;

    [Header("ForMapEditor")]
    [SerializeField] private Transform platform;
    [SerializeField] private bool isRun;
    [SerializeField] private Transform posArrivalTransform;
    private int findCnt;

    void Start()
    {
        InitState();
    }

    void FixedUpdate()
    {
        UpdateMove();
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            parentTransform = other.transform.parent;
            other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            // 충돌 방향에 따라서 플레이어가 무빙 플랫폼에 귀속되지 않도록 처리 (양 옆에 붙어있는 경우)
            ContactPoint2D contact = other.contacts[0];
            Vector2 pos = contact.point;
            Vector2 normal = contact.normal;

            // 플레이어가 옆에서 부딪히면 SetParent 되지 않도록
            if(Mathf.Abs(normal.x) > Mathf.Epsilon)
            {
                other.transform.SetParent(CommonObjs);
            }
            else
            {
                other.transform.SetParent(transform);
            }

        }
        else if (other.gameObject.name.Contains("Box"))
        {
            if(other.gameObject.layer - 7 != this.gameObject.layer){
                // 충돌 방향에 따라서 박스가 무빙 플랫폼에 귀속되지 않도록 처리 (양 옆에 붙어있는 경우)
                ContactPoint2D contact = other.contacts[0];
                Vector2 pos = contact.point;

                // pos를 로컬 좌표로 변환
                Vector2 localPoint = transform.InverseTransformPoint(pos);

                // 박스 - 무빙 플랫폼이 옆에서 충돌이 일어났고, 부딪힌 방향과 무빙 플랫폼의 이동 방향이 반대이면 SetParent(null)
                if((localPoint.x >= 2.4f && localPoint.x <= 2.55f && localPoint.x != 2.5f && dir.x < 0.0f || localPoint.x >= -2.55f && localPoint.x <= -2.4f && localPoint.x != -2.5f && dir.x > 0.0f))
                {
                    other.gameObject.transform.SetParent(null);
                }
                else
                {                    
                    other.gameObject.transform.SetParent(transform);
                }
            }
        }
        
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(CommonObjs);
            other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (other.gameObject.name.Contains("Box")) {
            other.gameObject.transform.SetParent(null);
        }
        
    }

    private void InitState(){
        posNext = new Vector3(1,0,0);
        
        platform = this.gameObject.transform;
        posStart = platform.parent.transform.position; 
        CommonObjs = GameObject.Find("Layers").transform;
        isRun = false;
        
        posNext = posArrival;
        distance = Vector3.Distance(posStart, posArrival);
        time = 0f;
        DelaySet = true;
    }

    private void UpdateMove(){
        if(posArrivalTransform == null){
            FindEndPos();
        }
        if(isRun){
            if (isStop)
                return;

            if (!isDelay)
            {
                Move();
                
                if (Vector3.Distance(platform.position, posNext) <= 0.1)
                {
                    ChangeDestination();

                    if (DelaySet)
                    {
                        time = 0f;
                        isDelay = true;
                    }
                } 
                
            }
            else
            {
                if (time >= delay)
                {
                    isDelay = false;
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
        }
    }

    private void Move()
    {
        Vector3 prePos = platform.position;
        platform.position = Vector3.MoveTowards(platform.position, posNext, speed * Time.deltaTime);
        dir = (posNext - transform.position).normalized;
    }

    private void FindEndPos(){

        LineRendererConnector Temp = this.transform.parent.transform.parent.GetComponent<LineAble>()?.Line.GetComponent<LineRendererConnector>();
        
        if(Temp == null){
            findCnt++;
            if(findCnt > 10){
                //10번정도 찾았는데 없으면 사용하지 않는 것으로 판단하여 SetActive False를 한다.
                Destroy(GetComponent<ObstacleMovement>());
            }
            return;
        }

        for(int i=0;i<Temp.Positions.Count;i++)
        {
            if(Temp.Positions[i]!=this.transform.parent.transform.parent.GetChild(0).transform)
            {
                posArrivalTransform=Temp.Positions[i].parent.GetChild(1).gameObject.transform;
                posNext = posArrivalTransform.position;
                distance = Vector3.Distance(posStart, posArrivalTransform.position);
                isRun = true;
                GameObject.FindWithTag("GameManager").transform.GetComponent<MovingObjInfo>()?.InitMovStack();
            }
        }

    }
    private void ChangeDestination()
    {
        posNext = (posNext != posStart ? posStart : posArrivalTransform.position);
    }

    public void SetStop(bool stop) {
        isStop = stop;
    }

    public MoveObstacleInfo GetInitPosReturn(){
        MoveObstacleInfo tempMoveInfo = new MoveObstacleInfo();
        tempMoveInfo.posNext = this.posNext;
        tempMoveInfo.isDelay = this.isDelay;
        tempMoveInfo.nowPos = this.posStart;
        tempMoveInfo.stackTime = 0f;

        return tempMoveInfo;
    }
    
    public MoveObstacleInfo GetNextPosReturn(){
        MoveObstacleInfo tempMoveInfo = new MoveObstacleInfo();
        tempMoveInfo.posNext = this.posNext;
        tempMoveInfo.isDelay = this.isDelay;
        tempMoveInfo.nowPos = this.platform.transform.position;
        tempMoveInfo.stackTime = this.time;

        return tempMoveInfo;
    }

    public void SetNextPos(MoveObstacleInfo tempInfo){
        this.posNext = tempInfo.posNext;
        this.isDelay = tempInfo.isDelay;
        platform.transform.position = tempInfo.nowPos;
        this.time =tempInfo.stackTime;
    }
}
