using UnityEngine;

public class ZombieHitEffect : HitEffect
{
    [SerializeField] private Zombie _zombie;

    private void OnEnable()
    {
        _zombie.Hit += OnHitEffect;
    }

    private void OnDisable()
    {
        _zombie.Hit -= OnHitEffect;
    }
}
