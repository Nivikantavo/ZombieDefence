using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Track : Target
{
    public float MaxFuel => _maxFuel;
    public float CurrentFuel => _currentFuel;

    [SerializeField] private Collider _collider;
    [SerializeField] private float _maxFuel;
    [SerializeField] private float _fillingTime;
    [SerializeField] private float _fillingStep;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioSource _audioSource;

    private float _currentFuel;

    public event UnityAction<float> FuelUpdate;

    protected override void Awake()
    {
        SetMaxHealth();
        base.Awake();
        _currentFuel = 0;
    }

    private void Start()
    {
        StartCoroutine(FillUpTank());
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PlayHitSound();
    }

    public override Vector3 GetClosesetPositin(Vector3 position)
    {
        return _collider.ClosestPointOnBounds(position);
    }

    private IEnumerator FillUpTank()
    {
        WaitForSeconds fillDelay = new WaitForSeconds(_fillingTime / (_maxFuel / _fillingStep));
        while(_currentFuel < _maxFuel)
        {
            _currentFuel++;
            FuelUpdate?.Invoke(_currentFuel);
            yield return fillDelay;
        }
    }

    private void SetMaxHealth()
    {
        MaxHealthPoints = SaveSystem.Instance.GetData().TruckHealth;
    }

    private void PlayHitSound()
    {
        _audioSource.pitch = Random.Range(1f, 2f);

        _audioSource.PlayOneShot(_hitClip);
    }
}
