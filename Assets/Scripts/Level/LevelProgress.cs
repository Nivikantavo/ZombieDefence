using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    public bool LevelEnded { get; private set; }

    [SerializeField] private Track _track;
    [SerializeField] private Player _player;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private EndLevelPanel _endLevelPanel;
    [SerializeField] private LevelChoicer _levelChoicer;
    [SerializeField] private LevelEndZone _endZone;
    [SerializeField] private Character _charachter;
    [SerializeField] private GameObject _mobileInput;
    [SerializeField] private EducationPanel _educationPanel;

    private bool _levelComplited = false;

    private void Awake()
    {
        LevelEnded = false;
    }

    private void Start()
    {
        if (_levelChoicer.CurrentLevelNumber == 0 && SaveSystem.Instance.GetData().TrainingCompleted == false)
        {
            _educationPanel.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        _endZone.PlayerInLevelEndZone += LevelEnd;
        _player.TargetDied += PlayerLost;
        _track.TargetDied += PlayerLost;
        _zombieSpawner.AllZombieDied += PlayerWin;
    }

    private void OnDisable()
    {
        _endZone.PlayerInLevelEndZone -= LevelEnd;
        _player.TargetDied -= PlayerLost;
        _track.TargetDied -= PlayerLost;
        _zombieSpawner.AllZombieDied -= PlayerWin;
    }

    private void LevelEnd()
    {
        LevelEnded = true;
        if (_levelComplited)
        {
            SaveProgress();
        }
        _mobileInput.SetActive(false);
        _endLevelPanel.gameObject.SetActive(true);
        _endLevelPanel.Initialize(_levelComplited);
    }

    private void PlayerWin()
    {
        if(_levelChoicer.SurvivalMode == false)
        {
            _levelComplited = true;
            _endZone.gameObject.SetActive(true);
        }
    }

    private void PlayerLost()
    {
        _mobileInput.SetActive(false);
        _endLevelPanel.gameObject.SetActive(true);
        _endLevelPanel.Initialize(_levelComplited);
    }

    private void SaveProgress()
    {
        int stageNumber = SceneManager.GetActiveScene().buildIndex;
        SaveSystem.Instance.SetProgress(_levelChoicer.CurrentLevelNumber + 1, stageNumber);
    }
}
