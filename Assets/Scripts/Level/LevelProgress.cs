using InfimaGames.LowPolyShooterPack;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private DifficultyChoicer _difficultyChoicer;
    private bool _levelComplited = false;
    private bool _endSequenceStarted;
    private PlayerInput _playerInput;

    public event Action<bool> LevelComplited;
    public event Action LevelFinished;

    private void Awake()
    {
        LevelEnded = false;
        _playerInput = _player.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        bool showTraining = _difficultyChoicer != null
            && _difficultyChoicer.CurrentLevelNumber == 0
            && SaveSystem.Instance != null
            && SaveSystem.Instance.GetData() != null
            && SaveSystem.Instance.GetData().TrainingCompleted == false;

        if (showTraining)
        {
            _educationPanel.gameObject.SetActive(true);
            return;
        }

        NotifyLevelStarted();
    }

    private void OnEnable()
    {
        _endZone.PlayerInLevelEndZone += LevelEnd;
        _player.TargetDied += PlayerLost;
        _track.TargetDied += PlayerLost;
        _zombieSpawner.AllZombieDied += PlayerWin;
        if (_educationPanel != null)
            _educationPanel.Closed += NotifyLevelStarted;
    }

    private void OnDisable()
    {
        _endZone.PlayerInLevelEndZone -= LevelEnd;
        _player.TargetDied -= PlayerLost;
        _track.TargetDied -= PlayerLost;
        _zombieSpawner.AllZombieDied -= PlayerWin;
        if (_educationPanel != null)
            _educationPanel.Closed -= NotifyLevelStarted;
    }

    public void SetCurrentLevel(DifficultyChoicer difficultyChoicer)
    {
        _difficultyChoicer = difficultyChoicer;
    }

    public void NotifyLevelStarted()
    {
        PlatformServices.Lifecycle?.NotifyLevelStarted(
            world: GetWorldName(),
            level: GetLevelName());
        PlatformServices.Banners?.Hide();
    }

    private void LevelEnd()
    {
        BeginEndSequence(completedByExit: true);
    }

    private void PlayerWin()
    {
        if (_difficultyChoicer.SurvivalMode == false)
        {
            _levelComplited = true;
            _endZone.gameObject.SetActive(true);

            LevelComplited?.Invoke(_levelComplited);
        }
    }

    private void PlayerLost()
    {
        BeginEndSequence(completedByExit: false);
        LevelComplited?.Invoke(_levelComplited);
    }

    private void BeginEndSequence(bool completedByExit)
    {
        if (_endSequenceStarted)
            return;

        _endSequenceStarted = true;
        LevelEnded = true;

        if (completedByExit && _levelComplited)
            SaveProgress();

        _mobileInput.SetActive(false);
        if (_playerInput != null)
            _playerInput.enabled = false;

        if (_levelComplited)
        {
            PlatformServices.Lifecycle?.NotifyLevelCompleted(GetWorldName(), GetLevelName());
            TryNotifyStageCompleteAchievement();
        }
        else
        {
            PlatformServices.Lifecycle?.NotifyLevelFailed(GetWorldName(), GetLevelName());
        }

        _endLevelPanel.SetPauseBlocked(true);
        Time.timeScale = 0f;

        if (PlatformServices.Ads != null)
        {
            PlatformServices.Ads.ShowInterstitialBeforeEndgame(ShowEndPanel);
            return;
        }

        ShowEndPanel();
    }

    private void ShowEndPanel()
    {
        _endLevelPanel.gameObject.SetActive(true);
        _endLevelPanel.Initialize(_levelComplited);
        PlatformServices.Banners?.ShowEndgame(_levelComplited);
        LevelFinished?.Invoke();
    }

    private void TryNotifyStageCompleteAchievement()
    {
        if (_difficultyChoicer == null || _difficultyChoicer.SurvivalMode)
            return;

        bool lastLevelOfStage = _difficultyChoicer.CurrentLevelNumber + 1 >= Stage.LevelsPerStage;
        if (lastLevelOfStage)
            PlatformServices.Lifecycle?.NotifyAchievement();
    }

    private void SaveProgress()
    {
        int stageNumber = SaveSystem.Instance.GetData().SelectedStage;
        SaveSystem.Instance.SetProgress(_difficultyChoicer.CurrentLevelNumber + 1, stageNumber);
    }

    private string GetWorldName()
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.GetData() == null)
            return null;

        return SaveSystem.Instance.GetData().SelectedStage.ToString();
    }

    private string GetLevelName()
    {
        return _difficultyChoicer != null ? _difficultyChoicer.CurrentLevelNumber.ToString() : null;
    }
}
