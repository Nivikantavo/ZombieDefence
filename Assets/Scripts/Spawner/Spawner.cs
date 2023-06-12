using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Track _track;
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private List<Transform> _spawnPoints;

    private Wave _currentWave;
    private int _currentWaveNumber = 0;

    private void Start()
    {
        SetCurrentWave(_currentWaveNumber);
        StartCoroutine(SpawnWaves());
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
            for (int j = 0; j < _currentWave.Count; j++)
            {
                int spawnPointNumber = Random.Range(0, _spawnPoints.Count);
                Target target;

                _currentWave.TryGetObject(out GameObject enemy);
                enemy.transform.position = _spawnPoints[spawnPointNumber].position;
                target = _track.TryAddAttacker() == true ? _track : _player;
                enemy.GetComponent<Zombie>().Initialize(target);
                enemy.gameObject.SetActive(true);
                yield return spawnDelay;
            }

            yield return waveDelay;
            NextWave();
        }
        
    }

    private void NextWave()
    {
        if(_currentWaveNumber < _waves.Count)
        {
            _currentWaveNumber++;
            SetCurrentWave(_currentWaveNumber);
        }
    }
}
