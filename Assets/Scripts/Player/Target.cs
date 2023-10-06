using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour, Idamageable
{
    public float MaxHealth => MaxHealthPoints;
    public float CurrentHealth => _currentHealth;

    [SerializeField] protected float MaxHealthPoints;

    private float _currentHealth;

    public event UnityAction<float> HealthChanged;
    public event UnityAction HasAttacked;
    public event UnityAction TargetDied;

    protected virtual void Awake()
    {
        _currentHealth = MaxHealthPoints;
    }

    public virtual void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealthPoints);
        HealthChanged?.Invoke(_currentHealth);
        HasAttacked?.Invoke();
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual Vector3 GetClosesetPositin(Vector3 position)
    {
        return transform.position;
    }

    private void Die()
    {
        TargetDied?.Invoke();
    }
}
