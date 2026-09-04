public class DefaultPlatformPolicy : IPlatformPolicy
{
    public bool AdsEnabled => true;

    public bool ShowInterstitialBeforeEndgame => false;

    public bool ShowInterstitialOnNavigationButtons => true;

    public bool UseAdvancedBanners => false;

    public bool RequiresAuthForIap => false;

    public bool IapEnabled => true;

    public bool AllowHappytime => false;

    public int InterstitialMinimumDelaySeconds => 60;
}
