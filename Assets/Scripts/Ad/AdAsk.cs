using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdAsk : MonoBehaviour
{
    [SerializeField] private int _reward;
    [SerializeField] private Button _adStartButton;
    [SerializeField] private MoneyCollecter _moneyCollecter;

    private void OnEnable()
    {
        _adStartButton.onClick.AddListener(ShowVideoAd);
    }

    private void OnDisable()
    {
        _adStartButton.onClick.RemoveListener(ShowVideoAd);
    }

    private void ShowVideoAd()
    {
        VideoAd.Show(OnVideoAdOpen, OnRewardCallback);
    }

    private void OnVideoAdOpen()
    {
        gameObject.SetActive(false);
    }

    private void OnRewardCallback()
    {
        _moneyCollecter.AddMoney(_reward);
    }
}
