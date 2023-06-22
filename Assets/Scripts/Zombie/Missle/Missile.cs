using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] protected Collider _collider;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Target>(out Target target))
        {
            target.TakeDamage(_damage);
        }
        if (collision.gameObject.TryGetComponent<HitBox>(out HitBox hitBox) == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void Throw()
    {
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        _collider.enabled = false;
    }
}
