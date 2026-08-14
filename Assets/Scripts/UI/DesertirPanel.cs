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

    private void OnEnable()
    {
        Time.timeScale = 0;
        characterBehaviour.LockCursor(false);
        _inMenuButton.onClick.AddListener(OnInMenuButtonClick);
        _restartButton.onClick.AddListener(OnRestartLevelButtonClick);
    }

    private void OnDisable()
    {
        _inMenuButton.onClick.RemoveListener(OnInMenuButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartLevelButtonClick);
    }

    private void OnRestartLevelButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlaygamaAds.ShowInterstitial(null, OnRestartAdClose, OnRestartAdError, GameAnalyticsAds.Placement.DesertirRestart);
#else
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
    }

    private void OnInMenuButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlaygamaAds.ShowInterstitial(null, OnMenuAdClose, OnMenuAdError, GameAnalyticsAds.Placement.DesertirMenu);
#else
        LoadMainMenu();
#endif
    }

    private void OnRestartAdClose(bool wasShown = true)
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnRestartAdError(string error)
    {
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMenuAdClose(bool wasShown = true)
    {
        LoadMainMenu();
    }

    private void OnMenuAdError(string error)
    {
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        _loadingScreen.LoadScene(0);
    }
}
