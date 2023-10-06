using UnityEngine;

public class CoinsPool : ObjectPool
{
    [SerializeField] private GameObject _coinTemplate;
    [SerializeField] private ZombieSpawner _spawner;
    [SerializeField] private Vector3 _spawnOffset;

    private int _capacity;

    private void OnEnable()
    {
        _spawner.ZombyCounted += Init;
    }

    private void OnDisable()
    {
        _spawner.ZombyCounted -= Init;
    }

    private void Init()
    {
        _capacity = _spawner.ZombieCount;
        Initialize(_coinTemplate, _capacity);
    }

    public void SpawnCoin(Vector3 spawnPosition)
    {
        TryGetObject(out GameObject spawned);

        if(spawned == null)
        {
            return;
        }

        spawned.transform.position = spawnPosition + _spawnOffset;
        spawned.SetActive(true);
    }
}
