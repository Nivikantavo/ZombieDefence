using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoneyCollecter : MonoBehaviour
{
    private int _coinsCount = 0;
    private CapsuleCollider _collectionCollider;

    public event UnityAction<Coin> CoinCollected;

    private void Awake()
    {
        _collectionCollider = GetComponent<CapsuleCollider>();
        _collectionCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Coin>(out Coin coin))
        {
            CollectCoin(coin);
        }
    }

    private void CollectCoin(Coin coin)
    {
        _coinsCount += coin.Count;
        coin.Collect();
        CoinCollected?.Invoke(coin);
        coin.gameObject.SetActive(false);
    }
}
