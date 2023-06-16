using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class CoinMagnetizer : MonoBehaviour
{
    [SerializeField] private float _magnetizationRadius;
    [SerializeField] private float _magnetizationForce;

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
                coinRigidBody.WakeUp();
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
                coinRigidBody.Sleep();
                _coins.Remove(coinRigidBody);
            }
        }
    }
}
