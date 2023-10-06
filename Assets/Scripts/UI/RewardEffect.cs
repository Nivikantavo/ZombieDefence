using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RewardEffect : ObjectPool
{
    public event UnityAction CoinDelivered;

    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private EndLevelPanel _endLevelPanel;
    [SerializeField] private Transform _moneyView;
    [SerializeField] private float _moveTimeDelay;
    [SerializeField] private int _burstVolume;

    private int _spawnSpace = 150;

    private void Start()
    {
        Initialize(_spawnPrefab, _burstVolume);
    }

    private void OnEnable()
    {
        _endLevelPanel.RewardAdClose += OnRewardReceived;
    }

    private void OnDisable()
    {
        _endLevelPanel.RewardAdClose -= OnRewardReceived;
    }

    private void OnRewardReceived()
    {
        SpawnCoin(_burstVolume);
    }

    private void SpawnCoin(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-_spawnSpace, _spawnSpace), Random.Range(-_spawnSpace, _spawnSpace));
            if (TryGetObject(out GameObject coin))
            {
                coin.transform.position = transform.position + offset;
                coin.SetActive(true);
                StartCoroutine(MoveCoin(coin, _moneyView.position));
            }
        }
    }

    private IEnumerator MoveCoin(GameObject coin, Vector3 position)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(_moveTimeDelay);
        float step = 0.01f;
        for (float i = 0; i <= 1; i += step)
        {
            coin.transform.position = Vector3.Lerp(coin.transform.position, position, i);
            yield return delay;
        }
        
        coin.SetActive(false);
        CoinDelivered?.Invoke();
    }
}
