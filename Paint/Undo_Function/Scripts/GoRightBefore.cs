using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.UI;

/*
    * File : GoRightBefore.cs
    * Desc : Undo 기능 구현
    *
    & Functions 
    &   [public]
    &   : InitStartState()                  - 변수 전체 초기화
    &   : CommonPushState(PushStateParam)   - Push를 통한 상태 저장
    &   : PopState()                        - Pop을 통한 직전 상황 열람
    &   : ReadStack(GameState)              - Pop으로 얻은 데이터로 게임에 적용
    &   : ResetStack()                      - 스택 초기화
    &   : SetisTBinfo(bool)                 - 타이머 블록 기믹 존재 여부 설정
    &   : SetisMovinginfo(bool)             - 움직이는 발판 기믹 존재 여부 설정
    &   : SetActiveGoRightBeforeBtn(bool)   - 뒤로가기 버튼 on/off 설정
    &   : SetKeyPosition(Vector3)           - 키 위치 설정
    &   : SetGBTotalCount()                 - 다시하기 횟수 설정
    &   : GetisPushed()                     - Push 상황 리턴
    &   : GetGBcount()                      - 뒤로가기 횟수 리턴
    &   : GetGBTotalcount()                 - 한 게임 내 전체 뒤로가기 횟수 리턴
    &   : GetretryCount()                   - 다시하기 횟수 리턴
    &   : GetStoryPlayTime()                - 플레이 타임 리턴
    &   : GetStoryPlayTotalTime()           - 한 게임 내 전체 플레이 타임 리턴
    &   
    &   [private]
    &   : CheckCoroutine()          - 업적 관련 데이터 동기화
    &   : CheckAchieveTimeStory()   - 시간 관련 업적 갱신 (IEnumerator)
    &   : ChangeLazerInteract()     - 레이저 기믹 설정
    &   : InitGameState(GameState)  - GameState 변수 초기화
    &   : SetisPopFalse()           - Pop 상황 설정
    *
*/
public enum PushState
{
    Box,
    Rain, 
    Lazer, 
    Portal, 
    GravityPortal,
    Bowl, 
    Marker 
}


public class GoRightBefore : MonoBehaviour
{
    [Header("Stack")]

    [SerializeField]
    public Stack<GameState> stack = new Stack<GameState>();
    public GameState startStack; //초기 stack에 첫 번째로 들어갈 GameState 

    [Header("Brush")]
    public Vector3 brushInitPos, brushInitRot, brushDiff = new Vector3();

    [SerializeField]
    public GameObject brushParent, brushPrefab;


    [Header("Player")]
    public GameObject playerObject;
    public Player player;
    public Player_GetItem playerMarker;

    [Header("MarkerPrefab")]
    
    [SerializeField]
    GameObject[] basicMarkerPrefab;

    [SerializeField]
    GameObject[] specialMarkerPrefab;

    [Header("GoRightBeforeBtn")]
    [SerializeField]
    GameObject GoRightBeforeBtn;
    private bool isPushed;

    [Header("MovingObject")]
    public MovingObjInfo movObjInfo;
    public MovingObjStack movObjStack;

    [SerializeField]
    private bool isMovObj;

    [Header("BoxObject")]
    private bool isBoxObj;
    private BoxStack boxStack;

    [Header("TimeBlockObject")]
    public TimeblockInfosc timeblockInfo;
    private bool isTimeBlockObj;

    [Header("OnOffBlockObject")]
    public OnOffBlockInfosc onoffblockinfo;
    private bool isOnOffBlockObj;

    [Header("MapEditor")]
    [SerializeField]
    private SettingManager mapEditorSettingManager;

    [SerializeField]
    private bool idleME;

    [Header("TBForME")]
    public TB_ME_Stack tbinfo;
    private bool isTBinfo;

    [SerializeField]
    private Toggle startBtn;

    private bool isPop = false;

    [Header("Analytics GorightBefore Count")]
    [SerializeField]
    protected private int gbCount, gbTotalCount, retryCount =0 ;

    [SerializeField]
    protected private float storyPlayTime, storyPlayTotalTime =0f;


    [Header("Achievements")]

    [SerializeField]
    private bool isTutorial = false;

    [SerializeField]
    private int[] storyTimeValList;

    [SerializeField]
    private int[] totalSumPlayValList;

    private float temptotalStoryTime, tempMainmenutime, tempHomeEditortime, tempTotalcustomMapPlayTime, tempTotalCustomMapEditTime = 0f;

    [SerializeField]
    private Edit_PlayController editPlayController;

    private GameState tmpAll = new GameState();

    private void Start() {
        if(DataController.Instance.gameData.playMode == PlayMode.MAPEDITOR){
            DataController.Instance.gameData.canOpenSetting = true;
        }
        isPop = false;
        
        idleME = false;
        Invoke("CheckCoroutine", 0.8f);

        mapEditorSettingManager ??= GameObject.Find("PausePanelHandler")?.GetComponent<SettingManager>();        
        
    }

    private void FixedUpdate() {
        if(!isTutorial &&DataController.Instance.gameData.playerCanMove && DataController.Instance.gameData.playMode == PlayMode.STORY){
            storyPlayTime += Time.deltaTime;
            temptotalStoryTime += Time.deltaTime;
        }
        
    }

    private void CheckCoroutine(){
        if(DataController.Instance.gameData.playMode == PlayMode.STORY){
            temptotalStoryTime = BackendAchManager.Instance.achData.totFloatVal.tsmp;
            storyTimeValList = DataController.Instance.gameData.storyMapPlayTimeAchieveValList;

            tempMainmenutime = BackendAchManager.Instance.achData.totFloatVal.tmmp;
            tempHomeEditortime = BackendAchManager.Instance.achData.totFloatVal.thep;
            tempTotalcustomMapPlayTime = BackendAchManager.Instance.achData.totFloatVal.tcpt;
            tempTotalCustomMapEditTime =  BackendAchManager.Instance.achData.totFloatVal.tcet;

            totalSumPlayValList = DataController.Instance.gameData.totalSumPlayTimeAchieveValList;

            StartCoroutine("CheckAchieveTimeStory");
        }
    }
    private IEnumerator CheckAchieveTimeStory(){
        while(true){
            int tempCheckStory = Mathf.FloorToInt(temptotalStoryTime);
            int tempCheckStoryMinutes = tempCheckStory / 60;
            int storyIndex = Array.IndexOf(storyTimeValList, tempCheckStoryMinutes);            

            if(storyIndex != -1){
                DataController.Instance.CheckAchievementOnDC("SMPT" , storyIndex);
            }
            
            BackendAchManager.Instance.achData.totFloatVal.tsmp = temptotalStoryTime;

            float totalSumPlay = tempMainmenutime +
                tempHomeEditortime +
                tempTotalcustomMapPlayTime +
                tempTotalCustomMapEditTime +
                BackendAchManager.Instance.achData.totFloatVal.tsmp;

            BackendAchManager.Instance.achData.totFloatVal.tspt = totalSumPlay;

            yield return new WaitForSeconds(5f);
        }
    }

    public void InitStartState()
    {   
        idleME = (startBtn ??= GameObject.Find("PlayButton")?.GetComponent<Toggle>()) == null;
        
        if(idleME || startBtn.isOn != true){    
            stack = new Stack<GameState>();

            GetComponent<BoxStack>().InitBoxState();

            tbinfo = GetComponent<TB_ME_Stack>();
            tbinfo.InitTBStack();
            
            GameObject brushObj = GameObject.FindGameObjectWithTag("Brush");;
            brushInitPos = brushObj?.transform.position ?? Vector3.zero;
            brushInitRot = brushObj?.transform.localEulerAngles ?? Vector3.zero;
            
            brushParent = null;
            brushParent = brushObj?.transform.parent?.gameObject;
            brushDiff = brushObj?.transform.position ?? Vector3.zero;
            
            startStack = new GameState();
            
            playerObject = null;
            player = null;
            playerMarker = null;
            
            playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject?.GetComponent<Player>();
            
            playerMarker = playerObject?.transform.GetChild(0).transform.Find("ItemGetter").GetComponent<Player_GetItem>();
        
            var movingPlatformParent = GameObject.FindGameObjectWithTag("MovingPlatformParent");

            isMovObj = movingPlatformParent != null;

            if(isMovObj){
                movObjInfo = GetComponent<MovingObjInfo>();
                movObjStack = GetComponent<MovingObjStack>();
                movObjStack.SetmovObhInfo();
            }
            
            var timeBrokeBlock = GameObject.FindGameObjectWithTag("TimeBrokeBlock");
            
            isTimeBlockObj = timeBrokeBlock != null;

            boxStack = this.gameObject.transform.GetComponent<BoxStack>();

            var groundOnOffParent = GameObject.FindGameObjectWithTag("GroundOnOffParent");
            isOnOffBlockObj = groundOnOffParent != null;
            onoffblockinfo = groundOnOffParent?.transform.GetChild(0).GetComponent<OnOffBlockInfosc>();

            GoRightBeforeBtn ??= GameObject.FindGameObjectWithTag("GoRightBefore");    
            
            isPushed = false;
            
            SetActiveGoRightBeforeBtn(false);
            InitGameState(startStack);
            
            startStack.playerPos = playerObject.transform.position;
            startStack.playerColor = player.ColorGetter();
            startStack.preMarker.hadMarker = player.key;
            startStack.preMarker.whichMarker = player.item;
            startStack.hadBrush = player.GetKeyValue();
            stack.Push(startStack);
            
        }

    }

    public void CommonPushState(PushStateParam tmpparam){
        movObjInfo?.PushState();
        timeblockInfo?.PushState();
        onoffblockinfo?.PushState();
        tbinfo?.PushState();
        boxStack?.PushState();
         
        InitGameState (tmpAll);

        playerObject ??= GameObject.FindGameObjectWithTag("Player");
        player ??= playerObject?.GetComponent<Player>();

        tmpAll.playerPos = tmpparam.playerposition.HasValue ? tmpparam.playerposition.Value : playerObject.gameObject.transform.position;
        tmpAll.playerRot = playerObject.transform.eulerAngles;
        tmpAll.playerColor = tmpparam.prePlayerColor.HasValue ? tmpparam.prePlayerColor.Value : player.ColorGetter();

        tmpAll.hadBrush = player.GetKeyValue();

        tmpAll.preMarker.whichMarker = tmpparam.tmpPushstate == PushState.Marker ? 0 : player.item;  //Marker가 없을 때만 집을 수 있으니 이 함수가 Push가 되면 없을 때 집은거라 0을 바로 입력.

        tmpAll.preMarker.isSpecial = tmpparam.markerSpecialStatus.Value;

        // 상호 작용 당시의 플레이어의 특수 상태 변수들 저장.
        tmpAll.preSpecialPumpkin.isSpecial = player.GetSpecialPumpkin().isSpecial;
        tmpAll.preSpecialPumpkin.prevColor = player.GetSpecialPumpkin().prevColor;
        tmpAll.preSpecialPumpkin.specialTime = player.GetSpecialPumpkin().specialTime;
        
        tmpAll.interacctedObj.interactedObject = tmpparam.interactedObj;
        tmpAll.interacctedObj.pushState = tmpparam.tmpPushstate.Value;
        tmpAll.interacctedObj.isSpecial = tmpparam.markerSpecialStatus.Value;

        if(tmpparam.tmpPushstate.Value == PushState.Marker){
            tmpAll.interacctedObj.interactedObjectPos = tmpparam.markerPos.Value;
            tmpAll.interacctedObj.interactedObjectRot = tmpparam.markerRot.Value;
            tmpAll.interacctedObj.markerIndex = tmpparam.tempColorindex.Value;
            tmpAll.preMarker.isSpecial = !tmpparam.markerSpecialStatus.Value;
        }
        if(tmpparam.tmpPushstate.Value == PushState.Bowl){
            tmpAll.interacctedObj.markerIndex = tmpparam.prevBowlColor.Value;

            GameState stackpeekTemp = stack.Peek();
            if(player.ColorGetter() == tmpparam.prevBowlColor.Value && !tmpAll.preSpecialPumpkin.isSpecial && !tmpparam.prevbowlSpecialStatus.Value){
                /*
                    Push 하지 않는 경우 :
                    ____________________________________________________________
                    |                                 | Player 상태 | 물통 상태 |
                    -------------------------------------------------------------
                    | case 1 | Player 색상 = 물통 색상 |    기본     |    기본   |
                    ____________________________________________________________
                */
            }else{
                /*
                    Push 하는 경우 :
                    _____________________________________________________________
                    |                                  | Player 상태 | 물통 상태 |
                    -------------------------------------------------------------
                    | case 1 |                         |    기본     |   스페셜  |
                    | case 2 | Player 색상 = 물통 색상  |   스페셜    |   스페셜  |
                    | case 3 |                         |   스페셜    |    기본   |
                    -------------------------------------------------------------
                    | case 4 |              Player 색상 != 물통 색상             |
                    _____________________________________________________________
                */

                stack.Push (tmpAll);
            }
        }
        else
        {
            stack.Push (tmpAll);
        }

        if(!isPushed){
            isPushed = true;
            SetActiveGoRightBeforeBtn(true);
        }

    }
    
    public void PopState()
    {
        if(!isPop){
            DataController.Instance.gameData.canLazerInteract = false;
            gbCount++;
            if(!isTutorial){
                BackendAchManager.Instance.achData.totIntVal.tgbc++;
            }else{
                BackendAchManager.Instance.achData.totIntVal.tgbc = 1;
                gbCount = 1;
            }
            
            DataController.Instance.CheckAchievementOnDC("GRBC");

            if(DataController.Instance.gameData.playMode == PlayMode.STORY && !isTutorial){
                DataController.Instance.CheckAchievementOnDC("InSMGRBC", gbCount + gbTotalCount, true);
            }else if(DataController.Instance.gameData.playMode == PlayMode.MAPEDITOR){
                if(editPlayController?.isOthers ?? false)
                {
                    DataController.Instance.CheckAchievementOnDC("InCMGRBC", gbCount, true);
                }
            }

            isPop = true;
            Invoke("SetisPopFalse",0.3f);
            
            player ??= playerObject.GetComponent<Player>();
            player?.SetRBZero();
                        
            movObjStack?.PopstackMoveInfo();
            timeblockInfo?.PopState();
            onoffblockinfo?.PopState();
            tbinfo?.PopState();
            boxStack?.PopState();

            if(stack.Count > 1){
                var popStack = stack.Pop();
                GameState temp = new GameState();
                temp = popStack;

                ReadStack(temp);

                if(stack.Count == 1){
                    isPushed = false;
                }
            }else{
                SetActiveGoRightBeforeBtn(false);
                isPushed = false;
                ReadStack(startStack);
            }
         
        }
        
    }

    public void ReadStack(GameState tempStack){
        playerObject.gameObject.transform.position = tempStack.playerPos; //상호작용한 위치로 이동하기
        player.ColorSetterForGoRightBefore(tempStack.playerColor, tempStack.preSpecialPumpkin.isSpecial); //상호작용 직전 색상으로 돌아가기
 
        player.SetPumpkinSpecial(tempStack.preSpecialPumpkin); //특수 상태 설정
        player.SetMoveSpeed(); //player의 속도 0 설정

        playerMarker.ColorSetter(tempStack.preMarker.whichMarker, tempStack.preMarker.isSpecial); // 상호작용 직전 가지고 있던 물감
        player.isTubeSpecial = tempStack.preMarker.isSpecial;
        player.itemSetterForGoRightBefore(tempStack.preMarker.whichMarker); 

        if(!tempStack.hadBrush){
            if(player.GetKeyValue()){ // 직전 상황에는 Key를 가지고 있지 않지만, 현재 가지고 있다면 이전으로 갈 때 Key를 재생성 해야 한다.
                player.KeyUnSetter();

                GameObject clone = Instantiate(brushPrefab, brushInitPos, Quaternion.identity);
                clone.name = "brush(Clone)";
                clone.GetComponent<ItemScripts>().starBox = GameObject.FindGameObjectWithTag("Door").transform.GetChild(0).gameObject;
            
                brushParent ??= GameObject.Find("BrushPosition(Clone)");
                            
                clone.transform.parent = brushParent?.transform;
                clone.transform.position = brushDiff;
                clone.transform.localEulerAngles = brushInitRot;
                clone.GetComponent<ItemScripts>().starBox.SetActive(false);

                GameObject.FindGameObjectWithTag("Door").transform.GetChild(8).gameObject.SetActive(false);
                
            }
            
        }
        switch(tempStack.interacctedObj.pushState){
            case PushState.Bowl :
                if(tempStack.interacctedObj.interactedObject != null){
                    tempStack.interacctedObj.interactedObject.GetComponent<ItemScripts>().SetColor(tempStack.interacctedObj.markerIndex);
                    tempStack.interacctedObj.interactedObject.GetComponent<ItemScripts>().SetIsSpecial(tempStack.interacctedObj.isSpecial);

                    player.isTubeSpecial = tempStack.preMarker.isSpecial;
                    
                    if(tempStack.interacctedObj.markerIndex== 0){
                        tempStack.interacctedObj.interactedObject.GetComponent<ItemScripts>().SetSRWhite();
                    }
                }
                break;
            case PushState.Marker :
                bool isParent = (tempStack.interacctedObj.interactedObject == null ? false : true);
                player.isTubeSpecial = false;
                
                if(tempStack.interacctedObj.markerIndex > 5 && tempStack.interacctedObj.markerIndex < 10){
                    var markerclone = tempStack.interacctedObj.isSpecial ? Instantiate(specialMarkerPrefab[tempStack.interacctedObj.markerIndex-6], tempStack.interacctedObj.interactedObjectPos, Quaternion.identity) : Instantiate(basicMarkerPrefab[tempStack.interacctedObj.markerIndex-6], tempStack.interacctedObj.interactedObjectPos, Quaternion.identity);    
                    markerclone.transform.localEulerAngles = tempStack.interacctedObj.interactedObjectRot;
                    if(isParent){
                        markerclone.transform.localEulerAngles = tempStack.interacctedObj.interactedObjectRot + tempStack.interacctedObj.interactedObject.transform.localEulerAngles;
                        markerclone.transform.parent = tempStack.interacctedObj.interactedObject.transform;
                    }
                }
                break;
            case PushState.GravityPortal :
                player.GravityReverse();
                playerObject.transform.eulerAngles = tempStack.playerRot;
                break;
            case PushState.Lazer :
                tempStack.interacctedObj.interactedObject.GetComponent<LazerDiode>().ResetAfterGRB();
                break;
            case PushState.Box :
                break;
            default :
                DebugX.Log(tempStack.interacctedObj.whichInteractionObjectIndex.ToString());
                break;
        }
    
        isPop = false;
        Invoke("ChangeLazerInteract", 0.1f);

    }
    private void ChangeLazerInteract(){
        DataController.Instance.gameData.canLazerInteract = true;
    }
    private void InitGameState(GameState init){
        init.playerPos = new Vector3(0,0,0);
        init.playerColor =0;
        init.preMarker.hadMarker = false;
        init.preMarker.whichMarker= 0;
        init.hadBrush = false;
        init.interacctedObj.whichInteractionObjectIndex = 0;
        init.interacctedObj.interactedObjectPos = new Vector3(0,0,0);
        init.interacctedObj.markerIndex = 0;
        init.interacctedObj.interactedObjectName = "";
        
    }

    public void ResetStack(){
        stack = new Stack<GameState>();
        InitStartState();
    }

    private void SetisPopFalse(){
        isPop = false;
    }

    public void SetisTBinfo(bool temp){
        isTBinfo = temp;
    }

    public void SetisMovinginfo(bool temp){
        isMovObj = temp;
    }

    public void SetActiveGoRightBeforeBtn(bool state){
        GoRightBeforeBtn.SetActive(state);
    }

    public void SetKeyPosition(Vector3 temp){
        brushInitPos = temp;
    }

    public void SetGBTotalCount(){ 
        bool analyisRetry = false;
        int analyGRB = 0;
        float analyTime = 0;

        if(retryCount == 0 ){
            analyisRetry = false;
            analyGRB = 0;
            analyTime = 0;
        }else{
            analyisRetry = true;
            analyGRB = gbCount;
            analyTime = storyPlayTime;
        }

        //Analytics 데이터 전송
        if(this.gameObject.scene.name != "HomePlayGrid_ResultScene" && DataController.Instance.gameData.playMode == PlayMode.STORY){
            CheckValueAnalytics checkAnaly = GameObject.Find("CheckValueAnalytics").GetComponent<CheckValueAnalytics>();
            checkAnaly.StartPlayStory(
                analyisRetry,
                DataController.Instance.gameData.chapter,
                DataController.Instance.gameData.currentStage,
                analyGRB,
                analyTime
            );
        }

        gbTotalCount = gbTotalCount + gbCount;
        storyPlayTotalTime = storyPlayTotalTime + storyPlayTime;
        storyPlayTime = 0;

        //다시하기 횟수 업적 확인
        DataController.Instance.CheckAchievementOnDC("InSMRC", retryCount, true);
        
        retryCount++;
        gbCount = 0;
    }
    
    public bool GetisPushed(){
        return isPushed;
    }
    public int GetGBcount(){
        return gbCount;
    }

    public int GetGBTotalcount(){
        return gbTotalCount;
    }

    public int GetretryCount(){
        return retryCount;
    }

    public float GetStoryPlayTime(){
        return storyPlayTime;
    }

    public float GetStoryPlayTotalTime(){
        return storyPlayTotalTime;
    }
}
