using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private GameObject _endLevelPanel;
    [SerializeField] private LevelEndZone _endZone;

    private void OnEnable()
    {
        _track.TargetDied += LevelEnd;
        _zombieSpawner.AllZombieDied += LevelEnd;
    }

    private void OnDisable()
    {
        _track.TargetDied -= LevelEnd;
        _zombieSpawner.AllZombieDied -= LevelEnd;
    }

    private void LevelEnd()
    {
        _endZone.gameObject.SetActive(true);
    }
}
