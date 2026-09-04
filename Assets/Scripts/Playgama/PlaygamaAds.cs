using System;
using GameAnalyticsSDK;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
#endif

public static class PlaygamaAds
{
    public static bool IsInterstitialSupported
    {
        get
        {
#if UNITY_WEBGL
            return Bridge.instance != null && Bridge.advertisement.isInterstitialSupported;
#else
            return false;
#endif
        }
    }

    public static bool IsRewardedSupported
    {
        get
        {
#if UNITY_WEBGL
            return Bridge.instance != null && Bridge.advertisement.isRewardedSupported;
#else
            return false;
#endif
        }
    }

    public static bool IsBannerSupported
    {
        get
        {
#if UNITY_WEBGL
            return Bridge.instance != null && Bridge.advertisement.isBannerSupported;
#else
            return false;
#endif
        }
    }

    public static void ShowInterstitial(
        Action onOpen = null,
        Action<bool> onClose = null,
        Action<string> onError = null,
        string placement = null)
    {
#if UNITY_WEBGL
        if (Bridge.instance == null || Bridge.advertisement.isInterstitialSupported == false)
        {
            onClose?.Invoke(false);
            return;
        }

        GameAnalyticsAds.Request(GAAdType.Interstitial, placement);
        Action<InterstitialState> handler = null;
        handler = state =>
        {
            switch (state)
            {
                case InterstitialState.Opened:
                    GameAnalyticsAds.Show(GAAdType.Interstitial, placement);
                    PlaygamaAdPause.Begin();
                    onOpen?.Invoke();
                    break;
                case InterstitialState.Closed:
                    Bridge.advertisement.interstitialStateChanged -= handler;
                    PlaygamaAdPause.End();
                    onClose?.Invoke(true);
                    break;
                case InterstitialState.Failed:
                    Bridge.advertisement.interstitialStateChanged -= handler;
                    GameAnalyticsAds.FailedShow(GAAdType.Interstitial, placement);
                    PlaygamaAdPause.End();
                    onClose?.Invoke(false);
                    onError?.Invoke("Interstitial ad failed");
                    break;
            }
        };

        Bridge.advertisement.interstitialStateChanged += handler;
        Bridge.advertisement.ShowInterstitial(placement);
#else
        onClose?.Invoke(false);
#endif
    }

    public static void ShowRewarded(
        Action onOpen = null,
        Action onRewarded = null,
        Action onClose = null,
        Action<string> onError = null,
        string placement = null)
    {
#if UNITY_WEBGL
        if (Bridge.instance == null || Bridge.advertisement.isRewardedSupported == false)
        {
            onError?.Invoke("Rewarded ad is not supported");
            return;
        }

        GameAnalyticsAds.Request(GAAdType.RewardedVideo, placement);
        bool rewarded = false;
        Action<RewardedState> handler = null;
        handler = state =>
        {
            switch (state)
            {
                case RewardedState.Opened:
                    GameAnalyticsAds.Show(GAAdType.RewardedVideo, placement);
                    PlaygamaAdPause.Begin();
                    onOpen?.Invoke();
                    break;
                case RewardedState.Rewarded:
                    rewarded = true;
                    GameAnalyticsAds.RewardReceived(placement);
                    onRewarded?.Invoke();
                    break;
                case RewardedState.Closed:
                    Bridge.advertisement.rewardedStateChanged -= handler;
                    PlaygamaAdPause.End();
                    onClose?.Invoke();
                    break;
                case RewardedState.Failed:
                    Bridge.advertisement.rewardedStateChanged -= handler;
                    PlaygamaAdPause.End();
                    if (rewarded == false)
                    {
                        GameAnalyticsAds.FailedShow(GAAdType.RewardedVideo, placement);
                        onError?.Invoke("Rewarded ad failed");
                    }
                    else
                    {
                        onClose?.Invoke();
                    }
                    break;
            }
        };

        Bridge.advertisement.rewardedStateChanged += handler;
        Bridge.advertisement.ShowRewarded(placement);
#else
        onOpen?.Invoke();
        onRewarded?.Invoke();
        onClose?.Invoke();
#endif
    }

    public static void CheckAdBlock(Action<bool> onComplete)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null)
        {
            onComplete?.Invoke(false);
            return;
        }

        Bridge.advertisement.CheckAdBlock(blocked => onComplete?.Invoke(blocked));
#else
        onComplete?.Invoke(false);
#endif
    }

    public static bool IsMobileDevice()
    {
#if UNITY_WEBGL
        if (Bridge.instance == null)
            return Application.isMobilePlatform;

        var type = Bridge.device.type;
        return type == Playgama.Modules.Device.DeviceType.Mobile
            || type == Playgama.Modules.Device.DeviceType.Tablet;
#else
        return Application.isMobilePlatform;
#endif
    }
}
