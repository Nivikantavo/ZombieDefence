using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelChoicer : MonoBehaviour
{
    public int CurrentLevelNumber => _currentLevelNumber;
    private int _currentLevelNumber;

    private List<LevelWaves> _levels;
    private SurvivalMode _survivalMode;

    private void Awake()
    {
        PlayerData data = SaveSystem.Instance.GetData();

        if(data.SurvivalMode == true)
        {
            _survivalMode = transform.GetComponentInChildren<SurvivalMode>();
            _survivalMode.gameObject.SetActive(true);
        }
        else
        {
            _currentLevelNumber = data.ComplitedLevelsOnStage;
            _levels = transform.GetComponentsInChildren<LevelWaves>(true).ToList();

            for (int i = 0; i < _levels.Count; i++)
            {
                if (_currentLevelNumber == i)
                {
                    _levels[i].gameObject.SetActive(true);
                }
            }
        }
    }
}
