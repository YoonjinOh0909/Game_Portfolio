// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;

/*
    * File : GenerateAchievementBanner.cs
    * Desc : 업적 달성 확인 및 업적 배너 생성.
    *
    & 
    &   [public]
    &   : GenerateBanner(...)                          - 업적 달성 배너 생성
    &   : ConfirmAchiveValue(...)                             - 값이 int로 이루어진 업적들 확인 함수
    &   : ConfirmAchiveValue(string, int)                 - 값이 float로 이루어진 업적들 확인 함수
    &   : ConfirmAchiveValueForAllClear(...)                         - 각 챕터의 스테이지의 all clear 여부 업적 확인 함수
    &   : ConfirmAchiveValueForJustTriggerOne(string)         - 특정 지역을 지났을 경우 얻는 업적 확인 함수
    *
*/


public class GenerateAchievementBanner : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    GameObject rewardBannerprefab;

    [SerializeField]
    Transform parent;

    [SerializeField]
    private Sprite[] rewardBackground;

    [SerializeField]
    private AchievementBanner achievementBanner;

    public void GenerateBanner(AchieveGeneralEachInfo tempGenBannerEachInfo, bool isInt, bool isNewAch){
        //실행이 될 때마다 어떠한 업적이 달성된 상황. 그렇기 때문에 여기서 업적에 해당하는 보상 제공.
        
        string name = tempGenBannerEachInfo.key;
        int value = tempGenBannerEachInfo.value;

        string kor_Contents = tempGenBannerEachInfo.Kor_Contents;
        string eng_Contents = tempGenBannerEachInfo.Eng_Contents;
        string jpn_Contents = tempGenBannerEachInfo.Jpn_Contents;

        string kor_Title = tempGenBannerEachInfo.Kor_Title;
        string eng_Title = tempGenBannerEachInfo.Eng_Title;
        string jpn_Title = tempGenBannerEachInfo.Jpn_Title;

        string kor_prevAch = tempGenBannerEachInfo.Kor_prevAch;
        string eng_prevAch = tempGenBannerEachInfo.Eng_prevAch;
        string jpn_prevAch = tempGenBannerEachInfo.Jpn_prevAch;

        string spritePath = tempGenBannerEachInfo.SpritePath;
        string title = "";
        string contents = "";
        string prev = "";
        string sprite = "";
        int reward_key = tempGenBannerEachInfo.Reward_Key;

        switch(DataController.Instance.gameData.language)
        {
            case 0:
                title = kor_Title;
                contents = kor_Contents;
                prev = kor_prevAch;
                break;
            case 1:
                title = eng_Title;
                contents = eng_Contents;
                prev = eng_prevAch;
                break;
            case 2:
                title = jpn_Title;
                contents = jpn_Contents;
                prev = jpn_prevAch;
                break;
            default:
                title = eng_Title;
                contents = eng_Contents;
                prev = eng_prevAch;
                break;
        }

        /*
            업적에 관련된 보상 획득.
        */
        GameObject rewardBanner = rewardBannerprefab.gameObject;
        rewardBanner.GetComponent<AchiveBannerObject>().InitSetting();

        int newRewardCnt = 0;           //한 번도 받지 않았던 새로운 보상을 받는다면 1씩 증가한다.
        int newRewardGlassesCnt = 0;    //한 번도 받지 않았던 새로운 안경 스킨 보상을 받는다면 1씩 증가한다.
        int newRewardHatCnt = 0;        //한 번도 받지 않았던 새로운 모자 스킨 보상을 받는다면 1씩 증가한다.
        int newRewardMaskCnt = 0;       //한 번도 받지 않았던 새로운 인형탈 스킨 보상을 받는다면 1씩 증가한다.
        int newRewardTileCnt = 0;       //한 번도 받지 않았던 새로운 타일 보상을 받는다면 1씩 증가한다.
        int newRewardTitleCnt = 0;      //한 번도 받지 않았던 새로운 칭호 보상을 받는다면 1씩 증가한다.

        if(reward_key != 0 && DataController.Instance.gameData.rewardItemGeneralInfo.ContainsKey(reward_key)){ //reward_key가 0이라면 업적을 달성해도 얻을 수 있는 보상이 없다.
            for(int k = 0; k < DataController.Instance.gameData.rewardItemGeneralInfo[reward_key].Count; k++){
                int cateIndex = -1;
                Sprite itemTempSprite = null;
                switch(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].rewardCategory){
                    case RewardCategory.Glasses:
                        if(!BackendSkinManager.Instance.skinData.glassesListInt.Contains(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex)){
                            BackendSkinManager.Instance.skinData.glassesListInt.Add(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            itemTempSprite = achievementBanner.GetSpriteOnGlassesPrefabs(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex%1000);
                            newRewardCnt++;
                            newRewardGlassesCnt++;
                            cateIndex = 0;
                        }
                        break;
                    case RewardCategory.Hat:
                        if(!BackendSkinManager.Instance.skinData.hatListInt.Contains(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex)){
                            BackendSkinManager.Instance.skinData.hatListInt.Add(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            itemTempSprite = achievementBanner.GetSpriteOnHatPrefabs(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex%2000);
                            newRewardCnt++;
                            newRewardHatCnt++;
                            cateIndex = 1;
                        }
                        break;
                    
                    case RewardCategory.Mask:
                        if(!BackendSkinManager.Instance.skinData.maskListInt.Contains(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex)){
                            BackendSkinManager.Instance.skinData.maskListInt.Add(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            itemTempSprite = achievementBanner.GetSpriteOnMaskPrefabs(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex%3000);
                            newRewardCnt++;
                            newRewardMaskCnt++;
                            cateIndex = 2;
                        }
                        break;
                    case RewardCategory.Title:
                        if(!DataController.Instance.gameData.titleRewardIndexList.Contains(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex)){
                            //스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 로컬에 저장을 하도록 한다.
                            DataController.Instance.gameData.titleRewardIndexList.Add(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            itemTempSprite = achievementBanner.GetSpriteOnTitleSprite(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            newRewardCnt++;
                            newRewardTitleCnt++;
                            cateIndex = 3;
                        }
                        break;
                    case RewardCategory.Tile:
                        if(!DataController.Instance.gameData.tileRewardIndexList.Contains(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex)){
                            //스킨은 모두 서버로 백업을 하지만 칭호 및 타일들은 로컬에 저장을 하도록 한다.
                            DataController.Instance.gameData.tileRewardIndexList.Add(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            itemTempSprite = achievementBanner.GetSpriteOnTileSprite(DataController.Instance.gameData.rewardItemGeneralInfo[reward_key][k].itemIndex);
                            newRewardCnt++;
                            newRewardTileCnt++;
                            cateIndex = 4;
                        }
                        break;
                    default:
                        cateIndex = -1;
                        break;
                }
                if(k < 7){
                    if(cateIndex != -1 && itemTempSprite != null){
                        int cbIndex = !DataController.Instance.gameData.isColorFilterAssistant ? 0 : 5;
                        rewardBanner.transform.GetChild(0).GetChild(0).GetChild(k).gameObject.SetActive(true);
                        rewardBanner.transform.GetChild(0).GetChild(0).GetChild(k).gameObject.GetComponent<Image>().sprite = rewardBackground[cateIndex + cbIndex];
                        rewardBanner.transform.GetChild(0).GetChild(0).GetChild(k).GetChild(0).gameObject.GetComponent<Image>().sprite = itemTempSprite;
                    }else{
                        rewardBanner.transform.GetChild(0).transform.GetChild(0).GetChild(k).gameObject.SetActive(false);
                    }
                }
            }
        }


        prefab.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = prev;
        prefab.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>().text = title;
        prefab.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = contents;
        //achivementBannerObject//Image////////////Padding/////////////////Texts

        string path = !DataController.Instance.gameData.isColorFilterAssistant ? "AchieveImages/" + spritePath : "AchieveImages/" + spritePath +"_CB";
        
        prefab.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(path) as Sprite;;
        //achivementBannerObject//Image////////////Padding//////////////////Icon///////////////Image
        

        GameObject myInstance = Instantiate(prefab, parent); // 부모 지정 //업적 배너 생성

        if(reward_key != 0 && DataController.Instance.gameData.rewardItemGeneralInfo.ContainsKey(reward_key) && newRewardCnt != 0){ 
            GameObject rewardInstance = Instantiate(rewardBanner, parent); // 부모 지정 //업적에 따른 보상 배너 생성

            //스킨 획득 후 서버에 업데이트.         
            #if UNITY_EDITOR
                DebugX.Log("Not Update Server cause UNITY_EDITOR");

            #else
                DebugX.Log("Update Server");

                //만약 모든 스킨 카테고리에서 받았다면.
                if(newRewardGlassesCnt > 0 && newRewardHatCnt > 0 && newRewardMaskCnt > 0){
                    BackendSkinManager.Instance.UpdateAllSkinData();
                }
                else{
                    //모든 스킨 카테고리는 아니고, 그 중에서 안경 스킨은 업데이트가 있음.
                    if(newRewardGlassesCnt > 0){
                        BackendSkinManager.Instance.UpdateGlassesSkinData();
                    }
                    //모든 스킨 카테고리는 아니고, 그 중에서 모자 스킨은 업데이트가 있음.
                    if(newRewardHatCnt > 0){
                        BackendSkinManager.Instance.UpdateHatSkinData();
                    }

                    //모든 스킨 카테고리는 아니고, 그 중에서 인형탈 스킨은 업데이트가 있음.
                    if(newRewardMaskCnt > 0){
                        BackendSkinManager.Instance.UpdateMaskSkinData();
                    }
                }
            #endif
        }
        
        #if UNITY_EDITOR
            DebugX.Log("Not Update Server cause UNITY_EDITOR");
        #else
            DebugX.Log("Update Server");
            BackendAchManager.Instance.UpdateAchListData();
        #endif
        
        // 새롭게 획득한 업적이면 업적 이름 컬럼 업데이트
        if(isNewAch) {
            #if UNITY_EDITOR
                DebugX.Log("Not Update Server cause UNITY_EDITOR");
            #else
                DebugX.Log("Update Server");
                BackendAchManager.Instance.UpdateAchNameData();
            #endif
            
        }

        // Int값에 대한 업적 달성이면 Int관련 업적 데이터 컬럼 업데이트
        if(isInt) {
            #if UNITY_EDITOR
                DebugX.Log("Not Update Server cause UNITY_EDITOR");
            #else
                DebugX.Log("Update Server");
                BackendAchManager.Instance.UpdateTotIntValData();
            #endif
            
        }

        // Float관련 업적 데이터 컬럼 업데이트
        else {
            #if UNITY_EDITOR
                DebugX.Log("Not Update Server cause UNITY_EDITOR");
            #else
                DebugX.Log("Update Server");
                
                BackendAchManager.Instance.UpdateTotFloatValData();
            #endif
            
        }
    }

    public void ConfirmAchiveValue(string achieveName, int[] achiveVal, int val){
        //업적 값들 중 int 값들만 있다.
        int switchVal = Array.IndexOf(achiveVal, val);
        if(switchVal != -1){ 
            //만약 achiveVal이 {10, 20, 30, 40} 이고 val 이 8이면 switchVal이 -1이라서 함수가 실행이 되지 않는다.
            List<string> tempachieveOrderList = new List<string>();
            tempachieveOrderList = BackendAchManager.Instance.achData.achName;

            int achindex = tempachieveOrderList.IndexOf(achieveName);

            List<AchList> tempachieveEachInfo = new List<AchList>();
            tempachieveEachInfo = BackendAchManager.Instance.achData.achInfo;

            // 키 값 있는지 확인 후 키 값 있을 때만 진행하도록 변경
            if(!DataController.Instance.gameData.achGeneralInfo.ContainsKey(achieveName))
            {
                return;
            }

            AchieveGeneralEachInfo tempGenEachInfo = DataController.Instance.gameData.achGeneralInfo[achieveName][switchVal];

            if(switchVal == 0 ){
                if(achindex == -1){
                    tempachieveOrderList.Add(achieveName); // 원래 업적이름이 안들어가 있었으니 넣어준다.
                    tempachieveEachInfo.Add(new AchList());
                    achindex = tempachieveOrderList.IndexOf(achieveName);
                    tempachieveEachInfo[achindex].achList.Add(true); 
                    GenerateBanner(tempGenEachInfo, true, true);
                }
                return;
            }else if(switchVal > 0 && switchVal < 11){
                if(achindex != -1 && tempachieveEachInfo[achindex].achList.Count < switchVal+1){
                    tempachieveEachInfo[achindex].achList.Add(true); 
                    GenerateBanner(tempGenEachInfo, true, false);
                }
                return;
            }

        }
        
    }

    public void ConfirmAchiveValue(string achieveName, int switchVal){
        //업적 값들이 float 일 때 넘어오는 값들.
        
        if(switchVal != -1){ 
            List<string> tempachieveOrderList = new List<string>();
            tempachieveOrderList = BackendAchManager.Instance.achData.achName;

            int achindex = tempachieveOrderList.IndexOf(achieveName);

            List<AchList> tempachieveEachInfo = new List<AchList>();
            tempachieveEachInfo = BackendAchManager.Instance.achData.achInfo;

            

            //키 값 있는지 확인 후 키 값 있을 때만 진행하도록 변경
            if(!DataController.Instance.gameData.achGeneralInfo.ContainsKey(achieveName))
            {
                return;
            }

            AchieveGeneralEachInfo tempGenEachInfo = DataController.Instance.gameData.achGeneralInfo[achieveName][switchVal];
            
            if(switchVal == 0 ){
                if(achindex == -1){
                    tempachieveOrderList.Add(achieveName); 
                    tempachieveEachInfo.Add(new AchList());
                    achindex = tempachieveOrderList.IndexOf(achieveName);
                    tempachieveEachInfo[achindex].achList.Add(true); 
                    GenerateBanner(tempGenEachInfo, false, true);
                }
                return;
            }else if(switchVal > 0 && switchVal < 11){
                if(achindex != -1 && tempachieveEachInfo[achindex].achList.Count < switchVal+1){
                    tempachieveEachInfo[achindex].achList.Add(true); 
                    GenerateBanner(tempGenEachInfo, false, false);
                }
                return;

            }
        }
        
    }

    public void ConfirmAchiveValueForAllClear(string achieveName, int[] achiveVal, int val){
        //All clear 관련 업적들인데, int 값들임.
        int switchVal = Array.IndexOf(achiveVal, val);
        if(switchVal != -1){
            List<string> tempachieveOrderList = new List<string>();
            tempachieveOrderList = BackendAchManager.Instance.achData.achName;

            int achindex = tempachieveOrderList.IndexOf(achieveName);

            List<AchList> tempachieveEachInfo = new List<AchList>();
            tempachieveEachInfo = BackendAchManager.Instance.achData.achInfo;

            // 키 값 있는지 확인 후 키 값 있을 때만 진행하도록 변경
            if(!DataController.Instance.gameData.achGeneralInfo.ContainsKey(achieveName))
            {
                return;
            }
            
            AchieveGeneralEachInfo tempGenEachInfo = DataController.Instance.gameData.achGeneralInfo[achieveName][switchVal];
            bool isfirstGenerate = false;

            if(achindex == -1){
                tempachieveOrderList.Add(achieveName);
                tempachieveEachInfo.Add(new AchList());
                achindex = tempachieveOrderList.IndexOf(achieveName);

                for(int i = 0; i < 11; i++) {
                    /*
                        이렇게 10번 false를 넣어주는 이유:
                        다른 변수들 같은 경우에는 0에서 차근 차근 올라간다. 
                        하지만 allstageclear 같은 경우에는 1 chapter는 넘기고 2 Chapter가 먼저 클리어가 될 수도 있다.
                        따라서 먼저 10개의 false를 추가해두고 해당하는 index만 true로 바꿔주는 작업을 하는게 맞다고 판단하였다.
                    */
                    tempachieveEachInfo[achindex].achList.Add(false); 
                }
                isfirstGenerate = true;

            }

            // achList가 11개 (0 ~ Chap10)보다 작으면 false 추가
            if(tempachieveEachInfo[achindex].achList.Count < 11)
            {
                for(int i = tempachieveEachInfo[achindex].achList.Count; i < 11; i++)
                {
                    tempachieveEachInfo[achindex].achList.Add(false);
                }
            }

            if(!tempachieveEachInfo[achindex].achList[switchVal]){
                //false 였다는 것은 한 번도 업적을 안 받았다는 뜻이고, 그렇다면 아래 코드를 진행.
                tempachieveEachInfo[achindex].achList[switchVal] = true;
                GenerateBanner(tempGenEachInfo, true, isfirstGenerate);
            }
        }
        
    }

    public void ConfirmAchiveValueForJustTriggerOne(string achieveName){
        /*
            횟수, 시간, 클리어 등 누적해서 값들을 계산하는 것이 아니라 특정 조건이 실행되면 한 번 실행한다.
            예를 들어, 1-3의 특정 타일을 지난다면 이 함수가 실행되도록 한다.그렇다면 array에 들어있는지 확인하는 것이 아니라
            그냥 조건에 맞았고, 해당하는 업적을 얻었다고 판단한다. 
                => 같은 카테고리의 업적이 2개 이상이 아닐 경우
            물론 배너가 연속해서 뜨지 않도록 하기 위해 얻었는지는 확인한다.

            따라서 여기로 넘어오는 것은 업적의 이름이면 충분하다.
        */

        
        List<string> tempachieveOrderList = new List<string>();
        tempachieveOrderList = BackendAchManager.Instance.achData.achName;

        int achindex = tempachieveOrderList.IndexOf(achieveName);

        List<AchList> tempachieveEachInfo = new List<AchList>();
        tempachieveEachInfo = BackendAchManager.Instance.achData.achInfo;

        if(DataController.Instance.gameData.achGeneralInfo.ContainsKey(achieveName)){
            AchieveGeneralEachInfo tempGenEachInfo = DataController.Instance.gameData.achGeneralInfo[achieveName][0];

            if(achindex == -1){
                GenerateBanner(tempGenEachInfo, true, true); //업적이 하나 밖에 없어서 bool 값으로 해도 되지만, 서버에 저장이 되어있는 부분이 있기 때문에 int로 1,0으로 구분.
                tempachieveOrderList.Add(achieveName);
                tempachieveEachInfo.Add(new AchList());
                achindex = tempachieveOrderList.IndexOf(achieveName);
                tempachieveEachInfo[achindex].achList.Add(true); 
            }

        }
        
        
    }

}
