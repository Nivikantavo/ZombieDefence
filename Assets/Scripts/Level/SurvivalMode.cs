using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalMode : MonoBehaviour
{
    [SerializeField] private List<Wave> _easy;
    [SerializeField] private List<Wave> _medium;
    [SerializeField] private List<Wave> _hard;

    [SerializeField] private ZombieSpawner _spawner;

    [SerializeField] private float _periodIncreasingComplexity;

    private enum Complexity
    {
        easy,
        medium,
        hard
    }

    private float _timer = 0;

    private void Update()
    {
        
    }

    private IEnumerator SpawnEasyMode()
    {
        Wave wave = _easy[Random.Range(0, _easy.Count)];
        WaitForSeconds delay = new WaitForSeconds(wave.DelayAfterWave);
        _spawner.StartSpawnWave(wave);
        yield return delay;
    }
}
