using GameAnalyticsSDK;

public static class GameAnalyticsAds
{
    private const string SdkName = "playgama";
    private const string DefaultPlacement = "unknown";

    public static class Placement
    {
        public const string LevelEnd = "level_end";
        public const string LevelEndRestart = "level_end_restart";
        public const string LevelEndNext = "level_end_next";
        public const string LevelEndMenu = "level_end_menu";
        public const string LevelEndDoubleReward = "level_end_double_reward";
        public const string AdAskMoney = "ad_ask_money";
        public const string DesertirRestart = "desertir_restart";
        public const string DesertirMenu = "desertir_menu";
    }

    public static void Request(GAAdType adType, string placement)
    {
        Send(GAAdAction.Request, adType, placement);
    }

    public static void Show(GAAdType adType, string placement)
    {
        Send(GAAdAction.Show, adType, placement);
    }

    public static void FailedShow(GAAdType adType, string placement)
    {
        if (GameAnalytics.Initialized == false)
            return;

        GameAnalytics.NewAdEvent(
            GAAdAction.FailedShow,
            adType,
            SdkName,
            NormalizePlacement(placement),
            GAAdError.Unknown);
    }

    public static void RewardReceived(string placement)
    {
        Send(GAAdAction.RewardReceived, GAAdType.RewardedVideo, placement);
    }

    private static void Send(GAAdAction action, GAAdType adType, string placement)
    {
        if (GameAnalytics.Initialized == false)
            return;

        GameAnalytics.NewAdEvent(action, adType, SdkName, NormalizePlacement(placement));
    }

    private static string NormalizePlacement(string placement)
    {
        return string.IsNullOrWhiteSpace(placement) ? DefaultPlacement : placement;
    }
}
