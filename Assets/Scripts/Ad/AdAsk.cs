using UnityEngine;
using UnityEngine.UI;

public class AdAsk : MonoBehaviour
{
    [SerializeField] private int _reward;
    [SerializeField] private Button _adStartButton;
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private InBackgroundCheker _backgroundCheker;

    private void OnEnable()
    {
        _adStartButton.interactable = true;
        _adStartButton.onClick.AddListener(ShowVideoAd);
    }

    private void OnDisable()
    {
        _adStartButton.onClick.RemoveListener(ShowVideoAd);
    }

    private void ShowVideoAd()
    {
        _adStartButton.interactable = false;
        PlaygamaAds.ShowRewarded(null, OnRewardCallback, OnVideoAdClose, OnErrorCallback, GameAnalyticsAds.Placement.AdAskMoney);
    }

    private void OnRewardCallback()
    {
        _moneyCollecter.AddMoney(_reward);
    }

    private void OnVideoAdClose()
    {
        gameObject.SetActive(false);
    }

    private void OnErrorCallback(string error)
    {
        OnVideoAdClose();
        Debug.LogError(error);
    }
}
