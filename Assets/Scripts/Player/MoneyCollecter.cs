using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoneyCollecter : MonoBehaviour, ILoadable
{
    public int Money => _money;

    private int _money = 0;
    private CapsuleCollider _collectionCollider;

    public event UnityAction<Coin> CoinCollected;
    public event UnityAction<int> CoinsCountChanged;

    private void Awake()
    {
        _collectionCollider = GetComponent<CapsuleCollider>();
        _collectionCollider.isTrigger = true;
    }

    private void Start()
    {
        SetData(SaveSystem.Instance.GetData());
    }

    private void OnDisable()
    {
        SaveSystem.Instance.SetMoneyValue(_money);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Coin>(out Coin coin))
        {
            CollectCoin(coin);
        }
    }

    public bool TrySpendMoney(int cost)
    {
        if(_money < cost)
        {
            return false;
        }
        _money -= cost;
        return true;
    }

    private void CollectCoin(Coin coin)
    {
        _money += coin.Count;
        coin.Collect();
        CoinCollected?.Invoke(coin);
        CoinsCountChanged?.Invoke(_money);
        coin.gameObject.SetActive(false);
    }

    public void SetData(PlayerData data)
    {
        Debug.Log(data);
        _money = data.Money;
        CoinsCountChanged?.Invoke(_money);
    }
}
