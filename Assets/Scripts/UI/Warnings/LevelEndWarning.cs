using UnityEngine;

public class LevelEndWarning : MonoBehaviour
{
    [SerializeField] private LevelProgress _levelProgress;
    [SerializeField] private Warning _warning;

    private void OnEnable()
    {
        _levelProgress.LevelComplited += OnLevelComplited;
        _levelProgress.LevelFinished += OnLevelFinished;
    }

    private void OnDisable()
    {
        _levelProgress.LevelComplited -= OnLevelComplited;
        _levelProgress.LevelFinished -= OnLevelFinished;
    }

    private void OnLevelComplited(bool complited)
    {
        if (complited)
        {
            _warning.ShowPersistent();
        }
        else
        {
            _warning.Hide();
        }
    }

    private void OnLevelFinished()
    {
        _warning.Hide();
    }
}
