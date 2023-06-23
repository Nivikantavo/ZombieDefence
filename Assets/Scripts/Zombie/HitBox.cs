using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private float _damageRatio;

    protected Idamageable HitTarget;

    protected virtual void Awake()
    {
        HitTarget = GetComponentInParent<Idamageable>();
    }

    public void OnHit(float damage)
    {
        HitTarget.TakeDamage(damage * _damageRatio);
    }
}
