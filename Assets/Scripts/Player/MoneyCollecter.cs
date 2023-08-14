using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class MoneyCollecter : MonoBehaviour, ILoadable
{
    public int Money => _money;
    public int StartMoney => _startMoney;

    private int _money = 0;
    private int _startMoney = 0;
    private CapsuleCollider _collectionCollider;

    public event UnityAction<Coin> CoinCollected;
    public event UnityAction<int> MoneyCountChanged;
    public event UnityAction<int> MoneyLoaded;

    private void Awake()
    {
        _collectionCollider = GetComponent<CapsuleCollider>();
        _collectionCollider.isTrigger = true;
    }

    private IEnumerator Start()
    {
        while (SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        SetData(SaveSystem.Instance.GetData());
    }

    private void OnEnable()
    {
        SaveSystem.Instance.DataUpdated += OnDataUpdated;
    }

    private void OnDisable()
    {
        SaveSystem.Instance.DataUpdated -= OnDataUpdated;
        SaveSystem.Instance.SetMoneyValue(_money);
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
        _money = data.Money;
        _startMoney = _money;
        MoneyLoaded?.Invoke(_money);
    }

    public bool TrySpendMoney(int cost)
    {
        if(_money < cost)
        {
            return false;
        }
        _money -= cost;
        MoneyCountChanged?.Invoke(_money);
        return true;
    }

    public void AddMoney(int money)
    {
        _money += money;
        MoneyCountChanged?.Invoke(_money);
    }

    private void CollectCoin(Coin coin)
    {
        AddMoney(coin.Count);
        coin.Sleep();
        CoinCollected?.Invoke(coin);
        coin.gameObject.SetActive(false);
    }

    private void OnDataUpdated()
    {
        SetData(SaveSystem.Instance.GetData());
    }
}
