using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZombieSpawner : MonoBehaviour
{
    public int ZombieCount { get; private set; }

    [SerializeField] private Player _player;
    [SerializeField] private Track _track;
    [SerializeField] private CoinsPool _coinsPool;
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private List<Transform> _spawnPoints;

    private List<DieState> _zombiesDieStates = new List<DieState>();
    private Wave _currentWave;
    private int _currentWaveNumber = 0;

    private void Awake()
    {
        foreach (var wave in _waves)
        {
            ZombieCount += wave.ZombieCount;
        }
    }

    private void Start()
    {
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

    private void SetCurrentWave(int waveNumber)
    {
        if(waveNumber < _waves.Count)
           _currentWave = _waves[waveNumber];
    }

    private IEnumerator SpawnWaves()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(_currentWave.DelayBetweenSpawn);
        WaitForSeconds waveDelay = new WaitForSeconds(_currentWave.DelayAfterWave);

        for (int i = 0; i < _waves.Count; i++)
        {
            for (int j = 0; j < _currentWave.ZombieCount; j++)
            {
                SpawnZombie();
                yield return spawnDelay;
            }
            yield return waveDelay;
            NextWave();
        }
    }

    private void SpawnZombie()
    {
        int spawnPointNumber = Random.Range(0, _spawnPoints.Count);
        _currentWave.TryGetObject(out GameObject enemy);
        enemy.transform.position = _spawnPoints[spawnPointNumber].position;
        enemy.GetComponent<TargetSwitcher>().Initialize(_player, _track);
        enemy.gameObject.SetActive(true);
        AddInList(enemy);
    }

    private void AddInList(GameObject zombie)
    {
        DieState dieState = zombie.GetComponent<DieState>();
        dieState.NeedSpawnCoin += OnNeedSpawnCoin;
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
}
