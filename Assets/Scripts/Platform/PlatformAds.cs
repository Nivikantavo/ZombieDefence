using System;
#if UNITY_WEBGL
using Playgama;
#endif

public class PlatformAds
{
    private readonly IPlatformPolicy _policy;

    public PlatformAds(IPlatformPolicy policy)
    {
        _policy = policy;
    }

    public bool IsInterstitialSupported => _policy.AdsEnabled && PlaygamaAds.IsInterstitialSupported;

    public bool IsRewardedSupported => _policy.AdsEnabled && PlaygamaAds.IsRewardedSupported;

    public void ShowInterstitialBeforeEndgame(Action onComplete)
    {
        ShowIfAllowed(_policy.ShowInterstitialBeforeEndgame, onComplete, GameAnalyticsAds.Placement.LevelEnd);
    }

    public void ShowInterstitialOnNavigation(Action onComplete, string placement)
    {
        ShowIfAllowed(_policy.ShowInterstitialOnNavigationButtons, onComplete, placement);
    }

    public void ShowRewarded(
        Action onRewarded,
        Action onClose,
        Action<string> onError,
        string placement)
    {
        if (_policy.AdsEnabled == false || IsRewardedSupported == false)
        {
            onError?.Invoke("Rewarded ad is not supported");
            return;
        }

        PlaygamaAds.ShowRewarded(null, onRewarded, onClose, onError, placement);
    }

    private void ShowIfAllowed(bool enabled, Action onComplete, string placement)
    {
        if (_policy.AdsEnabled == false || enabled == false || IsInterstitialSupported == false)
        {
            onComplete?.Invoke();
            return;
        }

        AdBlockOverlay.Show();
        PlaygamaAds.ShowInterstitial(
            null,
            wasShown =>
            {
                AdBlockOverlay.Hide();
                onComplete?.Invoke();
            },
            error =>
            {
                AdBlockOverlay.Hide();
                onComplete?.Invoke();
            },
            placement);
    }

    public void ApplyInterstitialDelay()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (_policy.AdsEnabled == false || Bridge.instance == null)
            return;

        Bridge.advertisement.SetMinimumDelayBetweenInterstitial(_policy.InterstitialMinimumDelaySeconds);
#endif
    }
}
