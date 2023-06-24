using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Track _track;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private GameObject _endLevelPanel;

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

    private void LevelEnd()
    {
        _endLevelPanel.SetActive(true);
        _player.GetComponent<PlayerInput>().enabled = false;
    }
}
