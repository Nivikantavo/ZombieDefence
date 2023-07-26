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
    [SerializeField] private TMP_Text _totalScore;
    [SerializeField] private TMP_Text _forKillEarned;
    [SerializeField] private TMP_Text _levelBonus;
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _settingScoreDelay;

    private void OnEnable()
    {
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        StartCoroutine(SetScore());
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

    private IEnumerator SetScore()
    {
        WaitForSeconds delay = new WaitForSeconds(_settingScoreDelay);
        int forKillScore = _moneyCollecter.Money - _moneyCollecter.StartMoney;
        int totalScore = forKillScore + _levelChoicer.CurrentLevel.LevelBonus;
        yield return delay;
        _forKillEarned.text = forKillScore.ToString();
        yield return delay;
        _levelBonus.text = _levelChoicer.CurrentLevel.LevelBonus.ToString();
        yield return delay;
        _totalScore.text = totalScore.ToString();
    }
}
