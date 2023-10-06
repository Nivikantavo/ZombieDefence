using InfimaGames.LowPolyShooterPack.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : Element
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private DesertirPanel _desertirPanel;

    private bool _paused = false;

    protected override void Awake()
    {
        base.Awake();
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
        if (_desertirPanel.gameObject.activeSelf == false)
        {
            _paused = !_paused;
            _pausePanel.SetActive(_paused);
        }
    }
}
