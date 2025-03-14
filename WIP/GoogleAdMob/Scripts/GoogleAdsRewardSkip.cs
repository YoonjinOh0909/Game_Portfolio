using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
/*
    *   File : GoogleAdsRewardSkip.cs
    *   Desc : skip 기능에 사용하는 30초 보상 광고 관리 코드
    *
    &   [public]
    &   : LoadRewardedAd()                             - 보상형 광고 불러오기
    &   : ShowRewardedAdSkipStage()                    - 광고 재생하기
    &   : DestroyAd()                                  - 배너 제거하기
    &
    &   [private]
    &   : RegisterEventHandlers()                      - 보상형 광고에 필요한 event 추가 및 관리하기
    *
*/
public class GoogleAdsRewardSkip : MonoBehaviour
{
    [SerializeField]
    private StageClearController stageClearController;
    private int currentAdsStage = 0;
    [SerializeField] private GameManager gm;
    
    #if UNITY_EDITOR
        private string _adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트
    #elif UNITY_ANDROID
        private string _adUnitId = "------------------------------"; // 실제
    #else
        private string _adUnitId = "unused";
    #endif

    private RewardedAd _rewardedAd;

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                    "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                        + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
            });
    }

    public void ShowRewardedAdSkipStage()
    {
        currentAdsStage = gm.currentStage + 1;
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            DataController.Instance.gameData.isOpen[currentAdsStage] = true;
            stageClearController.ResetStageClear();

            if(gm.inGamePanel.activeSelf)
            {
                gm.arcadeSkipPanel.SetActive(false);
                gm.OnSkipArcadeStage();
            }
            
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
            LoadRewardedAd();
        };
    }

}
