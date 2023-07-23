using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;

public class PausePanel : Element
{
    [SerializeField] private GameObject _pausePanel;
    private bool _menuIsEnabled;

    protected override void Tick()
    {
        bool cursorLocked = characterBehaviour.IsCursorLocked();
        switch (cursorLocked)
        {
            case true when _menuIsEnabled:
                Hide();
                break;
            case false when !_menuIsEnabled:
                Show();
                break;
        }
    }

    private void Show()
    {
        _menuIsEnabled = true;
        _pausePanel.SetActive(_menuIsEnabled);
        Time.timeScale = 0;
    }

    private void Hide()
    {
        _menuIsEnabled = false;
        Time.timeScale = 1;
        _pausePanel.SetActive(_menuIsEnabled);
    }
}
