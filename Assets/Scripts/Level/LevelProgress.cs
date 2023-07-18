using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private Player _player;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private GameObject _endLevelPanel;
    [SerializeField] private LevelEndZone _endZone;

    private void OnEnable()
    {
        _player.TargetDied += LevelEnd;
        _track.TargetDied += LevelEnd;
        _zombieSpawner.AllZombieDied += LevelEnd;
    }

    private void OnDisable()
    {
        _player.TargetDied -= LevelEnd;
        _track.TargetDied -= LevelEnd;
        _zombieSpawner.AllZombieDied -= LevelEnd;
    }

    private void LevelEnd(bool levelComplited)
    {
        _endZone.gameObject.SetActive(true);
        if (levelComplited)
        {
            SaveProgress();
        }
    }

    private void SaveProgress()
    {

    }
}
