using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/*
    * File : BoxTrigger.cs
    * Desc : 상자가 위 아래로 겹치거나 플레이어가 올라갔을 때 하나의 더미로 인식하는 기능 구현
    *
    & 
    &   [public]
    &   : SetisPull()                   - 상자를 당길 때 개수에 따라 이동 속도 제한
    &   : SetDelay()                    - 당기는 모션에 생기는 딜레이 여부 설정
    &   : SetisPullDelay()              - 당기는 모션 여부 변경
    &   : SetFalseisPull()              - 당기는 행동 끝났을 때
    &   : CalchildBoxcnt()              - 박스 위에 있는 박스들이 몇개인지 확인
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    *
*/

public class BoxPullTrigger : MonoBehaviour
{
    private Vector3 diff;

    [SerializeField]
    private bool isPull;

    private GameObject PlayerObj;

    private Player player;

    private Rigidbody2D boxrb;

    private Rigidbody2D playerrb;

    private bool isPulldelay;

    public Rigidbody2D[] childBoxrb;

    [SerializeField]
    private GameObject boxtrigger;

    [SerializeField]
    private int childBoxcnt;

    public bool canMove;

    private ItemScripts itemscripts;

    private Box box;

    RaycastHit2D hit;

    [SerializeField]
    private Rigidbody2D rigid2d;

    [SerializeField]
    private float diffVelBoxNPlayer = 1f;
    [SerializeField]
    private float additionalSpeed;

    [SerializeField]
    private float distanceWithPlayer;

    int layerMask;

    private void Start() {
        InitState();
    }

    void FixedUpdate() {
        player = player ?? PlayerObj.GetComponent<Player>();
        if(rigid2d.gravityScale > 0){
            this.gameObject.transform.eulerAngles = new Vector3(0,0,0);
            hit = Physics2D.BoxCast(transform.position + new Vector3(0,-1.09f,0), new Vector2(1.8f, 0.01f), 0f, Vector2.up * -1 * 0.1f, 1, layerMask);
        } else{
            this.gameObject.transform.eulerAngles = new Vector3(0,0,-180);
            hit = Physics2D.BoxCast(transform.position + new Vector3(0,2.09f,0), new Vector2(2f, 0.01f), 0f, Vector2.up * -1 * 0.1f, 1, layerMask);
        }
        
        if(hit.collider == null){
            box.NonFreezeY();
        }else{
            if(!hit.collider.CompareTag("Ground")){
                box.NonFreezeY();
            }else{
                if((hit.collider.gameObject.layer == this.gameObject.layer) || (hit.collider.gameObject.layer + 7 == this.gameObject.layer)){
                    box.NonFreezeY();
                }
            }
        }

        if(isPull && canMove){
            if(player.MoveChecker()){
                childBoxrb = gameObject.GetComponentsInChildren<Rigidbody2D>();
                if(distanceWithPlayer < Vector3.Distance(PlayerObj.gameObject.transform.position, transform.position)){
                    additionalSpeed = 0.15f;
                }else{
                    additionalSpeed = 0;
                }
                boxrb.velocity = new Vector2(PlayerObj.gameObject.GetComponent<Rigidbody2D>().velocity.x, 0) * (diffVelBoxNPlayer + additionalSpeed);

                foreach(Rigidbody2D rb in childBoxrb.Skip(1)){
                    rb.velocity = new Vector2(boxrb.velocity.x, 0);
                }
                
            }else{
                if(childBoxrb != null){
                    foreach(Rigidbody2D rb in childBoxrb){
                        rb.velocity = new Vector2(0,0);
                    }
                    childBoxrb = null;
                }
                
            }
                
        }

    }   
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            PlayerObj = other.gameObject;
            playerrb = PlayerObj.gameObject.GetComponent<Rigidbody2D>();
            distanceWithPlayer = Vector3.Distance(PlayerObj.gameObject.transform.position, transform.position);
            CalchildBoxcnt();
        }
    }
    private void InitState(){
        boxrb = this.gameObject.GetComponent<Rigidbody2D>();
        itemscripts = this.gameObject.GetComponent<ItemScripts>();
        diff = new Vector3(0,0,0);
        isPull = false;
        isPulldelay = false;
        canMove = false;
        box = this.gameObject.GetComponent<Box>();

        playerrb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        layerMask = (1<<0)+(1<<6) + (1<<7) + (1<<8) + (1<<9) + (1<<10) + (1<<29);
    }
    public void SetisPull(){
        if(!isPulldelay){
            isPull = !isPull;
            player = player ?? PlayerObj.GetComponent<Player>();
            canMove = (player.GetgetBoxobj() == this.gameObject);
            if(isPull){
                boxtrigger.gameObject.transform.localScale = new Vector3(1.05f,1,0);
                /*
                    상자 1개 : 7.4 ~ 7.5
                    상자 2개 : 4.8 ~ 4.9
                    상자 3개 : 2.1 ~ 2.2
                    상자 4개 이상 : 0
                */
                float playerSpeed = (10f - 2 * (childBoxcnt + 1)) * 0.1f;
                player.SetSpeedByBox(playerSpeed);
                player.pullingBoxes.Add(this.gameObject);
            }else{
                boxtrigger.gameObject.transform.localScale = new Vector3(0.7f,1,0);
                SetFalseisPull();
            }
            
        }
        
        isPulldelay = true;
        Invoke("SetDelay", 0.3f);
    }

    public void SetDelay(){
        isPulldelay = false;
    }
    public void SetisPullDelay(){
        isPull = !isPull;
    }
    public void SetFalseisPull(){
        if(isPull){
            itemscripts.forBoxPushState();
        }
        isPull= false;
        player = player ?? PlayerObj.GetComponent<Player>();
        boxtrigger.gameObject.transform.localScale = new Vector3(0.7f,1,0);
        
        
        player.pullingBoxes.Remove(this.gameObject);
        itemscripts.SetIspushFalse();

        if(player.pullingBoxes.Count <= 0) {
           player.SetSpeedByBox(1); 
        }

        canMove = false;
    }

    public void CalchildBoxcnt(){
        Rigidbody2D[] temprb = gameObject.GetComponentsInChildren<Rigidbody2D>();
        childBoxcnt = temprb.Length;
    }
}
