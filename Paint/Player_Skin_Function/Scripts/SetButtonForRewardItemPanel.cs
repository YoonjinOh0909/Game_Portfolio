using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;


/*
    * File : SetButtonForRewardItemPanel.cs
    * Desc : Skin Setting Controller의 역할을 수행
    *
    & 
    &   [public]
    &   : CloseSkinPanel()                          - 스킨 세팅 패널 닫는 기능
    &   : InitSetting()                             - 초기 변수 및 버튼 세팅
    &   : SBSize()                                  - Invoke RealSB() 를 위한 함수
    &   : AdaptNowSetting()                         - 변경된 세팅 값 적용
    &   : SetRect(int, RewardCategory)              - 스킨의 개수에 따라 Scroll Rect 설정.
    &   : ChangeItemExp(string)                     - 스킨 버튼 클릭할 때 설명도 변경
    &   : ResetAdaptAnim()                          - 미리 보기 애니메이션 초기화
    &
    &   [private]
    &   : SetPreventDoubleClick(bool, float)        - 닫을 때 중복 눌림 방지 기능
    &   : SetPlayerCanMove()                        - 의도되지 않는 플레이어 움직임 방지 기능
    &   : SetTryanyThingFalse()                     - 홈 화면에서 ui 조작에 의한 delay 설정
    &   : SettingBtnOnPannel(...)                   - 각 카테고리별 획득한 스킨 버튼 세팅.
    &   : RealSB()                                  - 스크롤바 크기 조정
    &   : SetEachCategory(...)                      - 각 카테고리별 스킨 착용 미리보기 이미지 적용
    &   : ChangeItemSkin(...)                       - 실제 플레이어의 스킨 아이템 변경 후 적용
    &   : CheckAllIndex(SkinItemButton[])           - 적용된 스킨 체크 표시
    *
*/

public class SetButtonForRewardItemPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject eachBtn;

    [SerializeField]
    private GameObject eachDeleteBtn;

    public int selectGlassesIndex = 0;

    public int selectHatIndex = 0;

    public int selectMaskIndex = 0;
   
    public GameObject GlassesPanel;
    public GameObject HatPanel;
    public GameObject MaskPanel;
    
    [SerializeField]
    private SetCharacterItem playerGlasses;

    [SerializeField]
    private SetCharacterItem playerHat;

    [SerializeField]
    private SetCharacterItem playerMask;
    public int currentGlassesCnt = -1 ;
    public int currentHatCnt = -1 ;
    public int currentMaskCnt = -1 ;

    [SerializeField]
    private Image MaskImg;

    [SerializeField]
    private Image MaskImg_ForFilter;

    [SerializeField]
    private Image GlassesImg;

    [SerializeField]
    private Image HatImg;

    [SerializeField]
    private SkinItemButton[] glassesSIB;

    [SerializeField]
    private SkinItemButton[] hatSIB;

    [SerializeField]
    private SkinItemButton[] maskSIB;

    [SerializeField]
    private Button GlassesCateBUtton;

    
    [SerializeField]
    private Button HatCateBUtton;

    [SerializeField]
    private Button MaskCateBUtton;

    [SerializeField]
    private Scrollbar glassesSB;

    [SerializeField]
    private Scrollbar hatSB;

    [SerializeField]
    private Scrollbar maskSB;

    [SerializeField]
    private ScrollRect glassesRect;

    [SerializeField]
    private ScrollRect hatRect;

    [SerializeField]
    private ScrollRect maskRect;

    [SerializeField]
    private Animator skinPanelAnim;

    [SerializeField]
    private PreventDoubleClick preventDoubleClick;

    public AchievementBanner achievementBanner;

    [SerializeField]
    private Image adaptMoonie;

    [SerializeField]
    private Sprite[] idleList;

    [SerializeField]
    private Player player;

    public LocalizeStringEvent localizedSkinName;
    public LocalizeStringEvent localizedSkinExplanation;
    
    [SerializeField]
    private Animator idleAnim;
    
    void Start()
    {
        achievementBanner = GameObject.FindWithTag("AchBanner").GetComponent<AchievementBanner>();
    }

    private void Update() {
         if(Input.GetButtonDown("Menu"))
        {
            CloseSkinPanel();
        }
    }

    public void CloseSkinPanel(){
        if(skinPanelAnim.GetBool("isOpenSkin"))
        {
            if (skinPanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !skinPanelAnim.IsInTransition(0))
            {
                EventSystem.current.SetSelectedGameObject(null);

                skinPanelAnim.SetBool("isOpenSkin", false);

                //업적 패널 꺼지는 와중에 클릭되는 것 방지 위해 Delay 추가
                StartCoroutine(SetPlayerCanMove(true, 1f));
                StartCoroutine(SetPreventDoubleClick(true, 1f));

                Invoke ("SetTryanyThingFalse", 0.5f);
            }
        }
        
    }
    
    
    private IEnumerator SetPreventDoubleClick(bool state, float delayTime) {
        var wait = new WaitForSeconds(delayTime);
        yield return wait;
        preventDoubleClick.SetonEnabledClick(state);
    }

    private IEnumerator SetPlayerCanMove(bool state, float delayTime) {
        var wait = new WaitForSeconds(delayTime);
        yield return wait;
        DataController.Instance.gameData.playerCanMove = state;
    }

    private void SetTryanyThingFalse(){
        DataController.Instance.gameData.tryAnythingonHomeGrid = false;
    }
    
    public void InitSetting(){
        ChangeItemExp("1000");        

        selectGlassesIndex = DataController.Instance.gameData.currentGlassesIndex+1;
        selectHatIndex = DataController.Instance.gameData.currentHatIndex+1;
        selectMaskIndex = DataController.Instance.gameData.currentMaskIndex+1;

        SettingBtnOnPannel(RewardCategory.Glasses, ref currentGlassesCnt, GlassesPanel, BackendSkinManager.Instance.skinData.glassesListInt, ref glassesSIB);
        SettingBtnOnPannel(RewardCategory.Hat, ref currentHatCnt, HatPanel, BackendSkinManager.Instance.skinData.hatListInt, ref hatSIB);
        SettingBtnOnPannel(RewardCategory.Mask, ref currentMaskCnt, MaskPanel, BackendSkinManager.Instance.skinData.maskListInt, ref maskSIB);

        
        int colorFilterIndex = DataController.Instance.gameData.isColorFilterAssistant ? 5 : 0;
        int playerLayer = 0;

        player ??= GameObject.FindWithTag("Player").GetComponent<Player>();
        playerLayer = (player?.ColorGetter() ?? 0) % 5;
        
        adaptMoonie.sprite = idleList[colorFilterIndex + playerLayer];
        idleAnim.SetInteger("idleIndex", colorFilterIndex + playerLayer);
        
        AdaptNowSetting();
        
    }

    private void SettingBtnOnPannel(RewardCategory rc, ref int currentCnt, GameObject skinPanel, List<int> listint, ref SkinItemButton[] sib){
        int rewardCnt = listint.Count;
        int divVal = (int)rc;
        if(currentCnt != rewardCnt || (currentCnt == -1)){    
            currentCnt = currentCnt >= 0 ? currentCnt : 0;
            int min = rewardCnt < 9 ? 9 : rewardCnt;

            foreach(SkinItemButton_ForDelete tempOBj in skinPanel.transform.GetComponentsInChildren<SkinItemButton_ForDelete>() ){
                Destroy(tempOBj.gameObject);
            }
            
            for(int i = currentCnt; i < min; i++) {
                if(i < rewardCnt){
                    var button = Instantiate(eachBtn, skinPanel.transform, false) as GameObject;
                    Sprite tempSprite = achievementBanner.GetSpriteOnSkinPrefabs(rc, listint[i]%divVal);
                    button.gameObject.GetComponent<SkinItemButton>().SetInitValue(this, rc, listint[i]%divVal, tempSprite, i+1);
                }else{
                    var button = Instantiate(eachDeleteBtn, skinPanel.transform, false) as GameObject;                
                }
                
            }
            currentCnt = rewardCnt;
        }

        sib = skinPanel.GetComponentsInChildren<SkinItemButton>();
    }   

    public void SBSize(){
        Invoke("RealSB", 0.05f);
    }

    private void RealSB(){
        glassesSB.size = 0.05f;
        hatSB.size = 0.05f;
        maskSB.size = 0.05f;
    }

    public void AdaptNowSetting(){
        int selectGI = selectGlassesIndex-1;
        int selectHI = selectHatIndex-1;
        int selectMI = selectMaskIndex-1;

        DataController.Instance.gameData.currentGlassesIndex = selectGI;
        DataController.Instance.gameData.currentHatIndex = selectHI;
        DataController.Instance.gameData.currentMaskIndex = selectMI;

        bool isColorFilterAssistant = DataController.Instance.gameData.isColorFilterAssistant;

        DataController.Instance.gameData.currentGlassesPrefabs = SetEachCategory(RewardCategory.Glasses, selectGI, isColorFilterAssistant, achievementBanner.glassesPrefabs, achievementBanner.glasses_Filter_Prefabs, ref GlassesImg);
        DataController.Instance.gameData.currentHatPrefabs = SetEachCategory(RewardCategory.Hat, selectHI, isColorFilterAssistant, achievementBanner.hatPrefabs, achievementBanner.hat_Filter_Prefabs, ref HatImg);
        DataController.Instance.gameData.currentMaskPrefabs = SetEachCategory(RewardCategory.Mask, selectMI, isColorFilterAssistant, achievementBanner.maskPrefabs, achievementBanner.mask_Filter_Prefabs, ref MaskImg);

        int skinParentIndex = isColorFilterAssistant ? 1 : 0;
        
        ChangeItemSkin(ref playerGlasses, "WearingGlasses", skinParentIndex);
        ChangeItemSkin(ref playerHat, "WearingHat", skinParentIndex);
        ChangeItemSkin(ref playerMask, "WearingMask", skinParentIndex);

        //체크 표시
        CheckAllIndex(glassesSIB);
        CheckAllIndex(hatSIB);
        CheckAllIndex(maskSIB);

    }

    private GameObject SetEachCategory(RewardCategory rc, int selectIndex, bool cb, GameObject[] prefabList, GameObject[] prefabList_forcb, ref Image skinImg){
        GameObject skinPrefab = null;
        if(selectIndex > -1){
            skinPrefab = !cb ?  prefabList[selectIndex] : prefabList_forcb[selectIndex];

            skinImg.color = !cb ? new Color(1,1,1,1) : new Color(0,0,0,0);
            skinImg.sprite = !cb ? prefabList[selectIndex].GetComponent<SpriteRenderer>().sprite : prefabList_forcb[selectIndex].GetComponent<SpriteRenderer>().sprite;
        }else{
            skinImg.color = new Color(0,0,0,0);
        }

        if(rc == RewardCategory.Mask){
            if(selectIndex > -1){
                MaskImg_ForFilter.color = !cb ? new Color(0,0,0,0) : new Color(1,1,1,1);
                MaskImg_ForFilter.sprite = prefabList_forcb[selectIndex].GetComponent<SpriteRenderer>().sprite;
            }else{
                MaskImg_ForFilter.color = new Color(0,0,0,0);
            }
        }
        return skinPrefab;
    }

    private void ChangeItemSkin(ref SetCharacterItem sci, string val, int spi){
        sci ??= GameObject.FindWithTag(val).transform.GetChild(spi).gameObject.GetComponent<SetCharacterItem>();
        sci?.ChangeItemSkinImage();
    }

    private void CheckAllIndex(SkinItemButton[] temp)
    {
        foreach(SkinItemButton obj in temp) {
            obj.CheckNowIndex();
        }
    }

    public void SetRect(int index, RewardCategory rewardCategory){
        int indexM = (index / 2);
        indexM = indexM < 4 ? 0 : 1;
        if(index < 2 || index > 7){
            switch(rewardCategory){
                case RewardCategory.Glasses :
                    glassesRect.normalizedPosition = new Vector2(indexM,0.0f);
                    break;
                case RewardCategory.Hat :
                    hatRect.normalizedPosition = new Vector2(indexM,0.0f);
                    break;
                case RewardCategory.Mask :
                    maskRect.normalizedPosition = new Vector2(indexM,0.0f);
                    break;
                default:
                    break;

            }
        }
        
    }

    public void ChangeItemExp(string keyString){ 
        localizedSkinName.StringReference.TableEntryReference = keyString; //key값
        localizedSkinExplanation.StringReference.TableEntryReference = keyString+"_exp";
    }

    public void ResetAdaptAnim(){
        idleAnim.SetInteger("idleIndex", -1);
    }
}
