using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelChoicer : MonoBehaviour
{
    public int CurrentLevelNumber => _currentLevelNumber;
    private int _currentLevelNumber;

    private List<LevelWaves> _levels;

    private void Awake()
    {
        _currentLevelNumber = SaveSystem.Instance.GetData().ComplitedLevelsOnStage;
        _levels = transform.GetComponentsInChildren<LevelWaves>(true).ToList();

        for (int i = 0; i < _levels.Count; i++)
        {
            if(_currentLevelNumber == i)
            {
                Debug.Log(i);
                _levels[i].gameObject.SetActive(true);
            }
        }
    }
}
