using UnityEngine;

public class DamageTextPool : ObjectPool
{
    public static DamageTextPool Instance;
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _damageTextTemplate;
    [SerializeField] private Vector3 _spawnOffset;
    [SerializeField] private int _capacity;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Init()
    {
        Initialize(_damageTextTemplate, _capacity);
    }

    public void SpawnDamageText(Vector3 position, float damage)
    {
        TryGetObject(out GameObject spawned);

        if (spawned == null)
        {
            return;
        }
        spawned.GetComponent<DamageText>().Initialize(damage);
        spawned.transform.position = position + _spawnOffset;
        spawned.transform.forward = spawned.transform.position - _player.transform.position;
        //spawned.transform.LookAt(_player.transform);
        spawned.SetActive(true);
    }
}
