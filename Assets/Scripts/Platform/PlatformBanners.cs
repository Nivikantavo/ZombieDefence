using System;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
#endif

public class PlatformBanners
{
    private readonly IPlatformPolicy _policy;
    private string _currentPlacement;

    public PlatformBanners(IPlatformPolicy policy)
    {
        _policy = policy;
    }

    public bool IsSupported
    {
        get
        {
            if (_policy.AdsEnabled == false)
                return false;

#if UNITY_WEBGL && !UNITY_EDITOR
            if (Bridge.instance == null)
                return false;

            return (_policy.UseAdvancedBanners && Bridge.advertisement.isAdvancedBannersSupported)
                || Bridge.advertisement.isBannerSupported;
#else
            return false;
#endif
        }
    }

    public void ShowMainMenu()
    {
        ShowIdle(BannerPlacements.MainMenu);
    }

    public void ShowPause()
    {
        ShowIdle(BannerPlacements.LevelPaused);
    }

    public void ShowEndgame(bool levelCompleted)
    {
        ShowIdle(levelCompleted ? BannerPlacements.LevelCompleted : BannerPlacements.LevelFailed);
    }

    public void Hide()
    {
        _currentPlacement = null;
        BannerLayoutGuard.Apply(false);
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null)
            return;

        if (Bridge.advertisement.isAdvancedBannersSupported)
            Bridge.advertisement.HideAdvancedBanners();

        if (Bridge.advertisement.isBannerSupported)
            Bridge.advertisement.HideBanner();
#endif
    }

    private void ShowIdle(string placement)
    {
        if (IsSupported == false)
            return;

        _currentPlacement = placement;
        BannerLayoutGuard.Apply(_policy.UseAdvancedBanners);
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null)
            return;

        if (_policy.UseAdvancedBanners && Bridge.advertisement.isAdvancedBannersSupported)
        {
            Bridge.advertisement.ShowAdvancedBanners(placement);
            return;
        }

        if (Bridge.advertisement.isBannerSupported)
            Bridge.advertisement.ShowBanner(BannerPosition.Top, placement);
#endif
    }
}
