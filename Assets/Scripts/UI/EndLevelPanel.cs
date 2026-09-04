using System;
using InfimaGames.LowPolyShooterPack.Interface;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelPanel : Element
{
    private const string WinText = "LevelEnd";
    private const string LostText = "LevelLost";
    private const int MainMenuSceneId = 0;

    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private SurviveTimer _surviveTimer;
    [SerializeField] private Track _track;
    [SerializeField] private InBackgroundCheker _backgroundCheker;

    [SerializeField] private SurviveScorePanel _surviveScorePanel;
    [SerializeField] private LevelScorePanel _levelScorePanel;
    
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _rewardButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private LeanLocalizedTextMeshProUGUI _labelText;
    [SerializeField] private GameObject _adErrorPanel;
    [SerializeField] private float _settingScoreDelay;

    private DifficultyChoicer _difficultyChoicer;
    private NextLevelLauncher _nextLevelLauncher;
    private int _levelBonus;
    private bool _wasRewarded;

    public bool BlocksPause { get; private set; }

    public event UnityAction RewardAdClose;

    public void SetPauseBlocked(bool blocked)
    {
        BlocksPause = blocked;
        if (blocked == false)
            return;

        UIInput uiInput = GetComponentInParent<UIInput>();
        uiInput?.ForceClosePause();
    }

    protected override void Awake()
    {
        base.Awake();
        _nextLevelLauncher = CreateNextLevelLauncher();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        _rewardButton.onClick.AddListener(OnRewardButtonClick);
        _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
        _rewardButton.onClick.RemoveListener(OnRewardButtonClick);
        _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
    }

    public void Initialize(bool levelComplited)
    {
        _nextLevelButton.gameObject.SetActive(CanGoToNextLevel(levelComplited));

        if (_difficultyChoicer.SurvivalMode)
        {
            OpenSurvivePanel();
        }
        else
        {
            _labelText.TranslationName = levelComplited ? WinText : LostText;
            OpenScorePanel(levelComplited);
        }

        bool rewardedSupported = PlatformServices.Ads == null || PlatformServices.Ads.IsRewardedSupported;
        if (_rewardButton != null && _difficultyChoicer.SurvivalMode == false)
            _rewardButton.gameObject.SetActive(rewardedSupported);
    }

    public void SetCurrentLevel(DifficultyChoicer difficultyChoicer)
    {
        _difficultyChoicer = difficultyChoicer;
    }

    private NextLevelLauncher CreateNextLevelLauncher()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError($"{nameof(SaveSystem)} is missing, the next level can not be started.");
            return null;
        }

        return new NextLevelLauncher(
            new LevelSequence(PlayerData.StagesCount, Stage.LevelsPerStage),
            new SaveSystemLevelSelection(SaveSystem.Instance),
            _loadingScreen,
            SceneManager.GetActiveScene().buildIndex);
    }

    private bool CanGoToNextLevel(bool levelComplited)
    {
        return levelComplited
            && _difficultyChoicer.SurvivalMode == false
            && _nextLevelLauncher != null
            && _nextLevelLauncher.HasNextLevel;
    }

    private void OpenSurvivePanel()
    {
        _surviveScorePanel.gameObject.SetActive(true);
        _levelScorePanel.gameObject.SetActive(false);
        _rewardButton.gameObject.SetActive(false);
        _surviveTimer.Stop();
        _surviveScorePanel.SetScore(_surviveTimer.SurviveTime);
    }

    private void OpenScorePanel(bool levelComplited)
    {
        _surviveScorePanel.gameObject.SetActive(false);
        _levelScorePanel.gameObject.SetActive(true);
        
        _levelBonus = levelComplited ? _difficultyChoicer.CurrentLevel.LevelBonus : 0;
        _levelScorePanel.SetScore(_moneyCollecter.Money - _moneyCollecter.StartMoney, _levelBonus);
        _moneyCollecter.AddMoney(_levelBonus);
    }

    private void OnRestartLevelButtonClick()
    {
        RunAfterNavigationAd(GameAnalyticsAds.Placement.LevelEndRestart, LoadRestart);
    }

    private void OnNextLevelButtonClick()
    {
        _nextLevelButton.interactable = false;
        RunAfterNavigationAd(GameAnalyticsAds.Placement.LevelEndNext, LoadNextLevel);
    }

    private void OnInMenuButtonClick()
    {
        RunAfterNavigationAd(GameAnalyticsAds.Placement.LevelEndMenu, LoadMainMenu);
    }

    private void OnRewardButtonClick()
    {
        _rewardButton.interactable = false;
        if (PlatformServices.Ads != null)
        {
            PlatformServices.Ads.ShowRewarded(
                OnRewardCallback,
                OnRewardAdClose,
                OnRewardAdError,
                GameAnalyticsAds.Placement.LevelEndDoubleReward);
            return;
        }

        PlaygamaAds.ShowRewarded(null, OnRewardCallback, OnRewardAdClose, OnRewardAdError, GameAnalyticsAds.Placement.LevelEndDoubleReward);
    }

    private void OnRewardCallback()
    {
        _moneyCollecter.AddMoney(_moneyCollecter.Money - _moneyCollecter.StartMoney);
        _rewardButton.interactable = false;
        _wasRewarded = true;
    }

    private void RunAfterNavigationAd(string placement, Action onComplete)
    {
        if (_wasRewarded || PlatformServices.Ads == null)
        {
            onComplete?.Invoke();
            return;
        }

        PlatformServices.Ads.ShowInterstitialOnNavigation(onComplete, placement);
    }

    private void LoadRestart()
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        _loadingScreen.LoadScene(MainMenuSceneId);
    }

    private void LoadNextLevel()
    {
        if (_nextLevelLauncher.TryLaunch() == false)
        {
            Debug.LogWarning("There is no level after the finished one, returning to the main menu.");
            LoadMainMenu();
        }
    }

    private void OnRewardAdClose()
    {
        RewardAdClose?.Invoke();
    }

    private void OnRewardAdError(string error)
    {
        _rewardButton.interactable = true;
        if (_adErrorPanel != null)
            _adErrorPanel.SetActive(false);
        Debug.Log(error);
    }
}
