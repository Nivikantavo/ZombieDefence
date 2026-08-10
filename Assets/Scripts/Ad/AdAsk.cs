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
        PlaygamaAds.ShowRewarded(OnVideoAdOpen, OnRewardCallback, OnVideoAdClose, OnErrorCallback);
    }

    private void OnVideoAdOpen()
    {
        AudioListener.pause = true;
        AudioListener.volume = 0f;
        if (_backgroundCheker == null)
        {
            _backgroundCheker = FindObjectOfType<InBackgroundCheker>();
        }

        if (_backgroundCheker != null)
        {
            _backgroundCheker.SetAdsShown(true);
        }
    }

    private void OnRewardCallback()
    {
        _moneyCollecter.AddMoney(_reward);
    }

    private void OnVideoAdClose()
    {
        gameObject.SetActive(false);
        if (_backgroundCheker == null)
        {
            _backgroundCheker = FindObjectOfType<InBackgroundCheker>();
        }

        if (_backgroundCheker != null)
        {
            _backgroundCheker.SetAdsShown(false);
        }

        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnErrorCallback(string error)
    {
        OnVideoAdClose();
        Debug.LogError(error);
    }
}
