using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;
    [SerializeField] private float _damageRatio;

    public void OnHit(float damage)
    {
        _zombie.TakeDamage(damage * _damageRatio);
    }
}
