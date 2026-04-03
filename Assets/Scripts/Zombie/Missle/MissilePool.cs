using UnityEngine;

public class MissilePool : ObjectPool
{
    public GameObject Container => _conteiter;

    [SerializeField] private GameObject _missileTemplate;
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private int _missilsPerZombie;

    private int _capacity = 0;

    private void OnEnable()
    {
        _zombieSpawner.ZombiesCounted += Init;
    }

    private void OnDisable()
    {
        _zombieSpawner.ZombiesCounted -= Init;
    }

    private void Init()
    {
        _capacity = _zombieSpawner.RangeZombieCount;
        Initialize(_missileTemplate, _capacity);
    }
}
