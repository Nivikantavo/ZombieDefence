using InfimaGames.LowPolyShooterPack.Interface;
using Agava.YandexGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
        InterstitialAd.Show(OnAdOpen, OnRestartAdClose, OnRestartAdError);
#endif
    }

    private void OnInMenuButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        InterstitialAd.Show(OnAdOpen, OnAdClose, OnAdError);
#endif
        _loadingScreen.LoadScene(0);
    }

    private void OnAdOpen()
    {
        _backgroundCheker.SetAdsShown(true);
        InputSystem.DisableDevice(Keyboard.current);
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }

    private void OnAdClose(bool wasShown = true)
    {
        _backgroundCheker.SetAdsShown(false);
        InputSystem.EnableDevice(Keyboard.current);
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnRestartAdClose(bool wasShown = true)
    {
        OnAdClose(wasShown);
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void OnRestartAdError(string error)
    {
        OnAdClose();
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnAdError(string error)
    {
        OnAdClose();
        Debug.Log(error);
    }
}
