using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : Element
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private DesertirPanel _desertirPanel;
    [SerializeField] private EndLevelPanel _endLevelPanel;
    [SerializeField] private EducationPanel _educationPanel;

    private bool _paused;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        SwitchPauseEnabled();
    }

    public void SwitchPauseEnabled()
    {
        if (_paused)
        {
            SetPauseState(false);
            return;
        }

        if (CanOpenPause() == false)
            return;

        SetPauseState(true);
    }

    public void SetPaused(bool paused)
    {
        if (paused && CanOpenPause() == false)
            return;

        SetPauseState(paused);
    }

    public void ForceClosePause()
    {
        SetPauseState(false);
    }

    private bool CanOpenPause()
    {
        if (_desertirPanel != null && _desertirPanel.gameObject.activeSelf)
            return false;

        if (_endLevelPanel != null && (_endLevelPanel.gameObject.activeSelf || _endLevelPanel.BlocksPause))
            return false;

        if (_educationPanel != null && _educationPanel.gameObject.activeSelf)
            return false;

        return true;
    }

    private void SetPauseState(bool paused)
    {
        _paused = paused;
        if (_pausePanel != null)
            _pausePanel.SetActive(paused);
    }
}
