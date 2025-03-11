using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    * File : BowlToCannon.cs
    * Desc : 비눗방울(Cannon과 동일)과 연결된 물통 설정 기능
    *
    & Functions 
    &   [public]
    &   : ChangeBulletState(int)      - 생성되는 방울(대포알) 상태 변경
    &
    &   [private]
    &   : InitState()                           - 변수 초기화
    &   : FindCannon()                     - 연결된 Cannon 탐색
    *
*/

public class BowlToCannon : MonoBehaviour
{
    [SerializeField]
    private CannonShoot cannon;

    public bool IsMapEditor;
    private int findCnt;
    private bool CanFindCannon = true;

    private void Start() {
        InitState();
    }
    void FixedUpdate() {
        if(IsMapEditor && cannon == null && CanFindCannon){
            FindCannon();
        }
    }

    private void InitState(){
        PlayMode nowMode = DataController.Instance.gameData.playMode;
        IsMapEditor = (nowMode != PlayMode.IDLE && SceneManager.GetActiveScene().name != "HomePlayGrid_Tutorial");
        findCnt = 0;
    }
    private void FindCannon(){
        LineAble tempLineAble = this.transform.parent.GetComponent<LineAble>();
        LineRendererConnector Temp = (tempLineAble.Line != null) ? tempLineAble.Line.GetComponent<LineRendererConnector>() : null;
        
        if(Temp == null){
            findCnt++;
            if(findCnt > 10){
                CanFindCannon = false;
                Destroy(GetComponent<BowlToCannon>());
            }
            return;
        }
            
        for(int i=0;i<Temp.Positions.Count;i++)
        {
            if(Temp.Positions[i]!=this.transform.parent.GetChild(0).transform)
            {
                cannon=Temp.Positions[i].parent.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<CannonShoot>();
                this.gameObject.GetComponent<ItemScripts>().SetiscannonBowl(cannon != null);
            }
        }
        
    }

    public void ChangeBulletState(int Color){
        cannon.SetBulletStateOnCannonShoot(Color);
        this.transform.parent.TryGetComponent<LineAble>(out LineAble templineable);
        templineable.Line.GetComponent<LineRendererConnector>().ChangeLineRender(Color);
    }

}
