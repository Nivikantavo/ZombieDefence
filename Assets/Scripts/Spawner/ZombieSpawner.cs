using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZombieSpawner : MonoBehaviour
{
    public int ZombieCount { get; private set; }
    public int DeadZombieCount => _deadZombie; 
    public int RangeZombieCount { get; private set; }

    [SerializeField] private Player _player;
    [SerializeField] private Track _track;
    [SerializeField] private CoinsPool _coinsPool;
    [SerializeField] private MissilePool _missilePool;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<Transform> _startSpawnPoints;

    private List<Wave> _waves;
    private List<DieState> _zombiesDieStates = new List<DieState>();
    private Wave _currentWave;
    private int _currentWaveNumber = 0;
    private int _deadZombie = 0;
    private int _defaultRangeZombieCount = 30;
    private bool _survivalMode = false;
    private bool _startWaveSpawned = false;

    public event UnityAction AllZombieDied;
    public event UnityAction ZombyCounted;

    private void Start()
    {
        _survivalMode = SaveSystem.Instance.GetData().SurvivalMode;
        if (_survivalMode)
        {
            RangeZombieCount = _defaultRangeZombieCount;
        }
        else
        {
            foreach (var wave in _waves)
            {
                ZombieCount += wave.ZombieCount;
                if (wave.EnemyTemplate.TryGetComponent<RangeSeekState>(out RangeSeekState rangeSeekState))
                {
                    RangeZombieCount += wave.ZombieCount;
                }
            }
        }
        ZombyCounted?.Invoke();

        if (_survivalMode)
        {
            return;
        }
        SetCurrentWave(_currentWaveNumber);
        StartCoroutine(SpawnWaves());
    }

    private void OnDisable()
    {
        foreach (var dieState in _zombiesDieStates)
        {
            dieState.NeedSpawnCoin -= OnNeedSpawnCoin;
        }
    }

    public void SetLevelWaves(List<Wave> waves)
    {
        _waves = waves;
    }

    public void SetStartWave(Wave wave)
    {
        _waves.Insert(0, wave);
    } 

    public void SetSpawnPoints(List<Transform> spawnPoints, List<Transform> startSpawnPoints)
    {
        _spawnPoints = spawnPoints;
        _startSpawnPoints = startSpawnPoints;
    }

    public void StartSpawnWave(Wave wave)
    {
        StartCoroutine(SpawnWave(wave));
    }

    private void SetCurrentWave(int waveNumber)
    {
        if(waveNumber < _waves.Count)
           _currentWave = _waves[waveNumber];
    }

    private IEnumerator SpawnWaves()
    {
        WaitForSeconds waveDelay = new WaitForSeconds(_currentWave.DelayAfterWave);
        WaitForSeconds spawnDelay;

        for (int i = 0; i < _waves.Count; i++)
        {
            List<Transform> spawnPoints = i == 0 ? _startSpawnPoints : _spawnPoints;

            spawnDelay = new WaitForSeconds(_currentWave.DelayBetweenSpawn);
            for (int j = 0; j < _currentWave.ZombieCount; j++)
            {
                SpawnZombie(spawnPoints);
                yield return spawnDelay;
            }
            yield return waveDelay;
            NextWave();
        }
    }

    private IEnumerator SpawnWave(Wave wave, List<Transform> spawnPoints = null)
    {
        if(spawnPoints == null)
        {
            spawnPoints = _spawnPoints;
        }

        _currentWave = wave;
        WaitForSeconds spawnDelay = new WaitForSeconds(wave.DelayBetweenSpawn);
        for (int j = 0; j < wave.ZombieCount; j++)
        {
            SpawnZombie(spawnPoints);
            yield return spawnDelay;
        }
    }

    private void SpawnZombie(List<Transform> spawnPoints)
    {
        int spawnPointNumber = Random.Range(0, spawnPoints.Count);
        if (_currentWave.TryGetObject(out GameObject enemy))
        {
            enemy.transform.position = spawnPoints[spawnPointNumber].position;
            enemy.GetComponent<TargetSwitcher>().Initialize(_player, _track);
            enemy.GetComponent<Zombie>().Initialize();
            if (enemy.TryGetComponent<RangeSeekState>(out RangeSeekState rangeSeekState))
            {
                rangeSeekState.SetMissilePool(_missilePool);
            }
            enemy.gameObject.SetActive(true);
            AddInList(enemy);
        }
    }

    private void AddInList(GameObject zombie)
    {
        DieState dieState = zombie.GetComponent<DieState>();
        dieState.NeedSpawnCoin += OnNeedSpawnCoin;
        dieState.ZombieDied += OnZombieDied;
        _zombiesDieStates.Add(dieState);
    }

    private void NextWave()
    {
        if(_currentWaveNumber < _waves.Count)
        {
            _currentWaveNumber++;
            SetCurrentWave(_currentWaveNumber);
        }
    }

    private void OnNeedSpawnCoin(Vector3 spawnPosition)
    {
        _coinsPool.SpawnCoin(spawnPosition);
    }

    private void OnZombieDied()
    {
        _deadZombie++;
        if (_survivalMode == false)
        {
            if (_deadZombie >= ZombieCount)
            {
                AllZombieDied?.Invoke();
            }
        }
    }
}
