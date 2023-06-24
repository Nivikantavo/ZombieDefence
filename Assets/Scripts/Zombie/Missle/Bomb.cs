using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, Idamageable
{
    [SerializeField] private Transform _explosionPrefab;
    [SerializeField] private float _radius;
    [SerializeField] private float _power;
    [SerializeField] private float _damage;
    [SerializeField] private float _maxHealth;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(_currentHealth > 0)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                BlowUp();
            }
        }
    }

    public void BlowUp()
    {
        Debug.Log("Explosion");
        RaycastHit checkGround;
        if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
        {
            Instantiate(_explosionPrefab, checkGround.point,
                Quaternion.FromToRotation(Vector3.forward, checkGround.normal));
        }

        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, _radius);
        foreach (Collider hit in colliders)
        {
            if(hit.TryGetComponent<Idamageable>(out Idamageable damageable))
            {
                damageable.TakeDamage(_damage);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(_power * 5, explosionPos, _radius, 3.0F);
        }
        gameObject.SetActive(false);
    }
}
