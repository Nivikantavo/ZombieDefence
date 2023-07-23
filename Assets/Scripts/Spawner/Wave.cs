using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave : ObjectPool
{
    public int ZombieCount;
    public float DelayBetweenSpawn;
    public float DelayAfterWave;
    public GameObject EnemyTemplate;

    private void Awake()
    {
        Initialize(EnemyTemplate, ZombieCount);
    }

    public void SetCount(int count)
    {
        ZombieCount = count;
    }
}
