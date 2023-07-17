using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelChoicer : MonoBehaviour
{
    private int _currentLevelNumber;

    private List<LevelWaves> _levels;

    private void Awake()
    {
        PlayerData playerData = SaveSystem.Instance.GetData();
        _currentLevelNumber = SaveSystem.Instance.GetData().Level;
        Debug.Log($"Stage: {playerData.Stage}, \n Level: {playerData.Level}");
        _levels = transform.GetComponentsInChildren<LevelWaves>(true).ToList();

        for (int i = 0; i < _levels.Count; i++)
        {
            if(_currentLevelNumber - 1 == i)
            {
                _levels[i].gameObject.SetActive(true);
            }
        }
        Debug.Log("LevelChoicer Set level");
    }
}
