using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave : ObjectPool
{
    public int Count;
    public float DelayBetweenSpawn;
    public float DelayAfterWave;
    public GameObject EnemyTemplate;

    private void Awake()
    {
        Initialize(EnemyTemplate, Count);
    }
}
