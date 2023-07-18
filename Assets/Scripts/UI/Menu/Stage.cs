using UnityEngine;

public class Stage : MonoBehaviour
{
    public int LevelsCount => _levelsCount;
    public int CurrentLevelNumber => _currentLevelNumber;
    public int Number => _number;
    public int ComplitedLevels => _complitedLevels;

    [SerializeField] private int _levelsCount;
    [SerializeField] private int _number;
    private int _currentLevelNumber;
    private int _complitedLevels;

    public void SetProgress(int complitedLevels)
    {
        _complitedLevels = complitedLevels;
        if(complitedLevels < 3)
        {
            _currentLevelNumber = complitedLevels + 1;
        }
        else
        {
            _currentLevelNumber = complitedLevels;
        }
        
    }
}
