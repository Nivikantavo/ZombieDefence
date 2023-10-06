using UnityEngine;
using UnityEngine.Events;

public class ArmorElement : MonoBehaviour, Idamageable
{
    [SerializeField] private float _maxHealth;

    private float _currentHealth;

    public event UnityAction Hit;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        Hit?.Invoke();
        if (_currentHealth < 0)
        {
            BreakDown();
        }
    }

    private void BreakDown()
    {
        gameObject.SetActive(false);
    }
}
