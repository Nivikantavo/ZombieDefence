using Agava.YandexGames;
using InfimaGames.LowPolyShooterPack.Interface;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelPanel : Element
{
    private const string WinText = "LevelEnd";
    private const string LostText = "LevelLost";

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
    [SerializeField] private LeanLocalizedTextMeshProUGUI _labelText;
    [SerializeField] private float _settingScoreDelay;

    private DifficultyChoicer _difficultyChoicer;
    private int _levelBonus;
    private bool _wasRewarded = false;
    

    public event UnityAction RewardAdClose;

    private void OnEnable()
    {
        Time.timeScale = 0;
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        _rewardButton.onClick.AddListener(OnRewardButtonClick);
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
        _rewardButton.onClick.RemoveListener(OnRewardButtonClick);
    }

    public void Initialize(bool levelComplited)
    {
        if (_difficultyChoicer.SurvivalMode)
        {
            OpenSurvivePanel();
        }
        else
        {
            _labelText.TranslationName = levelComplited ? WinText : LostText;
            OpenScorePanel(levelComplited);
        }
    }

    public void SetCurrentLevel(DifficultyChoicer difficultyChoicer)
    {
        _difficultyChoicer = difficultyChoicer;
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
        
        float trackHealthInPercent = Mathf.InverseLerp(0 , _track.MaxHealth, _track.CurrentHealth);
        _levelBonus = levelComplited ? Mathf.RoundToInt(_difficultyChoicer.CurrentLevel.LevelBonus * trackHealthInPercent) : 0;
        _levelScorePanel.SetScore(_moneyCollecter.Money - _moneyCollecter.StartMoney, _levelBonus);
        _moneyCollecter.AddMoney(_levelBonus);
    }

    private void OnRestartLevelButtonClick()
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
        if(_wasRewarded == false)
        {
            InterstitialAd.Show(OnAdOpen, OnRestartAdClose, OnRestartAdError);
        }
        else
        {
            _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
//#endif
    }

    private void OnInMenuButtonClick()
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
        if(_wasRewarded == false)
        {
            InterstitialAd.Show(OnAdOpen, OnAdClose);
        }
//#endif
        _loadingScreen.LoadScene(0);
    }

    private void OnRewardButtonClick()
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
        VideoAd.Show(OnAdOpen, OnRewardCallback, OnRewardAdClose);
//#endif
    }

    private void OnAdOpen()
    {
        _backgroundCheker.SetAdsShown(true);
        InputSystem.DisableDevice(Keyboard.current);
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }

    private void OnRewardCallback()
    {
        _moneyCollecter.AddMoney(_moneyCollecter.Money - _moneyCollecter.StartMoney);
        _rewardButton.interactable = false;
        _wasRewarded = true;
    }

    private void OnAdClose(bool wasShown = true)
    {
        _backgroundCheker.SetAdsShown(false);
        InputSystem.EnableDevice(Keyboard.current);
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnRestartAdClose(bool wasShown = true)
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
        _backgroundCheker.SetAdsShown(false);
        InputSystem.EnableDevice(Keyboard.current);
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnRestartAdError(string error)
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
        _backgroundCheker.SetAdsShown(false);
        InputSystem.EnableDevice(Keyboard.current);
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnRewardAdClose()
    {
        OnAdClose();
        RewardAdClose?.Invoke();
    }
}
