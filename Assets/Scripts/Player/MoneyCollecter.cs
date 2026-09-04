using System.Collections;
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
    private bool _dataUpdatedSubscribed;

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
        while (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        SubscribeToSaveSystem();
        SetData(SaveSystem.Instance.GetData());
    }

    private void OnEnable()
    {
        SubscribeToSaveSystem();
    }

    private void OnDisable()
    {
        UnsubscribeFromSaveSystem();
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
        if (data == null)
            return;

        _money = data.Money;
        _startMoney = _money;
        MoneyLoaded?.Invoke(_money);
    }

    public bool TrySpendMoney(int cost, bool persist = true)
    {
        if(_money < cost)
        {
            return false;
        }
        _money -= cost;
        MoneyCountChanged?.Invoke(_money);
        SaveSystem.Instance.SetMoneyValue(_money, persist);
        return true;
    }

    public void AddMoney(int money)
    {
        _money += money;
        MoneyCountChanged?.Invoke(_money);
        SaveSystem.Instance.SetMoneyValue(_money);
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
        PlayerData data = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;
        if (data == null)
            return;

        SetData(data);
    }

    private void SubscribeToSaveSystem()
    {
        if (_dataUpdatedSubscribed || SaveSystem.Instance == null)
            return;

        SaveSystem.Instance.DataUpdated += OnDataUpdated;
        _dataUpdatedSubscribed = true;
    }

    private void UnsubscribeFromSaveSystem()
    {
        if (_dataUpdatedSubscribed == false || SaveSystem.Instance == null)
            return;

        SaveSystem.Instance.DataUpdated -= OnDataUpdated;
        _dataUpdatedSubscribed = false;
    }
}
