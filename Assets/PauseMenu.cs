using System.Collections;
using System.Collections.Generic;
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
    }

    public void Pause() 
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }

    public void LoadMainMenu() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
