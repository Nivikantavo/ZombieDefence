using System;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
#endif

public static class PlaygamaAds
{
    public static void ShowInterstitial(
        Action onOpen = null,
        Action<bool> onClose = null,
        Action<string> onError = null,
        string placement = null)
    {
#if UNITY_WEBGL
        Action<InterstitialState> handler = null;
        handler = state =>
        {
            switch (state)
            {
                case InterstitialState.Opened:
                    onOpen?.Invoke();
                    break;
                case InterstitialState.Closed:
                    Bridge.advertisement.interstitialStateChanged -= handler;
                    onClose?.Invoke(true);
                    break;
                case InterstitialState.Failed:
                    Bridge.advertisement.interstitialStateChanged -= handler;
                    onError?.Invoke("Interstitial ad failed");
                    break;
            }
        };

        Bridge.advertisement.interstitialStateChanged += handler;
        Bridge.advertisement.ShowInterstitial(placement);
#else
        onOpen?.Invoke();
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
        bool rewarded = false;
        Action<RewardedState> handler = null;
        handler = state =>
        {
            switch (state)
            {
                case RewardedState.Opened:
                    onOpen?.Invoke();
                    break;
                case RewardedState.Rewarded:
                    rewarded = true;
                    onRewarded?.Invoke();
                    break;
                case RewardedState.Closed:
                    Bridge.advertisement.rewardedStateChanged -= handler;
                    onClose?.Invoke();
                    break;
                case RewardedState.Failed:
                    Bridge.advertisement.rewardedStateChanged -= handler;
                    if (rewarded == false)
                        onError?.Invoke("Rewarded ad failed");
                    else
                        onClose?.Invoke();
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

    public static bool IsMobileDevice()
    {
#if UNITY_WEBGL
        var type = Bridge.device.type;
        return type == Playgama.Modules.Device.DeviceType.Mobile
            || type == Playgama.Modules.Device.DeviceType.Tablet;
#else
        return Application.isMobilePlatform;
#endif
    }
}
