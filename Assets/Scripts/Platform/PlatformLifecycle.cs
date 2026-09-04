using System;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Platform;
#endif

public class PlatformLifecycle
{
    private readonly IPlatformPolicy _policy;
    private bool _gameplayActive;
    private bool _loading;

    public bool IsGameplayActive => _gameplayActive;

    public PlatformLifecycle(IPlatformPolicy policy)
    {
        _policy = policy;
    }

    public void NotifyLoadingStarted()
    {
        if (_loading)
            return;

        _loading = true;
        Send(PlatformMessageId.InGameLoadingStarted);
    }

    public void NotifyLoadingStopped()
    {
        if (_loading == false)
            return;

        _loading = false;
        Send(PlatformMessageId.InGameLoadingStopped);
    }

    public void NotifyLevelStarted(string world = null, string level = null)
    {
        _gameplayActive = true;
        Send(PlatformMessageId.LevelStarted, world, level);
    }

    public void NotifyLevelPaused(string world = null, string level = null)
    {
        if (_gameplayActive == false)
            return;

        Send(PlatformMessageId.LevelPaused, world, level);
    }

    public void NotifyLevelResumed(string world = null, string level = null)
    {
        if (_gameplayActive == false)
            return;

        Send(PlatformMessageId.LevelResumed, world, level);
    }

    public void NotifyLevelCompleted(string world = null, string level = null)
    {
        _gameplayActive = false;
        Send(PlatformMessageId.LevelCompleted, world, level);
    }

    public void NotifyLevelFailed(string world = null, string level = null)
    {
        _gameplayActive = false;
        Send(PlatformMessageId.LevelFailed, world, level);
    }

    public void NotifyAchievement()
    {
        if (_policy.AllowHappytime == false)
            return;

        Send(PlatformMessageId.PlayerGotAchievement);
    }

    private static void Send(string message, string world = null, string level = null)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null)
            return;

        if (string.IsNullOrEmpty(world) && string.IsNullOrEmpty(level))
        {
            Bridge.platform.SendMessage(Map(message));
            return;
        }

        var options = new System.Collections.Generic.Dictionary<string, object>();
        if (string.IsNullOrEmpty(world) == false)
            options["world"] = world;
        if (string.IsNullOrEmpty(level) == false)
            options["level"] = level;

        Bridge.platform.SendMessage(Map(message), options);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private static PlatformMessage Map(string message)
    {
        switch (message)
        {
            case PlatformMessageId.InGameLoadingStarted:
                return PlatformMessage.InGameLoadingStarted;
            case PlatformMessageId.InGameLoadingStopped:
                return PlatformMessage.InGameLoadingStopped;
            case PlatformMessageId.LevelStarted:
                return PlatformMessage.LevelStarted;
            case PlatformMessageId.LevelCompleted:
                return PlatformMessage.LevelCompleted;
            case PlatformMessageId.LevelFailed:
                return PlatformMessage.LevelFailed;
            case PlatformMessageId.LevelPaused:
                return PlatformMessage.LevelPaused;
            case PlatformMessageId.LevelResumed:
                return PlatformMessage.LevelResumed;
            case PlatformMessageId.PlayerGotAchievement:
                return PlatformMessage.PlayerGotAchievement;
            default:
                throw new ArgumentOutOfRangeException(nameof(message), message, null);
        }
    }
#endif

    private static class PlatformMessageId
    {
        public const string InGameLoadingStarted = "in_game_loading_started";
        public const string InGameLoadingStopped = "in_game_loading_stopped";
        public const string LevelStarted = "level_started";
        public const string LevelCompleted = "level_completed";
        public const string LevelFailed = "level_failed";
        public const string LevelPaused = "level_paused";
        public const string LevelResumed = "level_resumed";
        public const string PlayerGotAchievement = "player_got_achievement";
    }
}
