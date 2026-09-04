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
        bool supported = PlatformServices.Ads != null
            ? PlatformServices.Ads.IsRewardedSupported
            : PlaygamaAds.IsRewardedSupported || Application.isEditor;
        _adStartButton.gameObject.SetActive(supported);
        _adStartButton.interactable = supported;
        _adStartButton.onClick.AddListener(ShowVideoAd);

        if (supported)
        {
            PlaygamaAds.CheckAdBlock(blocked =>
            {
                if (blocked && _adStartButton != null)
                    _adStartButton.gameObject.SetActive(false);
            });
        }
    }

    private void OnDisable()
    {
        _adStartButton.onClick.RemoveListener(ShowVideoAd);
    }

    private void ShowVideoAd()
    {
        _adStartButton.interactable = false;
        if (PlatformServices.Ads != null)
        {
            PlatformServices.Ads.ShowRewarded(
                OnRewardCallback,
                OnVideoAdClose,
                OnErrorCallback,
                GameAnalyticsAds.Placement.AdAskMoney);
            return;
        }

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
