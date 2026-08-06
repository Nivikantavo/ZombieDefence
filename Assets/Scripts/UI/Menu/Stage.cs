using UnityEngine;

public class Stage : MonoBehaviour
{
    public const int LevelsPerStage = 12;

    public int LevelsCount => _levelsCount;
    public int CurrentLevelNumber => _currentLevelNumber;
    public int Number => _number;
    public int ComplitedLevels => _complitedLevels;

    [SerializeField] private int _levelsCount = LevelsPerStage;
    [SerializeField] private int _number;
    private int _currentLevelNumber;
    private int _complitedLevels;

    public void SetProgress(int complitedLevels)
    {
        _complitedLevels = complitedLevels;
        if (complitedLevels < _levelsCount)
        {
            _currentLevelNumber = complitedLevels + 1;
        }
        else
        {
            _currentLevelNumber = complitedLevels;
        }
    }
}
