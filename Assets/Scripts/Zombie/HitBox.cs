using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;

    public void OnHit(float damage)
    {
        _zombie.TakeDamage(damage);
    }

    public void SetZombie(Zombie zombie)
    {
        if(zombie != null)
        {
            _zombie = zombie;
        }
    }
}
