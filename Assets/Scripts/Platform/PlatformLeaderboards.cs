using System;
using System.Collections.Generic;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Leaderboards;
#endif

public class PlatformLeaderboards
{
    public bool IsAvailable
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Bridge.instance == null)
                return false;

            LeaderboardType type = Bridge.leaderboards.type;
            return type == LeaderboardType.InGame
                || type == LeaderboardType.Native
                || type == LeaderboardType.NativePopup;
#else
            return false;
#endif
        }
    }

    public bool CanReadEntries
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Bridge.instance != null && Bridge.leaderboards.type == LeaderboardType.InGame;
#else
            return false;
#endif
        }
    }

    public void SetScore(string leaderboardId, int score, Action<bool> onComplete = null)
    {
        if (string.IsNullOrEmpty(leaderboardId) || IsAvailable == false)
        {
            onComplete?.Invoke(false);
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.leaderboards.SetScore(leaderboardId, score, success =>
        {
            if (success == false)
                UnityEngine.Debug.LogWarning($"Leaderboard SetScore failed: {leaderboardId}={score}");

            onComplete?.Invoke(success);
        });
#else
        onComplete?.Invoke(false);
#endif
    }

    public void GetEntries(string leaderboardId, Action<bool, List<Dictionary<string, string>>> onComplete)
    {
        if (string.IsNullOrEmpty(leaderboardId) || CanReadEntries == false)
        {
            onComplete?.Invoke(false, null);
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.leaderboards.GetEntries(leaderboardId, onComplete);
#else
        onComplete?.Invoke(false, null);
#endif
    }
}
