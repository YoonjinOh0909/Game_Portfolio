using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    * File : RainScripts.cs
    * Desc : Trigger 범위 안에 플레이어가 들어왔을 때, 비구름 색상과 같은 색상으로 바꾸는 기능 구현.
    *
    & Functions 
    &   [public]
    &   : GetLayer()                    - 비구름 Layer 리턴
    &
    &   [private]
    &   : InitState()                   - 변수 초기화
    *
*/
public class RainScripts : MonoBehaviour
{
    [SerializeField]
    private int layer;

    private GoRightBefore GMGoRightBefore;
    
    [SerializeField]
    private GetPositionForGoRightBefore childObject;

    [SerializeField]
    private SoundCtr soundCtr;

    [SerializeField]
    private Animator thisAnimator;

    private void Start() {
        InitState();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")&&layer!=other.gameObject.layer){
            
            GMGoRightBefore.CommonPushState
            (
                new PushStateParam
                {
                    tmpPushstate = PushState.Rain,
                    playerposition = childObject.GetPlayerPosition(),
                    prePlayerColor = other.GetComponent<Player>().ColorGetter(),
                    markerSpecialStatus = other.GetComponent<Player>().isTubeSpecial
                }
            );
            
            soundCtr.PlaySound();

            other.GetComponent<Player>().ColorSetter(layer, false);
        }
    }

    private void InitState(){
        soundCtr = this.GetComponent<SoundCtr>();
        GMGoRightBefore = GameObject.FindWithTag("GameManager")?.GetComponent<GoRightBefore>();
        bool ColorBlind = DataController.Instance.gameData.isColorFilterAssistant;
        thisAnimator?.SetBool("CB",ColorBlind);
    }
    public int GetLayer(){
        return layer;
    }
}
