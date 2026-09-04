using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_WEBGL
using Playgama;
#endif

public class PlatformServices : MonoBehaviour
{
    public static PlatformServices Instance { get; private set; }

    public static IPlatformPolicy Policy => Instance != null ? Instance._policy : _fallbackPolicy;

    public static PlatformAds Ads => Instance != null ? Instance._ads : null;

    public static PlatformBanners Banners => Instance != null ? Instance._banners : null;

    public static PlatformLifecycle Lifecycle => Instance != null ? Instance._lifecycle : null;

    public static PlatformAuth Auth => Instance != null ? Instance._auth : null;

    public static PlatformLeaderboards Leaderboards => Instance != null ? Instance._leaderboards : null;

    private static readonly IPlatformPolicy _fallbackPolicy = new DefaultPlatformPolicy();

    private IPlatformPolicy _policy;
    private PlatformAds _ads;
    private PlatformBanners _banners;
    private PlatformLifecycle _lifecycle;
    private PlatformAuth _auth;
    private PlatformLeaderboards _leaderboards;
    private bool _initialLoadFinished;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject host = new GameObject(nameof(PlatformServices));
        DontDestroyOnLoad(host);
        host.AddComponent<PlatformServices>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CreateServices(null);
    }

    private IEnumerator Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        while (Bridge.instance == null)
            yield return null;

        IPlatformPolicy nextPolicy = PlatformContext.IsCrazyGames
            ? (IPlatformPolicy)new CrazyGamesPlatformPolicy()
            : new DefaultPlatformPolicy();
        if (_policy == null || _policy.GetType() != nextPolicy.GetType())
            CreateServices(nextPolicy);

        _ads.ApplyInterstitialDelay();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (_policy.AdsEnabled)
                _banners.ShowMainMenu();
            else
                _banners.Hide();

            if (_initialLoadFinished == false)
                _lifecycle.NotifyLoadingStarted();
        }
#else
        yield break;
#endif
    }

    public void NotifyInitialLoadComplete()
    {
        _initialLoadFinished = true;
        _lifecycle?.NotifyLoadingStopped();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _auth != null)
            _auth.RefreshAuthorizationState();
    }

    private void CreateServices(IPlatformPolicy policy)
    {
        if (policy == null)
        {
#if UNITY_WEBGL
            policy = PlatformContext.IsCrazyGames
                ? (IPlatformPolicy)new CrazyGamesPlatformPolicy()
                : new DefaultPlatformPolicy();
#else
            policy = new DefaultPlatformPolicy();
#endif
        }

        _policy = policy;
        _ads = new PlatformAds(_policy);
        _banners = new PlatformBanners(_policy);
        _lifecycle = new PlatformLifecycle(_policy);
        _auth = new PlatformAuth(_policy);
        _leaderboards = new PlatformLeaderboards();
        _ads.ApplyInterstitialDelay();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_lifecycle != null)
            _lifecycle.NotifyLoadingStopped();

        if (_banners == null)
            return;

        if (scene.buildIndex == 0)
        {
            if (_policy.AdsEnabled)
                _banners.ShowMainMenu();
            else
                _banners.Hide();
            return;
        }

        _banners.Hide();
    }
}
