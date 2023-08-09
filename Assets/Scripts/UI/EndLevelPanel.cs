using InfimaGames.LowPolyShooterPack.Interface;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelPanel : Element
{
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private LevelChoicer _levelChoicer;
    [SerializeField] private SurviveTimer _surviveTimer;

    [SerializeField] private SurviveScorePanel _surviveScorePanel;
    [SerializeField] private LevelScorePanel _levelScorePanel;
    
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _settingScoreDelay;

    private void OnEnable()
    {
        Time.timeScale = 0;
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
    }

    public void Initialize(bool levelComplited)
    {
        if (_levelChoicer.SurvivalMode)
        {
            OpenSurvivePanel();
        }
        else
        {
            OpenScorePanel(levelComplited);
        }
    }

    private void OpenSurvivePanel()
    {
        _surviveScorePanel.gameObject.SetActive(true);
        _levelScorePanel.gameObject.SetActive(false);
        _surviveTimer.Stop();
        _surviveScorePanel.SetScore(_surviveTimer.SurviveTime);
    }

    private void OpenScorePanel(bool levelComplited)
    {
        _surviveScorePanel.gameObject.SetActive(false);
        _levelScorePanel.gameObject.SetActive(true);
        int levelBonus = levelComplited ? _levelChoicer.CurrentLevel.LevelBonus : 0;

        _levelScorePanel.SetScore(_moneyCollecter.Money - _moneyCollecter.StartMoney, levelBonus);
        _moneyCollecter.AddMoney(levelBonus);
    }

    private void OnRestartLevelButtonClick()
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnInMenuButtonClick()
    {
        _loadingScreen.LoadScene(0);
    }
}
