using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : CannonShoot.cs
    * Desc : 비눗방울(Cannon과 동일) 기능 구현
    *
    & Functions 
    &   [public]
    &   : SetBulletStateOnCannonShoot(int)      - Cannon과 연결된 불통에 의해 재설정
    &   : SetLazerBtn()                         - 레이저와 연결된 버튼 설정
    &   : ResetAfterGRB()                       - 뒤로가기(Undo) 기능 이후 초기화
    &
    &   [private]
    &   : InitState()                           - 변수 초기화
    &   : CannonInitState()                     - Cannon 변수 초기화
    &   : delay()                               - Delay Invoke를 위한 함수
    &   : DestroyBullet()                       - 종속된 모든 방울(대포알) 제거
    *
*/
public class CannonShoot : MonoBehaviour
{
    [SerializeField] private float delaytime;
    [SerializeField] GameObject Bullet;
    [SerializeField] bool Delay;

    [SerializeField] private int bulletState;

    [SerializeField] private int originState;

    private Transform playerObj;

    [SerializeField] private CircleCollider2D circleCol;

    [SerializeField] private GameObject rangeSprite;

    [SerializeField] SpriteRenderer spriteRenderer; //RangeCollider

    [SerializeField] private Sprite[] cannonSprite;

    [SerializeField] private Sprite[] cannonSprite_ColorBlind;

    [SerializeField] private Sprite[] cannonRange;

    [SerializeField] private Sprite[] cannonRange_ColorBlind;

    [SerializeField] private SpriteRenderer cannonRenderer;

    private string[] LayerName = {"Cannon_R", "Cannon_G", "Cannon_B", "Cannon_Y", "Cannon"};

    private GoRightBefore GMGoRightBefore;
    private void Start() {
        InitState();
        CannonInitState();
    }
    
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Delay == false && collision.CompareTag("Player") && bulletState != 5) {
            Delay = true;
            Invoke("delay", delaytime);

            var obj = Instantiate(Bullet, this.GetComponent<Transform>());
            obj.gameObject.GetComponent<CannonPhysic>().SetBulletState(bulletState);
            obj.gameObject.GetComponent<CannonPhysic>().GRB = GMGoRightBefore;
            obj.gameObject.GetComponent<Transform>().rotation =  Quaternion.identity;
        }

    }
    private void InitState(){
        originState = bulletState;

        GMGoRightBefore = GameObject.FindWithTag("GameManager").GetComponent<GoRightBefore>();
        
        Delay = false;
    }
    private void CannonInitState(){
        circleCol.radius = rangeSprite.transform.localScale.x/2;
        if(playerObj == null){
            playerObj = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        if(5 > bulletState || bulletState > 10){
            bulletState = (bulletState == 0) ? 5 : 6;
        }
            
        int colorIndex = ((bulletState-1) % 5);
        bool isColorFilterAssistant = DataController.Instance.gameData.isColorFilterAssistant;
        
        spriteRenderer.sprite = isColorFilterAssistant ? cannonRange_ColorBlind[colorIndex] : cannonRange[colorIndex];
        cannonRenderer.sprite = isColorFilterAssistant ? cannonSprite_ColorBlind[colorIndex] : cannonSprite[colorIndex];

        spriteRenderer.sortingLayerName = "Default";
        cannonRenderer.sortingLayerName = LayerName[colorIndex];

    }

    private void delay() {
        Delay = false;
    }

    public void SetBulletStateOnCannonShoot(int temp){
        
        if(temp == 0 || temp == 5){
            bulletState = originState;
        }else{
            bulletState = temp;
        }
        
        CannonInitState();
        DestroyBullet();
    }

    private void DestroyBullet(){
        Transform[] child = GetComponentsInChildren<Transform>();

        for(int i =1; i < child.Length; i++){
            Destroy(child[i].gameObject);
        }
    }
}
