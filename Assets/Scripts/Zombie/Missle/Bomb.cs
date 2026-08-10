using UnityEngine;

public class Bomb : MonoBehaviour, Idamageable
{
    [SerializeField] private Transform _explosionPrefab;
    [SerializeField] private float _radius;
    [SerializeField] private float _power;
    [SerializeField] private float _damage;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _zombieDamadeMultiplier;

    private float _currentHealth;

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
                BlowUp();
        }
    }

    public void BlowUp()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit checkGround, 50f))
        {
            GameObject explosion = PrefabPool.Get(_explosionPrefab, checkGround.point,
                Quaternion.FromToRotation(Vector3.forward, checkGround.normal));

            if (explosion != null &&
                explosion.GetComponent<InfimaGames.LowPolyShooterPack.Legacy.ImpactScript>() == null &&
                explosion.GetComponent<PooledExplosionAutoRelease>() == null)
            {
                explosion.AddComponent<PooledExplosionAutoRelease>();
            }
        }

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, _radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider hit = colliders[i];
            if (hit.TryGetComponent(out Idamageable damageable))
            {
                if (damageable is Zombie)
                    damageable.TakeDamage(_damage * _zombieDamadeMultiplier);
                else
                    damageable.TakeDamage(_damage);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(_power * 5f, explosionPos, _radius, 3.0f);
        }

        gameObject.SetActive(false);
    }
}
