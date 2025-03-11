using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : CannonPhysic.cs
    * Desc : CannonBullet prefab에 들어있어서 물리 연산하여 이동하는 기능.
    *
    & Functions 
    &   [public]
    &   : SetBulletState()              - 방울(대포알) 상태 설정
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    *
*/
public class CannonPhysic : MonoBehaviour
{

    Rigidbody2D rigidbody;
    GameObject playerObj;
    float disy;
    float disx;
    float dist;
    float Gpower;
    [SerializeField] private int state;
    SpriteRenderer spriteRenderer;

    [Header("Force")]
    [SerializeField]
    private float rightforce = 0.68f;

    [SerializeField]
    private float upforce = 21f;

    [Header("Sprite Render")]
    [SerializeField]
    private Sprite[] bulletSprite;
    [SerializeField]
    private Sprite[] bulletSprite_ColorBlind;

    private string[] LayerName = {"Cannon_R", "Cannon_G", "Cannon_B", "Cannon_Y", "Cannon"};

    public GoRightBefore GRB;
    
    void Start()
    {
        InitState();              
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != state){
            if (collision.CompareTag("Player"))
            {
                Player player = playerObj.gameObject.GetComponent<Player>();
                
                GRB?.CommonPushState
                (
                    new PushStateParam
                    {
                        tmpPushstate = PushState.Rain,
                        playerposition = playerObj.gameObject.transform.position,
                        prePlayerColor = player.ColorGetter(),
                        markerSpecialStatus = player.isTubeSpecial
                    }
                );
                
                player.ColorSetter(state, false);
                
            }
            if (collision.CompareTag("Player") || collision.CompareTag("Wall") || collision.CompareTag("Ground")) 
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    private void InitState(){
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        int colorIndex = ((state-1) % 5);

        this.spriteRenderer.sprite = !DataController.Instance.gameData.isColorFilterAssistant ? bulletSprite[colorIndex] : bulletSprite_ColorBlind[colorIndex];
        this.spriteRenderer.sortingLayerName = LayerName[colorIndex];

        playerObj =GameObject.FindWithTag("Player");
        rigidbody = this.gameObject.GetComponent<Rigidbody2D>();

        Gpower = (float)(rigidbody.gravityScale * 9.8*2);
        disy = playerObj.GetComponent<Transform>().position.y - this.GetComponent<Transform>().position.y;
        disx = ((playerObj.GetComponent<Transform>().position.x)- this.GetComponent<Transform>().position.x);
        dist = -(disy / 100) + 1;

        rigidbody.AddForce(transform.right*(disx) * rightforce,ForceMode2D.Impulse);
        rigidbody.AddForce(transform.up * (disy + upforce * dist), ForceMode2D.Impulse);
    }
    public void SetBulletState(int temp){
        state = temp;
    }

}
