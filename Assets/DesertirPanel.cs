using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DesertirPanel : Element
{
    [SerializeField] private Button _inMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private LoadingScreen _loadingScreen;

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
        if(_wasRewarded == false)
        {
            InterstitialAd.Show();
        }
#endif
        _loadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnInMenuButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if(_wasRewarded == false)
        {
            InterstitialAd.Show();
        }
#endif
        _loadingScreen.LoadScene(0);
    }
}
