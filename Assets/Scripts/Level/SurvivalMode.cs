using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalMode : MonoBehaviour
{
    [SerializeField] private List<Wave> _easy;
    [SerializeField] private List<Wave> _medium;
    [SerializeField] private List<Wave> _hard;

    [SerializeField] private ZombieSpawner _spawner;

    [SerializeField] private int _easyWavesCount;
    [SerializeField] private int _mediumWavesCount;
    [SerializeField] private int _minimumEnemiesInWave;
    [SerializeField] private int _maximumEnemiesInWave;

    private int _spawnedWavesCount = 0;
    private float _timeAfterLastWave = 0;
    private float _delayBetweenWaves = 0;
    private Complexity _currentComplexity;

    private enum Complexity
    {
        easy,
        medium,
        hard
    }

    private void Awake()
    {
        UpdateComplexity();
        _timeAfterLastWave = _delayBetweenWaves;
    }

    private void Update()
    {
        _timeAfterLastWave += Time.deltaTime;
        if(_timeAfterLastWave >= _delayBetweenWaves)
        {
            
            if (_currentComplexity == Complexity.easy)
            {
                Debug.Log("Spawn Wave easy");
                SpawnWave(_easy);
            }
            else if (_currentComplexity == Complexity.medium)
            {
                Debug.Log("Spawn Wave medium");
                SpawnWave(_easy);
                SpawnWave(_medium);
            }
            else
            {
                Debug.Log("Spawn Wave hard");
                SpawnWave(_easy);
                SpawnWave(_medium);
                SpawnWave(_hard);
            }
            _timeAfterLastWave = 0;
        }

        UpdateComplexity();
    }

    private void UpdateComplexity()
    {
        if(_spawnedWavesCount >= _easyWavesCount)
        {
            if(_spawnedWavesCount >= _mediumWavesCount)
            {
                _currentComplexity = Complexity.hard;
            }
            else
            {
                _currentComplexity = Complexity.medium;
            }
        }
        else
        {
            _currentComplexity = Complexity.easy;
        }
    }

    private void SpawnWave(List<Wave> wavesType)
    {
        int enemiesCount = Random.Range(_minimumEnemiesInWave, _maximumEnemiesInWave);
        Wave wave = wavesType[Random.Range(0, wavesType.Count)];
        wave.SetCount(enemiesCount);
        _delayBetweenWaves = enemiesCount * wave.DelayBetweenSpawn + wave.DelayAfterWave;
        _spawner.StartSpawnWave(wave);
    }
}
