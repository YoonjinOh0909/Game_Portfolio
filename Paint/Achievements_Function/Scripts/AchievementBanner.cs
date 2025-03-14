using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Unity.Services.Analytics;
using Unity.Services.Core;

/*
    * File : AchievementBanner.cs
    * Desc : 게임 입장 시 업적 확인 및 그에 따른 스킨 초기 설정 기능
    *
    & 
    &   [public]
    &   : SetInitPlayerSkin()                               - 저장되어 있는 값에 따른 스킨 프리펩 초기 세팅.
    &   : SetEachSkin(...)                                  - 각 카테고리에 맞도록 스킨 프리펩 세팅
    &   : GetSpriteOnSkinPrefabs(RewardCategory, int)       - 카테고리에 따른 해당 스킨 sprite return
    &   : GetSpriteOnGlassesPrefabs(int)                    - 안경 카테고리 스킨 sprite return
    &   : GetSpriteOnHatPrefabs(int)                        - 모자 카테고리 스킨 sprite return
    &   : GetSpriteOnMaskPrefabs(int)                       - 가면 카테고리 스킨 sprite return
    &   : GetSpriteOnTitleSprite(int)                       - 타이틀 카테고리 스킨 sprite return
    &   : GetSpriteOnTileSprite(int)                        - 타일 카테고리 스킨 sprite return
    &   : CheckAchFirstTime()                               - 실행시 업적 확인.
    &
    &   [private]
    &   : ReadRewardInfo()                                  - 전체 스킨 정보 불러오기
    &   : ReadAchievementGeneralInfo()                      - 업적 정보 불러오기
    &   : SortByAchNum()                                    - 업적 정보 순서, 크기에 따라 정렬
    &   : CheckAchieveTimeTotalSum()                        - 전체 플레이 타임 업적 확인
    &   : CheckUploadAchList()                              - 5분에 한 번씩 업적 테이블 자동 업데이트
    &   : AcheckEachAch(...)                                - 플레이 타임에 관련된 업적 확인 기능
    *
*/

public class AchievementBanner : MonoBehaviour
{

    [SerializeField]
    public Dictionary<string, List<AchieveGeneralEachInfo>> tempachGeneralInfo = new Dictionary<string, List<AchieveGeneralEachInfo>>();

    [SerializeField]
    private int[] totalSumPlayValList;

    public GameObject[] glassesPrefabs;

    public GameObject[] hatPrefabs;

    public GameObject[] maskPrefabs;

    
    public GameObject[] glasses_Filter_Prefabs;

    public GameObject[] hat_Filter_Prefabs;

    public GameObject[] mask_Filter_Prefabs;


    [SerializeField]
    private Sprite[] titleSprites;
    
    [SerializeField]
    private Sprite[] tileSprites;

    [SerializeField]
    private Sprite[] GlassesImgList;

    [SerializeField]
    private Sprite[] HatImgList;

    [SerializeField]
    private Sprite[] MaskImgList;

    [SerializeField]
    private Sprite[] Glasses_Filter_ImgList;

    [SerializeField]
    private Sprite[] Hat_Filter_ImgList;

    [SerializeField]
    private Sprite[] Mask_Filter_ImgList;

    private bool isFinishCheckAchFirstTime = false;
    private bool isGetRewardinfo = false;


    private void Awake() {

        var obj = FindObjectsOfType<AchievementBanner>();

        if(obj.Length == 1) {
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        
    }

    public void SetInitPlayerSkin(){ //데이터 받아온 이후 skin 적용
        int tempCurrentGlassIndex = DataController.Instance.gameData.currentGlassesIndex;
        int tempCurrentHatIndex = DataController.Instance.gameData.currentHatIndex;
        int tempCurrentMaskIndex = DataController.Instance.gameData.currentMaskIndex;

        DataController.Instance.gameData.currentGlassesPrefabs = 
            SetEachSkin(RewardCategory.Glasses, ref tempCurrentGlassIndex, BackendSkinManager.Instance.skinData.glassesListInt, glassesPrefabs, glasses_Filter_Prefabs);
        DataController.Instance.gameData.currentGlassesIndex = tempCurrentGlassIndex;

        DataController.Instance.gameData.currentHatPrefabs = 
            SetEachSkin(RewardCategory.Hat, ref tempCurrentHatIndex, BackendSkinManager.Instance.skinData.hatListInt, hatPrefabs, hat_Filter_Prefabs);
        DataController.Instance.gameData.currentHatIndex = tempCurrentHatIndex;

        DataController.Instance.gameData.currentMaskPrefabs = 
            SetEachSkin(RewardCategory.Mask, ref tempCurrentMaskIndex, BackendSkinManager.Instance.skinData.maskListInt, maskPrefabs, mask_Filter_Prefabs);
        DataController.Instance.gameData.currentMaskIndex = tempCurrentMaskIndex;

        DataController.Instance.SetGenAchievementOnDataController(this.gameObject.GetComponent<GenerateAchievementBanner>());
        
        ReadRewardInfo();
        ReadAchievementGeneralInfo();
    }

    private GameObject SetEachSkin(RewardCategory rc, ref int curIndex, List<int> listInt, GameObject[] normalPrefab, GameObject[] cbPrefab){
        bool cb = DataController.Instance.gameData.isColorFilterAssistant;
        GameObject tempObj = null;
        if(curIndex > -1){
            if(listInt.Contains(curIndex+ 1 + (int)rc)){
                tempObj = !cb ? normalPrefab[curIndex] : cbPrefab[curIndex];
            }else{
                curIndex = -2;
                tempObj = null;
            }
            
        }else{
            tempObj = null;
        }

        return tempObj;
    }
    private void ReadRewardInfo(){ //RewardItemGeneralInfo에 기본 정보들을 넣어둔다. 그러면 이 정보를 가지고 업적을 달성하였을 때 각 아이템의 정보들을 얻어낼 수 있다.
        List<Dictionary<string,object>> data = CSVReader.Read("reward_Info");

        for(int i = 0; i < data.Count; i++) {

            int reward_key = int.Parse(data[i]["Reward_Key"].ToString());
            RewardCategory Category = (RewardCategory)Enum.Parse(typeof(RewardCategory), data[i]["Category"].ToString());
            int item_index = int.Parse(data[i]["Item_Index"].ToString());

            if(!DataController.Instance.gameData.rewardItemGeneralInfo.ContainsKey(reward_key)){
                DataController.Instance.gameData.rewardItemGeneralInfo.Add(reward_key, new List<RewardItemEachInfo>());
                DataController.Instance.gameData.rewardItemGeneralInfo[reward_key].Clear();
            }

            DataController.Instance.gameData.rewardItemGeneralInfo[reward_key].Add(new RewardItemEachInfo(Category, item_index));
            
        }

        isGetRewardinfo = true;
    }
    private void ReadAchievementGeneralInfo() {
        for(int i = 0 ; i < 4; i++){
            DataController.Instance.gameData.totalAchieveCountEachCategory[i] = 0;
        }

        DataController.Instance.gameData.playAchKeyList.Clear();
        DataController.Instance.gameData.exceptPlayAchKeyList.Clear();
        DataController.Instance.gameData.decorateAchKeyList.Clear();
        DataController.Instance.gameData.otherAchKeyList.Clear();
        DataController.Instance.gameData.achieveCSVinOrder.Clear();

        List<Dictionary<string,object>> data = CSVReader.Read("achieve_Info");

        for(int i = 0; i < data.Count; i++) {

            int value = int.Parse(data[i]["Value"].ToString());
            string key = data[i]["Key"].ToString();
            string kor_c = data[i]["Kor_Contents"].ToString();
            string eng_c = data[i]["Eng_Contents"].ToString();
            string jpn_c = data[i]["Jpn_Contents"].ToString();

            string kor_t = data[i]["Kor_Title"].ToString();
            string eng_t = data[i]["Eng_Title"].ToString();
            string jpn_t = data[i]["Jpn_Title"].ToString();

            string kor_p = data[i]["Kor_prevAch"].ToString();
            string eng_p = data[i]["Eng_prevAch"].ToString();
            string jpn_p = data[i]["Jpn_prevAch"].ToString();

            string spritePath = data[i]["Sprite_Path"].ToString();
            string category = data[i]["Category"].ToString();
            int reward_key = int.Parse(data[i]["Reward_Key"].ToString());

            switch (category){
                case "Except_Play":
                    if(DataController.Instance.gameData.exceptPlayAchKeyList.IndexOf(key) == -1){
                        DataController.Instance.gameData.exceptPlayAchKeyList.Add(key);
                    }
                    DataController.Instance.gameData.totalAchieveCountEachCategory[0]++;
                    break;
                case "Play":
                    if(DataController.Instance.gameData.playAchKeyList.IndexOf(key) == -1){
                        DataController.Instance.gameData.playAchKeyList.Add(key);
                    }
                    DataController.Instance.gameData.totalAchieveCountEachCategory[1]++;
                    break;
                case "Decorate":
                    if(DataController.Instance.gameData.decorateAchKeyList.IndexOf(key) == -1){
                        DataController.Instance.gameData.decorateAchKeyList.Add(key);
                    }
                    DataController.Instance.gameData.totalAchieveCountEachCategory[2]++;
                    break;
                default:
                    if(DataController.Instance.gameData.otherAchKeyList.IndexOf(key) == -1){
                        DataController.Instance.gameData.otherAchKeyList.Add(key);
                    }
                    DataController.Instance.gameData.totalAchieveCountEachCategory[3]++;
                    break;
            }

            if(!tempachGeneralInfo.ContainsKey(key)){
                tempachGeneralInfo.Add(key, new List<AchieveGeneralEachInfo>());
                tempachGeneralInfo[key].Clear();

                DataController.Instance.gameData.achGeneralCheckValInfo.Add(key, new List<int>());
                DataController.Instance.gameData.achGeneralCheckValInfo[key].Clear();
            }
            tempachGeneralInfo[key].Add(new AchieveGeneralEachInfo(value, key, kor_c, eng_c, jpn_c, kor_t, eng_t, jpn_t, kor_p, eng_p, jpn_p, spritePath, category, reward_key));
            DataController.Instance.gameData.achGeneralCheckValInfo[key].Add(value);

            DataController.Instance.gameData.achieveCSVinOrder.Add(new AchieveCSVOrder(key, value));
        }

        SortByAchNum();

        DataController.Instance.gameData.homeEditorAchieveValList = DataController.Instance.gameData.achGeneralCheckValInfo["HEPT"].ToArray();
        DataController.Instance.gameData.storyMapPlayTimeAchieveValList = DataController.Instance.gameData.achGeneralCheckValInfo["SMPT"].ToArray();
        DataController.Instance.gameData.totalSumPlayTimeAchieveValList = DataController.Instance.gameData.achGeneralCheckValInfo["TSPT"].ToArray();

        totalSumPlayValList = DataController.Instance.gameData.totalSumPlayTimeAchieveValList;

        DataController.Instance.SetAchIntValueOnDC();
        StartCoroutine("CheckAchieveTimeTotalSum");
        StartCoroutine("CheckUploadAchList");
        
    }

    private void SortByAchNum(){//혹시 csv 파일에서 정렬되지 않은 값으로 저장되었다면, 정렬하는 코드
        foreach(KeyValuePair<string, List<AchieveGeneralEachInfo>> aa in tempachGeneralInfo){
            
            var result = from p in aa.Value orderby p.value select p;
            
            foreach (AchieveGeneralEachInfo r in result)
            {
                if(!DataController.Instance.gameData.achGeneralInfo.ContainsKey(aa.Key)){
                    DataController.Instance.gameData.achGeneralInfo.Add(aa.Key, new List<AchieveGeneralEachInfo>());
                    DataController.Instance.gameData.achGeneralInfo[aa.Key].Clear();
                }
                DataController.Instance.gameData.achGeneralInfo[aa.Key].Add(r);
            }
        }

        foreach(KeyValuePair<string, List<int>> bb in DataController.Instance.gameData.achGeneralCheckValInfo){
            bb.Value.Sort();
        }
    }
    IEnumerator CheckAchieveTimeTotalSum(){
       
        while(true){

            int tempCheckTotalSum = Mathf.FloorToInt(
                BackendAchManager.Instance.achData.totFloatVal.tspt
            );
            int totalSumMinutes = tempCheckTotalSum / 60;
            int totalSumIndex = Array.IndexOf(totalSumPlayValList, totalSumMinutes);

            if(totalSumIndex != -1){
                DataController.Instance.CheckAchievementOnDC("TSPT" , totalSumIndex);
            }

            yield return new WaitForSeconds(30f);
        }
    }


    // 5분에 한 번 씩 업적 테이블의 모든 컬럼 업데이트
    private IEnumerator CheckUploadAchList() {
        while(true) {
            yield return new WaitForSeconds(300.0f);

            #if UNITY_EDITOR
                DebugX.Log("Not Update Server cause UNITY_EDITOR");
            #else
                DebugX.Log("Update Server");
                BackendAchManager.Instance.UpdateAllAchData();
            #endif
            
        }
    }

    public Sprite GetSpriteOnSkinPrefabs(RewardCategory rc, int tempIndex){
        switch(rc){
            case RewardCategory.Glasses:
                return GetSpriteOnGlassesPrefabs(tempIndex);
                break;
            case RewardCategory.Hat:
                return GetSpriteOnHatPrefabs(tempIndex);
                break;
            case RewardCategory.Mask:
                return GetSpriteOnMaskPrefabs(tempIndex);
                break;
            default:
                return null;
                break;
        }
    }

    public Sprite GetSpriteOnGlassesPrefabs(int tempIndex){
        if(tempIndex-1 < GlassesImgList.Length){
            return !DataController.Instance.gameData.isColorFilterAssistant ? GlassesImgList[tempIndex-1] : Glasses_Filter_ImgList[tempIndex-1];
        }
        return null;
        
    }
    public Sprite GetSpriteOnHatPrefabs(int tempIndex){
        if(tempIndex-1 < HatImgList.Length){
            return !DataController.Instance.gameData.isColorFilterAssistant ? HatImgList[tempIndex-1] : Hat_Filter_ImgList[tempIndex-1];
        }

        return null;
    }

    public Sprite GetSpriteOnMaskPrefabs(int tempIndex){
        if(tempIndex-1 < MaskImgList.Length){
            return !DataController.Instance.gameData.isColorFilterAssistant ? MaskImgList[tempIndex-1] : Mask_Filter_ImgList[tempIndex-1];
        }

        return null;
    }

    public Sprite GetSpriteOnTitleSprite(int tempIndex){
        if(tempIndex-1 < titleSprites.Length){
            return titleSprites[tempIndex-1];
        }
        return null;
    }
    public Sprite GetSpriteOnTileSprite(int tempIndex){
        if(tempIndex-1 < tileSprites.Length){
            return tileSprites[tempIndex-1];
        }
        return null;
    }

    public void CheckAchFirstTime(){ //모든 업적들을 한 번 쫙 확인할 수 있도록 한다.
        //1. INT value로 이뤄진 업적
            
        //2. FLOAT value로 이뤄진 업적
            /*
                //아래 업적 모두 사용하고 있는 상태이다. 따라서 아래 3개는 업적 달성 확인이 필요하다.
                HEPT : home edit play time (local에 homeEditorTime 이라는 변수로 저장 )
                SMPT : stage map play time (backend에 tsmp라는 변수로 저장)
                TSPT : total stage play time (backend에 tspt 라는 변수로 저장.)
            */

        int tempCheckHomeEditPlayTime = Mathf.FloorToInt(
            BackendAchManager.Instance.achData.totFloatVal.thep
        );
        int totalHomeEditPlayMinutes = tempCheckHomeEditPlayTime / 60;
        AcheckEachAch(totalHomeEditPlayMinutes, "HEPT", DataController.Instance.gameData.homeEditorAchieveValList);
        
        //2-2 SMPT
        int tempCheckStageMapPlayTime = Mathf.FloorToInt(
            BackendAchManager.Instance.achData.totFloatVal.tsmp
        );
        int totalStageMapPlayMinutes = tempCheckStageMapPlayTime / 60;

        AcheckEachAch(totalStageMapPlayMinutes, "SMPT", DataController.Instance.gameData.storyMapPlayTimeAchieveValList);

        //2-3 TSPT
        BackendAchManager.Instance.achData.totFloatVal.tspt = 
            BackendAchManager.Instance.achData.totFloatVal.tmmp + //홈 화면에 있는 시간
            BackendAchManager.Instance.achData.totFloatVal.thep + //홈에서 편집한 시간
            BackendAchManager.Instance.achData.totFloatVal.tcpt + //커스텀 맵 플레이 시간
            BackendAchManager.Instance.achData.totFloatVal.tcet + //커스텀 맵 편집 시간
            BackendAchManager.Instance.achData.totFloatVal.tsmp; //스테이지(스토리) 플레이 시간
        
        int tempCheckTotalSum = Mathf.FloorToInt(
            BackendAchManager.Instance.achData.totFloatVal.tspt
        );
        int totalSumMinutes = tempCheckTotalSum / 60;
        AcheckEachAch(totalSumMinutes, "TSPT", DataController.Instance.gameData.totalSumPlayTimeAchieveValList);

        #if UNITY_EDITOR
            DebugX.Log("Not Update Server cause UNITY_EDITOR");
        #else
            DebugX.Log("Update Server ");
            BackendAchManager.Instance.UpdateAchListData();
        #endif

        // 24-10-15
        //서버 주석 서버주석 server 주석하기.
        #if UNITY_EDITOR
            DebugX.Log("Not Update Server cause UNITY_EDITOR");
        #else
            DebugX.Log("Update Server");
            BackendAchManager.Instance.UpdateAchNameData();
        #endif           
        
        isFinishCheckAchFirstTime = true;

        //체크하는 것이 모두 끝났으니 스킨 적용 및 패널 적용
        SetButtonForRewardItemPanel setButtonForRewardItemPanel = GameObject.FindWithTag("SkinPanel").GetComponent<SetButtonForRewardItemPanel>();
        setButtonForRewardItemPanel.InitSetting();
    }

    private void AcheckEachAch(int totalMinutes, string achStr, int[] achValList){
        for(int i = 0; i < achValList.Length; i++) {
            int achIndex = BackendAchManager.Instance.achData.achName.IndexOf(achStr);
            if(totalMinutes >= achValList[i]){ // 기준점보다 높다면
                int totalSumIndex = i;
                DataController.Instance.CheckAchievementOnDC(achStr , totalSumIndex);
            }else{

                //업적 리스트에 들어가있을 때
                if(achIndex != -1){
                    string key = "ACH_" + achStr + "_" + achValList[i];

                    //만약 해당 업적에서 가장 작은 기준인 [0]에 해당하는 값보다 작다면
                    if(i == 0){ 
                        //업적 받은 리스트에서 삭제해야한다.
                        BackendAchManager.Instance.achData.achName.RemoveAt(achIndex);

                        //achInfo에서도 같은 index에 있는 정보를 삭제한다.
                        BackendAchManager.Instance.achData.achInfo.RemoveAt(achIndex);
                        
                        //다음 업적부터 계산해서 스킨을 받는게 있으면 취소한다.
                        for(int k = i; k < achValList.Length; k++){
                            int reward_key = DataController.Instance.gameData.achGeneralInfo[achStr][k].Reward_Key;

                            if(reward_key != 0 && DataController.Instance.gameData.rewardItemGeneralInfo.ContainsKey(reward_key)){ //reward_key가 0이라면 업적을 달성해도 얻을 수 있는 보상이 없다는 뜻.
                                for(int j = 0; j < DataController.Instance.gameData.rewardItemGeneralInfo[reward_key].Count; j++){
                                    int rewardItemIdx = DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][j].itemIndex;
                                    switch(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][j].rewardCategory){
                                        case RewardCategory.Glasses:
                                            if(BackendSkinManager.Instance.skinData.glassesListInt.Contains(rewardItemIdx)){
                                                BackendSkinManager.Instance.skinData.glassesListInt.Remove(rewardItemIdx);
                                            }
                                            break;
                                        case RewardCategory.Hat:
                                            if(BackendSkinManager.Instance.skinData.hatListInt.Contains(rewardItemIdx)){
                                                BackendSkinManager.Instance.skinData.hatListInt.Remove(rewardItemIdx);
                                            }
                                            break;
                                        
                                        case RewardCategory.Mask:
                                            if(BackendSkinManager.Instance.skinData.maskListInt.Contains(rewardItemIdx)){
                                                BackendSkinManager.Instance.skinData.maskListInt.Remove(rewardItemIdx);
                                            }
                                            break;
                                        case RewardCategory.Title:
                                            if(DataController.Instance.gameData.titleRewardIndexList.Contains(rewardItemIdx)){
                                                //현재 스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 정해진 것이 없어서 로컬에 저장을 하도록 한다.
                                                DataController.Instance.gameData.titleRewardIndexList.Remove(rewardItemIdx);
                                            }
                                            break;
                                        case RewardCategory.Tile:
                                            if(DataController.Instance.gameData.tileRewardIndexList.Contains(rewardItemIdx)){
                                                //현재 스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 정해진 것이 없어서 로컬에 저장을 하도록 한다.
                                                DataController.Instance.gameData.tileRewardIndexList.Remove(rewardItemIdx);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    //2번째 이상부터 
                    else{
                        //i와 Count가 같다는 뜻은 제대로 업적이 서버와 데이터간의 계산이 잘 되었다는 뜻.
                        if(BackendAchManager.Instance.achData.achInfo[achIndex].achList.Count <= i){

                        }

                        //Count가 더 크다는 뜻은 아직 달성되면 안되는 업적이 클리어되어있다고 판단된 것.
                        else if(BackendAchManager.Instance.achData.achInfo[achIndex].achList.Count > i){
                            
                            //필요한 개수 빼고 모두 삭제하도록 한다.

                            //1. 먼저 초기화를 한다.
                            BackendAchManager.Instance.achData.achInfo[achIndex].achList.Clear();

                            //2. 필요한 true 개수만큼 true를 추가한다
                            for(int j=0; j < i; j++){
                                BackendAchManager.Instance.achData.achInfo[achIndex].achList.Add(true);
                            }

                            //다음 업적부터 계산해서 스킨을 받는게 있으면 취소한다.
                            for(int k = i; k < achValList.Length; k++){
                                int reward_key = DataController.Instance.gameData.achGeneralInfo[achStr][k].Reward_Key;

                                if(reward_key != 0 && DataController.Instance.gameData.rewardItemGeneralInfo.ContainsKey(reward_key)){ 
                                    for(int j = 0; j < DataController.Instance.gameData.rewardItemGeneralInfo[reward_key].Count; j++){
                                        int rewardItemIdx = DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][j].itemIndex;
                                        switch(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][j].rewardCategory){
                                            case RewardCategory.Glasses:
                                                if(BackendSkinManager.Instance.skinData.glassesListInt.Contains(rewardItemIdx)){
                                                    BackendSkinManager.Instance.skinData.glassesListInt.Remove(rewardItemIdx);
                                                }
                                                break;
                                            case RewardCategory.Hat:
                                                if(BackendSkinManager.Instance.skinData.hatListInt.Contains(rewardItemIdx)){
                                                    BackendSkinManager.Instance.skinData.hatListInt.Remove(rewardItemIdx);
                                                }
                                                break;
                                            
                                            case RewardCategory.Mask:
                                                if(BackendSkinManager.Instance.skinData.maskListInt.Contains(rewardItemIdx)){
                                                    BackendSkinManager.Instance.skinData.maskListInt.Remove(rewardItemIdx);
                                                }
                                                break;
                                            case RewardCategory.Title:
                                                if(DataController.Instance.gameData.titleRewardIndexList.Contains(rewardItemIdx)){
                                                    //현재 스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 로컬에 저장을 하도록 한다.
                                                    DataController.Instance.gameData.titleRewardIndexList.Remove(rewardItemIdx);
                                                }
                                                break;
                                            case RewardCategory.Tile:
                                                if(DataController.Instance.gameData.tileRewardIndexList.Contains(rewardItemIdx)){
                                                    //현재 스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 로컬에 저장을 하도록 한다.
                                                    DataController.Instance.gameData.tileRewardIndexList.Remove(rewardItemIdx);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                }
                else{
                    // 기준점보다 가지고 있는 데이터가 작고, 해당 업적에 대한 정보가 없을 때는 반복을 멈춘다.
                    break;
                }
                
            }
        }
    }

}
