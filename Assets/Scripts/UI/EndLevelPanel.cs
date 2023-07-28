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
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);

        if (_levelChoicer.SurvivalMode)
        {
            _surviveScorePanel.gameObject.SetActive(true);
            _levelScorePanel.gameObject.SetActive(false);
            _surviveTimer.Stop();
            _surviveScorePanel.SetScore(_surviveTimer.SurviveTime);
        }
        else
        {
            _surviveScorePanel.gameObject.SetActive(false);
            _levelScorePanel.gameObject.SetActive(true);
            _levelScorePanel.SetScore(_moneyCollecter.Money - _moneyCollecter.StartMoney, _levelChoicer.CurrentLevel.LevelBonus);
            _moneyCollecter.AddMoney(_levelChoicer.CurrentLevel.LevelBonus);
        }
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
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
