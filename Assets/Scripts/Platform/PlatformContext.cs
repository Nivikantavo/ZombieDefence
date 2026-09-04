using System;
#if UNITY_WEBGL
using Playgama;
#endif

public static class PlatformContext
{
    public static bool IsWebGlRuntime
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public static string Id
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Bridge.instance == null)
                return string.Empty;

            return Bridge.platform.id ?? string.Empty;
#else
            return string.Empty;
#endif
        }
    }

    public static bool IsCrazyGames => IsPlatform(PlatformIds.CrazyGames);

    public static bool IsPlaygama => IsPlatform(PlatformIds.Playgama);

    public static bool IsQaTool => IsPlatform(PlatformIds.QaTool);

    public static bool IsMock => IsPlatform(PlatformIds.Mock);

    public static bool UsesGamCurrency => IsPlaygama || IsQaTool || IsMock;

    public static bool IsStoreApp
    {
        get
        {
            string applicationType = GetExtra(PlatformIds.ApplicationTypeKey);
            if (string.IsNullOrEmpty(applicationType))
                return false;

            return applicationType.Equals(PlatformIds.GooglePlayStore, StringComparison.OrdinalIgnoreCase)
                || applicationType.Equals(PlatformIds.AppleStore, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static bool IsIapUiAllowed => IsStoreApp == false;

    public static bool IsPlatform(string platformId)
    {
        if (string.IsNullOrEmpty(platformId))
            return false;

        string id = Id;
        return string.IsNullOrEmpty(id) == false
            && id.IndexOf(platformId, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static string GetExtra(string key)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null || string.IsNullOrEmpty(key))
            return null;

        var extra = Bridge.player.extra;
        if (extra == null)
            return null;

        return extra.TryGetValue(key, out string value) ? value : null;
#else
        return null;
#endif
    }
}
