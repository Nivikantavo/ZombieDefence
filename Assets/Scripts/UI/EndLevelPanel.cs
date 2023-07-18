using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelPanel : MonoBehaviour
{
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private TMP_Text _totalEarned;
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _settingScoreTime;

    private int _currentScore = 0;

    private void OnEnable()
    {
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        SetScoreSmoothly(_moneyCollecter.Money);
    }

    private void OnRestartLevelButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnInMenuButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator SetScoreSmoothly(int score)
    {
        WaitForSeconds delaye = new WaitForSeconds(_settingScoreTime);

        for (int i = 0; i < score; i++)
        {
            _currentScore++;
            _totalEarned.text = _currentScore.ToString();
            yield return delaye;
        }
    }
}
