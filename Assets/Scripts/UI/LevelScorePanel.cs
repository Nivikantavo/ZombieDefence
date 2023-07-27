using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelScorePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _forKillScore;
    [SerializeField] private TMP_Text _levelBonus;
    [SerializeField] private TMP_Text _totalScore;
    [SerializeField] private float _settingScoreDelay;

    public void SetScore(int forKillMoney, int levelBonus)
    {
        StartCoroutine(SetLevelScore(forKillMoney, levelBonus));
    }

    private IEnumerator SetLevelScore(int forKillMoney, int levelBonus)
    {
        WaitForSeconds delay = new WaitForSeconds(_settingScoreDelay);
        int totalScore = forKillMoney + levelBonus;

        yield return delay;
        _forKillScore.text = forKillMoney.ToString();

        yield return delay;
        _levelBonus.text = levelBonus.ToString();

        yield return delay;
        _totalScore.text = totalScore.ToString();
    }
}
