using System;
#if UNITY_WEBGL
using Playgama;
#endif

public class PlatformAuth
{
    private readonly IPlatformPolicy _policy;
    private bool _authorizing;
    private bool _wasAuthorized;

    public event Action Authorized;

    public PlatformAuth(IPlatformPolicy policy)
    {
        _policy = policy;
    }

    public bool IsAuthorizationSupported
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Bridge.instance != null && Bridge.player.isAuthorizationSupported;
#else
            return false;
#endif
        }
    }

    public bool IsAuthorized
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Bridge.instance != null && Bridge.player.isAuthorized;
#else
            return false;
#endif
        }
    }

    public bool RequiresAuthForIap => _policy.RequiresAuthForIap && IsAuthorizationSupported;

    public string GetUserToken()
    {
        return FirstExtra("token", "userToken", "jwt", "user_token");
    }

    public string GetXsollaUserToken()
    {
        string token = FirstExtra("xsollaUserToken", "xsolla_token", "xsollaToken");
        return string.IsNullOrEmpty(token) ? GetUserToken() : token;
    }

    public void ShowAuthPrompt(Action<bool> onComplete = null)
    {
        if (_authorizing)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.instance == null)
        {
            onComplete?.Invoke(false);
            return;
        }

        _authorizing = true;
        Bridge.player.Authorize(new System.Collections.Generic.Dictionary<string, object>(), success =>
        {
            _authorizing = false;
            onComplete?.Invoke(success);
            if (success)
            {
                _wasAuthorized = true;
                Authorized?.Invoke();
            }
        });
#else
        onComplete?.Invoke(false);
#endif
    }

    public void RefreshAuthorizationState()
    {
        bool authorized = IsAuthorized;
        if (authorized && _wasAuthorized == false)
            Authorized?.Invoke();

        _wasAuthorized = authorized;
    }

    private static string FirstExtra(params string[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            string value = PlatformContext.GetExtra(keys[i]);
            if (string.IsNullOrEmpty(value) == false)
                return value;
        }

        return null;
    }
}
