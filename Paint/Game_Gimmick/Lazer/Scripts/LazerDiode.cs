using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : LazerDiode.cs
    * Desc : 레이저 기능 구현.
    *
    & Functions 
    &   [public]
    &   : SetIsActive()                         - 레이저의 작동 여부 설정
    &   : SetLazerBtn()                         - 레이저와 연결된 버튼 설정
    &   : ResetAfterGRB()                       - 뒤로가기(Undo) 기능 이후 초기화
    &
    &   [private]
    &   : InitState()                           - 변수 초기화
    &   : CheckLazer()                          - 플레이어, 땅 등 케이스에 맞게 설정되는 Lazer 기능
    *
*/
public class LazerDiode : MonoBehaviour
{
    [SerializeField]
    private LazerLine visualizerLine;

    [SerializeField]
    private Transform owner;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    public int lazerState;

    [SerializeField]
    public Sprite[] diodelist;

    [SerializeField]
    public Sprite[] diodelist_ColorBlind;

    [SerializeField]
    public Sprite[] offdiodelist;

    [SerializeField]
    public Sprite[] offdiodelist_ColorBlind;

    public bool IsMapEditor;

    private GoRightBefore GMGoRightBefore;

    [SerializeField]
    private Vector2 sizeVector = new Vector2(1,1);
    [SerializeField]
    private Vector2 sizeVector_confrim = new Vector2(1,1);

    [SerializeField]
    private float angle = 45f;

    [SerializeField]
    private bool isPlayerFirstin = false;

    [SerializeField]
    private bool changeOnPlayerin = false;

    private GameObject lazerbtn = null;

    private bool isUD = false; //위 or 아래에서 쏘면 true, 좌우로 쏘면 false
    
    void Awake()
    {
        InitState();
    }

    private void Start() {
        GMGoRightBefore = GameObject.FindWithTag("GameManager").GetComponent<GoRightBefore>();
        sizeVector = new Vector2(0.25f,0.25f);
        sizeVector_confrim = new Vector2(0.1f,0.1f);
        angle = 45f;
    }

    void FixedUpdate()
    {
        CheckLazer();        
    }
    
    private void InitState(){
        this.gameObject.layer = lazerState;
        this.gameObject.transform.parent.gameObject.transform.GetChild(1).gameObject.layer = lazerState;
        if(lazerState == 5){
            this.gameObject.layer = 10;
            lazerState = 10;
            this.gameObject.transform.parent.gameObject.transform.GetChild(1).gameObject.layer = 10;
        }
        bool isColorFilterAssistant = DataController.Instance.gameData.isColorFilterAssistant;
        GetComponent<SpriteRenderer>().sprite = 
            isColorFilterAssistant ? diodelist_ColorBlind[lazerState-6] : diodelist[lazerState-6];
        visualizerLine.SetLazerState(lazerState);
    }

    private void CheckLazer(){
        int layerMask =  (1 << this.gameObject.layer) + (1 << LayerMask.NameToLayer("Ignore Raycast")) + (1 << LayerMask.NameToLayer("Items")) + (1 << LayerMask.NameToLayer("MapEditor")) + (1 << this.gameObject.layer + 7) + (1 << LayerMask.NameToLayer("BOX")) + (1 << 23) + (1 << 24) + (1 << 25) + (1 << 26) + (1 << 27) + (1 << LayerMask.NameToLayer("CanNotOnAir"));
        int confirmLayerMask = (1 << LayerMask.NameToLayer("Ignore Raycast")) + (1 << LayerMask.NameToLayer("Items")) + (1 << LayerMask.NameToLayer("MapEditor")) + (1 << this.gameObject.layer + 7) + (1 << LayerMask.NameToLayer("BOX")) + (1 << 23) + (1 << 24) + (1 << 25) + (1 << 26) + (1 << 27) + (1 << LayerMask.NameToLayer("CanNotOnAir"));
        
        layerMask = ~layerMask;
        confirmLayerMask = ~confirmLayerMask;

        if(isActive){
            float parentRotate = this.gameObject.transform.parent.transform.eulerAngles.z;
            Vector2 direction;
        
            direction = new Vector2( -Mathf.Sin(parentRotate* Mathf.Deg2Rad), Mathf.Cos(parentRotate * Mathf.Deg2Rad) ).normalized;
            
            isUD = parentRotate % 180 == 0 ? true : false;

            // RatcasHit2D hit는 자신의 색상과 같은 색상은 인식하지 않는다. 
            RaycastHit2D hit = Physics2D.BoxCast(owner.position, sizeVector, angle, direction, 1000, layerMask);
            GameObject hitgameObj = hit.collider.gameObject;

            // RatcasHit2D hit_confirm는 자신의 색상과 같은 색상도 인식한다. 레이저를 쏘는 동안에 player가 있는지 확인. 보통 뒤로가기 했을 때의 상황에서 색상 버그가 일어나지 않도록 하기 위해 추가된 변수.
            RaycastHit2D hit_confirm = Physics2D.BoxCast(owner.position, sizeVector, angle, direction, 1000, confirmLayerMask);
            GameObject hit_confirmgameObj = hit_confirm.collider.gameObject;
            
            if(hit && hit_confirm){
                if((hitgameObj.CompareTag("Player") && hit_confirmgameObj.CompareTag("Player")) || (hitgameObj.CompareTag("Player") && hit_confirmgameObj.CompareTag("Ground") && hit_confirmgameObj.layer == gameObject.layer)){
                    int prevColor =  hitgameObj.GetComponent<Player>().ColorGetter();
                    bool prevIsTubeSpecial = hitgameObj.GetComponent<Player>().isTubeSpecial;
                
                    if(hitgameObj.GetComponent<Player>().ColorGetter() != lazerState && DataController.Instance.gameData.canLazerInteract && !changeOnPlayerin && !isPlayerFirstin){
                        float grbX = 0;
                        float grbY = 0;

                        if(isUD){ // 일반적인 아래서 위로 쏘는 레이저, or 위에서 아래
                            if(Mathf.Abs(hit.point.x - transform.parent.transform.position.x) < 1e-4){
                                grbX = 0;                                
                            }else{
                                grbX = hit.point.x < transform.parent.transform.position.x ? -1.4f : 1.4f;
                            }
                            
                        }else{ // 좌우로 쏘는 레이저
                            //위에서 떨어지면서 레이저를 맞고나서 뒤로가기를 진행하면, 바로 떨어지면서 색상이 변하면서 무한 반복에 빠지게 된다. 따라서 약간의 차이를 둬서 빠르게 뒤로가기를 누르면 그 이전 상태로 벗어날 수 있도록 상태를 조정해둔다.
                            grbY = hit.point.y < transform.parent.transform.position.y ? -0.5f : 1.75f; 

                            // 왼쪽에서 오는 레이저를 맞을 때와 오른쪽에서 오는 레이저를 맞을 때의 오차가 다르다. 따라서 보정을 해준 것.
                            grbX = hit.point.x < transform.parent.transform.position.x ? -0.79f : 0.59f; 

                        }

                        GMGoRightBefore.CommonPushState
                        (
                            new PushStateParam
                            {
                                tmpPushstate = PushState.Lazer,
                                interactedObj = this.gameObject,
                                playerposition = new Vector3(hit.point.x + grbX, hit.point.y + grbY - 0.5f, 0),
                                prePlayerColor = prevColor,
                                markerSpecialStatus = prevIsTubeSpecial
                            }
                        );
                        changeOnPlayerin = true;
                        hitgameObj.GetComponent<Player>().ColorSetter(lazerState,false,true);
                    }

                    if(hit_confirmgameObj.CompareTag("Ground")){
                        float x_val = isUD ? transform.parent.transform.position.x : hit.point.x;
                        float y_val = isUD ? hit.point.y : transform.parent.transform.position.y;
                        visualizerLine.Play(owner.position, new Vector2(x_val,y_val));
                                                
                        if(hitgameObj.CompareTag("Player")){
                            isPlayerFirstin = true;
                        }else if(hitgameObj.CompareTag("Ground")){
                            isPlayerFirstin = false;
                            changeOnPlayerin = false;
                        }
                    }
                    
                }
                else if(hitgameObj.CompareTag("Ground")){
                    float x_val = isUD ? transform.parent.transform.position.x : hit.point.x;
                    float y_val = isUD ? hit.point.y : transform.parent.transform.position.y;
                    visualizerLine.Play(owner.position, new Vector2(x_val,y_val));
    
                    if(hit_confirmgameObj.CompareTag("Player")){
                        isPlayerFirstin = true;
                    }else if(hit_confirmgameObj.CompareTag("Ground") ){
                        isPlayerFirstin = false;
                        changeOnPlayerin = false;
                    }
                    
                }
                
            }
            else{
                visualizerLine.Stop();
            }
            
        }else{
            visualizerLine.Stop();
        }
    }
    public void SetIsActive(bool temp){
        isActive = temp;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        int index = lazerState-6;
        bool isColorFilterAssistant = DataController.Instance.gameData.isColorFilterAssistant;

        spriteRenderer.sprite = isActive
            ? (isColorFilterAssistant ? diodelist_ColorBlind[index] : diodelist[index])
            : (isColorFilterAssistant ? offdiodelist_ColorBlind[index] : offdiodelist[index]);
        
        ResetAfterGRB();
    }

    public void SetLazerBtn(GameObject btn){
        lazerbtn = btn;
    }
    public void ResetAfterGRB(){
        isPlayerFirstin = false;
        changeOnPlayerin = false;
    }
    
}
