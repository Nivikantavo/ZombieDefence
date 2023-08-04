using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class CoinMagnetizer : MonoBehaviour
{
    [SerializeField] private float _magnetizationRadius;
    [SerializeField] private float _magnetizationForce;
    [SerializeField] private MoneyCollecter _coinCollecter;

    private CapsuleCollider _magnetizationCollider;
    private List<Rigidbody> _coins = new List<Rigidbody>();

    private void Awake()
    {
        _magnetizationCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _magnetizationCollider.radius = _magnetizationRadius;
    }

    private void OnEnable()
    {
        _coinCollecter.CoinCollected += OnCoinCollected;
    }

    private void OnDisable()
    {
        _coinCollecter.CoinCollected -= OnCoinCollected;
    }

    private void Update()
    {
        if(_coins.Count > 0)
        {
            foreach (var coin in _coins)
            {
                coin.AddForce((transform.position - coin.transform.position).normalized * _magnetizationForce);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Coin>(out Coin coin))
        {
            Rigidbody coinRigidBody = coin.GetComponent<Rigidbody>();
            if(_coins.Contains(coinRigidBody) == false)
            {
                _coins.Add(coinRigidBody);
                coin.WakeUp();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Coin>(out Coin coin))
        {
            Rigidbody coinRigidBody = coin.GetComponent<Rigidbody>();
            if (_coins.Contains(coinRigidBody) == true)
            {
                coin.Sleep();
                _coins.Remove(coinRigidBody);
            }
        }
    }

    private void OnCoinCollected(Coin coin)
    {
        Rigidbody coinRigidBody = coin.GetComponent<Rigidbody>();
        if (_coins.Contains(coinRigidBody) == true)
        {
            coin.Sleep();
            _coins.Remove(coinRigidBody);
        }
    }
}
