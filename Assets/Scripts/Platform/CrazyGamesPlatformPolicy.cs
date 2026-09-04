public class CrazyGamesPlatformPolicy : IPlatformPolicy
{
    // CrazyGames base-launch submissions cannot include ads.
    // Set to true after approval to restore interstitial, rewarded, and banner ads.
    public bool AdsEnabled => false;

    public bool ShowInterstitialBeforeEndgame => AdsEnabled;

    public bool ShowInterstitialOnNavigationButtons => false;

    public bool UseAdvancedBanners => AdsEnabled;

    public bool RequiresAuthForIap => true;

    public bool IapEnabled => false;

    public bool AllowHappytime => true;

    public int InterstitialMinimumDelaySeconds => 0;
}
