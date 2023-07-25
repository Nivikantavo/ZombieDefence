using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _playerCanvas;

    private bool _paused = false;
    private bool _cursorLocked = false;

    private void Awake()
    {
        _cursorLocked = true;
        UpdateCursorState();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        switch (context)
        {
            case { phase: InputActionPhase.Performed }:
                SwitchPauseEnabled();
                break;
        }
    }

    public void SwitchPauseEnabled()
    {
        _paused = !_paused;
        _pausePanel.SetActive(_paused);
        _cursorLocked = !_cursorLocked;
        UpdateCursorState();
        Time.timeScale = _paused ? 0.0f : 1.0f;
        //_playerCanvas.SetActive(!_paused);
    }

    private void UpdateCursorState()
    {
        Cursor.visible = !_cursorLocked;
        Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
