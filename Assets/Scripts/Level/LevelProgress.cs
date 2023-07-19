using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private Player _player;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private GameObject _endLevelPanel;
    [SerializeField] private LevelChoicer _levelChoicer;
    [SerializeField] private LevelEndZone _endZone;

    private bool _levelComplited = false;

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
        if (_levelComplited)
        {
            SaveProgress();
        }
        _endLevelPanel.SetActive(true);
    }

    private void PlayerWin()
    {
        _levelComplited = true;
        _endZone.gameObject.SetActive(true);
    }

    private void PlayerLost()
    {
        _endLevelPanel.SetActive(true);
    }

    private void SaveProgress()
    {
        int stageNumber = SceneManager.GetActiveScene().buildIndex;
        SaveSystem.Instance.SetProgress(_levelChoicer.CurrentLevelNumber + 1, stageNumber);
    }
}
