using System;
using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DesertirPanel : Element
{
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private InBackgroundCheker _backgroundCheker;

    private bool _interstitialRequested;

    private void OnEnable()
    {
        Time.timeScale = 0;
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
        SetButtonsInteractable(false);
        RequestBreakInterstitial();
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
    }

    private void RequestBreakInterstitial()
    {
        if (_interstitialRequested)
        {
            SetButtonsInteractable(true);
            return;
        }

        _interstitialRequested = true;
        PlatformServices.Lifecycle?.NotifyLevelFailed();
        if (PlatformServices.Ads != null)
        {
            PlatformServices.Ads.ShowInterstitialBeforeEndgame(OnBreakAdFinished);
            return;
        }

        OnBreakAdFinished();
    }

    private void OnBreakAdFinished()
    {
        SetButtonsInteractable(true);
        PlatformServices.Banners?.ShowEndgame(false);
    }

    private void OnRestartLevelButtonClick()
    {
        RunAfterNavigationAd(
            GameAnalyticsAds.Placement.DesertirRestart,
            () => _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    private void OnInMenuButtonClick()
    {
        RunAfterNavigationAd(GameAnalyticsAds.Placement.DesertirMenu, LoadMainMenu);
    }

    private void RunAfterNavigationAd(string placement, Action onComplete)
    {
        SetButtonsInteractable(false);
        if (PlatformServices.Ads == null)
        {
            onComplete?.Invoke();
            return;
        }

        PlatformServices.Ads.ShowInterstitialOnNavigation(onComplete, placement);
    }

    private void LoadMainMenu()
    {
        _loadingScreen.LoadScene(0);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (_inMenuButton != null)
            _inMenuButton.interactable = interactable;
        if (_restartButton != null)
            _restartButton.interactable = interactable;
    }
}
