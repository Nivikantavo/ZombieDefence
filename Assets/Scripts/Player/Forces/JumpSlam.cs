using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class JumpSlam : Force
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _upForce;
    [SerializeField] private float _improveJumpForce;
    [SerializeField] private float _standartJumpForce;
    [SerializeField] private float _stunDuration;
    [SerializeField] private Movement _playerMovment;

    private CapsuleCollider _explosionCollider;
    private List<Zombie> _zombies = new List<Zombie>();

    private bool _jumpImproved = false;

    protected override void Awake()
    {
        base.Awake();
        _explosionCollider = GetComponent<CapsuleCollider>();
        _explosionCollider.radius = _explosionRadius;
    }

    protected override void Update()
    {
        base.Update();
        if(LastUseTime > Cooldown && _jumpImproved == false)
        {
            SetJumpForce(_improveJumpForce);
            _jumpImproved = true;
        }
    }

    private void OnEnable()
    {
        _playerMovment.JumpEnd += UseForce;
    }

    private void OnDisable()
    {
        _playerMovment.JumpEnd -= UseForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            if (_zombies.Contains(zombie) == false)
            {
                _zombies.Add(zombie);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            if (_zombies.Contains(zombie) == true)
            {
                _zombies.Remove(zombie);
            }
        }
    }

    public override void UseForce()
    {
        StartCoroutine(Slam());
        base.UseForce();
    }

    private IEnumerator Slam()
    {
        if(LastUseTime > Cooldown)
        {
            foreach (var zombie in _zombies)
            {
                Rigidbody rigidbody = zombie.GetComponentInChildren<Rigidbody>();
                RagDoll ragDoll = zombie.GetComponent<RagDoll>();
                zombie.Stun(_stunDuration);

                yield return ragDoll.GetActiveStatus();

                rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upForce, ForceMode.Impulse);
            }
            SetJumpForce(_standartJumpForce);
            _jumpImproved = false;
        }
    }

    private void SetJumpForce(float jumpforce)
    {
        _playerMovment.SetJumpForce(jumpforce);
    }
}
