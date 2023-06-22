using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePool : ObjectPool
{
    public GameObject Container => _conteiter;

    [SerializeField] private GameObject _missileTemplate;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private int _missilsPerZombie;

    private int _capacity = 0;

    private void Start()
    {
        _capacity = _zombieSpawner.RangeZombieCount * _missilsPerZombie;
        Initialize(_missileTemplate, _capacity);
    }

}
