using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    [SerializeField] private Progress _progress;
    [SerializeField] private LevelView _levelView;
    [SerializeField] private Sprite _standartlevelImage;
    [SerializeField] private Sprite _currentLevelImage;

    private void OnEnable()
    {
        _progress.DataLoaded += InstantiateLevelViews;
    }

    private void OnDisable()
    {
        _progress.DataLoaded -= InstantiateLevelViews;
    }

    private void InstantiateLevelViews()
    {
        for (int i = 0; i < _progress.CurrentStage.LevelsCount; i++)
        {
            int levelNumber = i + 1;
            Sprite levelImage = levelNumber != _progress.CurrentStage.CurrentLevelNumber ? _standartlevelImage : _currentLevelImage;

            LevelView levelView = Instantiate(_levelView, transform);
            levelView.Initialize(levelNumber.ToString(), levelImage);
        }
    }
}
