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
    [SerializeField] private TMP_Text _totalEarned;
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _settingScoreTime;

    private int _currentScore = 0;

    private void OnEnable()
    {
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        StartCoroutine(SetScoreSmoothly(_moneyCollecter.Money));
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

    private IEnumerator SetScoreSmoothly(int score)
    {
        WaitForSeconds delaye = new WaitForSeconds(_settingScoreTime / score);

        for (int i = 0; i < score; i++)
        {
            _currentScore++;
            _totalEarned.text = _currentScore.ToString();
            yield return delaye;
        }
    }
}
