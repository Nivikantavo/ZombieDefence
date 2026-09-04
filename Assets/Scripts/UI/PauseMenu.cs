using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    public bool PauseGame;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            if (PauseGame) 
            {
                Resume();
            } 
            else 
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
        if (PlatformServices.Lifecycle != null && PlatformServices.Lifecycle.IsGameplayActive)
        {
            PlatformServices.Lifecycle.NotifyLevelResumed();
            PlatformServices.Banners?.Hide();
        }
    }

    public void Pause() 
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        PlatformServices.Lifecycle?.NotifyLevelPaused();
        PlatformServices.Banners?.ShowPause();
    }

    public void LoadMainMenu() 
    {
        Time.timeScale = 1f;
        PlatformServices.Lifecycle?.NotifyLoadingStarted();
        SceneManager.LoadScene("MainMenu");
    }
}
