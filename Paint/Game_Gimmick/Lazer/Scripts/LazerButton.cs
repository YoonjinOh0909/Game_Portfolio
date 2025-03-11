using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    * File : LazerButton.cs
    * Desc : GimmickPush 인터페이스의 구현 코드
    
    & Functions 
    &   [private]
    &   : FindLazerDiode()              - LazerBtn 과 연결된 Lazer 확인
    &   : InitState()                   - 변수 초기화
    &   : CheckLazerBtn()               - Btn 안에 Player 혹은 상자 존재 여부 확인
    *
*/

public class LazerButton : MonoBehaviour
{
    RaycastHit2D hit;

    [SerializeField]
    private LazerDiode lazerDiode;

    [SerializeField]
    private Sprite[] buttonSprite;

    private bool isPush;

    private SpriteRenderer spriteRenderer;
    private int findCnt;

    void Start()
    {
        InitState();
    }

    void Update() {
        if(lazerDiode == null){
            FindLazerDiode();
        }
    }

    private void FindLazerDiode(){
        LineAble tempLineAble = this.transform.parent.transform.parent.GetComponent<LineAble>();
        LineRendererConnector Temp = (tempLineAble.Line != null) ? tempLineAble.Line.GetComponent<LineRendererConnector>() : null;

        if(Temp == null){
            findCnt++;
            if(findCnt > 10){
                //10번정도 찾았는데 없으면 사용하지 않는 것으로 판단하여 SetActive False를 한다.
                this.gameObject.SetActive(false);
            }
            return;
        }

        for(int i=0;i<Temp.Positions.Count;i++)
        {
            if(Temp.Positions[i]!=this.transform.parent.transform.parent.GetChild(0).transform)
            {
                lazerDiode=Temp.Positions[i].parent.GetChild(1).GetChild(0).gameObject.GetComponent<LazerDiode>();
                lazerDiode.SetLazerBtn(this.gameObject);
            }
        }
        
    }

    void FixedUpdate()
    {
        CheckLazerBtn();
    }

    private void InitState(){
        findCnt = 0;
        isPush = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = !DataController.Instance.gameData.isColorFilterAssistant ? buttonSprite[0] : buttonSprite[2];
    }

    private void CheckLazerBtn(){
        int layerMask = (1<<0)+(1<<6) + (1<<7) + (1<<8) + (1<<9) + (1<<10);

        var rigidbody = GetComponent<Rigidbody2D>();

        hit = Physics2D.BoxCast(transform.position + new Vector3(0,0.3f,0), new Vector2(0.8f, 0.01f), 0f, Vector2.up * -1 * 0.01f, 0.3f, layerMask);
        BoxCastDrawer.Draw(hit, transform.position + new Vector3(0,0.3f,0), new Vector2(0.8f, 0.01f), 0f, Vector2.up * -1 * 0.01f, 0.3f);
        
        bool isColorFilterAssistant = DataController.Instance.gameData.isColorFilterAssistant;
        if(hit.collider == null){
            lazerDiode?.SetIsActive(true);
            if(isPush){
                spriteRenderer.sprite = isColorFilterAssistant ? buttonSprite[2] : buttonSprite[0];
                isPush = false;
            }
        }else{
            if(hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Player")){
                lazerDiode?.SetIsActive(false);
                if(!isPush){
                    spriteRenderer.sprite = isColorFilterAssistant ? buttonSprite[3] : buttonSprite[1];
                    isPush = true;
                }
            }
        }
    }
}
