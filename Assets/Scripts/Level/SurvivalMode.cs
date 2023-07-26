using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalMode : MonoBehaviour
{
    [SerializeField] private GameObject _surviveTimer;

    [SerializeField] private List<Wave> _easy;
    [SerializeField] private List<Wave> _medium;
    [SerializeField] private List<Wave> _hard;

    [SerializeField] private ZombieSpawner _spawner;

    [SerializeField] private int _easyWavesCount;
    [SerializeField] private int _mediumWavesCount;
    [SerializeField] private int _minimumEnemiesInWave;
    [SerializeField] private int _maximumEnemiesInWave;

    private int _magnificationEnemiesLimit;

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
        _magnificationEnemiesLimit = _easy.Count;
        UpdateComplexity();
        _timeAfterLastWave = _delayBetweenWaves;
    }

    private void OnEnable()
    {
        _surviveTimer.SetActive(true);
    }

    private void Update()
    {
        _timeAfterLastWave += Time.deltaTime;
        if(_timeAfterLastWave >= _delayBetweenWaves)
        {
            
            if (_currentComplexity == Complexity.easy)
            {
                SpawnWave(_easy);
            }
            else if (_currentComplexity == Complexity.medium)
            {
                SpawnWave(_easy);
                SpawnWave(_medium);
            }
            else
            {
                SpawnWave(_easy);
                SpawnWave(_medium);
                SpawnWave(_hard);
            }
            _timeAfterLastWave = 0;

            if(_maximumEnemiesInWave < _magnificationEnemiesLimit)
            {
                _maximumEnemiesInWave++;
            }
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
        _spawnedWavesCount++;
    }
}
