using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    * File : SkinItemButton.cs
    * Desc : 세팅창에서 각 스킨에 맞게 적용하는 기능.
    *
    & 
    &   [public]
    &   : AdaptOnlyonPanel()                - 각 카테고리에 맞는 아이템 index 설정
    &   : SetInitValue(...)                 - 스킨 버튼의 기본 세팅 진행. 이미지, index 등을 설정
    &   : CheckNowIndex()                   - 현재 설정된 값을 확인하고, 선택한 값이 기존과 같다면 스킨 해제하는 기능
    &   : MoveRect()                        - 각 카테고리의 스킨의 개수에 따라 Scroll Rect 설정.
    *
*/

public class SkinItemButton : MonoBehaviour
{
    public SetButtonForRewardItemPanel setButtonForRewardItemPanel;
    public RewardCategory rewardCategory;
    public int selectIndex;
    public Image image;

    public GameObject checkMark;

    public int orderIndex = 0;
 
    void Start()
    {
        CheckNowIndex();
    }

    public void AdaptOnlyonPanel(){
        int itemIndex = 1000;

        switch(rewardCategory){
            case RewardCategory.Glasses:
                if(setButtonForRewardItemPanel.selectGlassesIndex == selectIndex){
                    setButtonForRewardItemPanel.selectGlassesIndex = -1;
                }else{
                    if(selectIndex != -1){
                        itemIndex = selectIndex + 1000;
                    }
                    setButtonForRewardItemPanel.selectGlassesIndex = selectIndex;
                }
                break;

            case RewardCategory.Hat:
                if(setButtonForRewardItemPanel.selectHatIndex == selectIndex){
                    setButtonForRewardItemPanel.selectHatIndex = -1;
                }else{
                    if(selectIndex != -1){
                        itemIndex = selectIndex + 2000;
                    } 
                    setButtonForRewardItemPanel.selectHatIndex = selectIndex;
                }
                break;
            case RewardCategory.Mask:
                if(setButtonForRewardItemPanel.selectMaskIndex == selectIndex){
                    setButtonForRewardItemPanel.selectMaskIndex = -1;
                }else{
                    if(selectIndex != -1){
                        itemIndex = selectIndex + 3000;
                    }
                    
                    setButtonForRewardItemPanel.selectMaskIndex = selectIndex;
                }
                
                break;
            default:
                break;
        }

        setButtonForRewardItemPanel.ChangeItemExp(itemIndex.ToString());
        setButtonForRewardItemPanel.AdaptNowSetting();
    }

    public void SetInitValue(SetButtonForRewardItemPanel tempSet, RewardCategory category, int tempSelectIndex, Sprite tempSprite, int tempOrder){
        setButtonForRewardItemPanel = tempSet;
        rewardCategory = category;
        selectIndex = tempSelectIndex;
        if(tempSprite != null){
            image.sprite = tempSprite;
        }
        orderIndex = tempOrder;
    }

    public void CheckNowIndex(){
        bool turnOn = false;
        switch(rewardCategory){
            case RewardCategory.Hat:
                turnOn = (DataController.Instance.gameData.currentHatIndex == (selectIndex-1));
                break;
            case RewardCategory.Glasses:
                turnOn = DataController.Instance.gameData.currentGlassesIndex == (selectIndex -1);
                break;
            case RewardCategory.Mask:
                turnOn = DataController.Instance.gameData.currentMaskIndex == (selectIndex -1);
                break;
            default:
                break;
        }
        checkMark.SetActive(turnOn);
    }

    public void MoveRect(){
        setButtonForRewardItemPanel.SetRect(orderIndex, rewardCategory);
    }
}
