public interface IPlatformPolicy
{
    bool AdsEnabled { get; }

    bool ShowInterstitialBeforeEndgame { get; }

    bool ShowInterstitialOnNavigationButtons { get; }

    bool UseAdvancedBanners { get; }

    bool RequiresAuthForIap { get; }

    bool IapEnabled { get; }

    bool AllowHappytime { get; }

    int InterstitialMinimumDelaySeconds { get; }
}
