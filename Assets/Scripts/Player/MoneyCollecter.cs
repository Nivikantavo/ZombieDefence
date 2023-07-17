using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoneyCollecter : MonoBehaviour, ILoadable
{
    public int CoinsCount => _coinsCount;

    private int _coinsCount = 0;
    private CapsuleCollider _collectionCollider;

    public event UnityAction<Coin> CoinCollected;
    public event UnityAction<int> CoinsCountChanged;

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

    public void SetData(PlayerData data)
    {
        _coinsCount = data.Money;
        CoinsCountChanged?.Invoke(_coinsCount);
    }

    private void CollectCoin(Coin coin)
    {
        _coinsCount += coin.Count;
        coin.Collect();
        CoinCollected?.Invoke(coin);
        CoinsCountChanged?.Invoke(_coinsCount);
        coin.gameObject.SetActive(false);
    }
}
