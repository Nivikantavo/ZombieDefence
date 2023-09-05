using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelWaves : MonoBehaviour
{
    public int LevelBonus => _levelBonus;

    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private int _levelBonus;
    [SerializeField] private Wave _startWave;

    private List<Wave> _waves = new List<Wave>();

    private void Awake()
    {
        _waves = GetComponentsInChildren<Wave>().ToList();
        _zombieSpawner.SetStartWave(_startWave);
        _zombieSpawner.SetLevelWaves(_waves);
    }
}
