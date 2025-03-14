using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/*
    *   File : GoogleAdsMobileBanner.cs
    *   Desc : 적응형 배너 광고 관리 코드
    *
    &   [public]
    &   : CreateBannerView()                 - 적응형 배너(이미지) 생성
    &   : LoadAd()                           - 광고 불러오기
    &   : ControlInvoke()                    - LoadAd 함수를 Invoke 여부 제어
    &   : DestroyAd()                        - 배너 제거하기
    &
    &   [private]
    &   : ListenToAdEvents()                 - 배너 광고에 필요한 event 추가 및 관리하기
    *
*/

public class GoogleAdsMobileBanner : MonoBehaviour
{
    #if UNITY_EDITOR
        string _adUnitId = "ca-app-pub-3940256099942544/6300978111"; // 테스트
    #elif UNITY_ANDROID
        private string _adUnitId = "------------------------------"; // 실제
    #else
        private string _adUnitId = "unused";
    #endif

    BannerView _bannerView;

    public void CreateBannerView()
    {
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // 적응형 배너 광고 사이즈 지정
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Top);
    }

    public void LoadAd()
    {
        if(!DataController.Instance.gameData.isRemoveAds){
            if (_bannerView != null)
            {
                DestroyAd();
            }
            if(_bannerView == null)
            {
                CreateBannerView();
            }
            
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            _bannerView.LoadAd(adRequest);
        }else{
            CancelInvoke("LoadAd");
        }
         
    }

    public void ControlInvoke(bool temp){
        if(temp){
            InvokeRepeating("LoadAd", 1f, 10f);
        }else{
            CancelInvoke("LoadAd");
        }
    }
   
    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
}
