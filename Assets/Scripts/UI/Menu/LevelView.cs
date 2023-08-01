using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private Image _levelImage;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Button _selectButton;

    private Stage _stage;
    private bool _locked;
    public void Initialize(Stage stage, bool locked)
    {
        _stage = stage;

        (locked ? new Action(Lock) : Unlock)();
    }

    public void Lock()
    {
        _locked = true;
        _lockImage.enabled = _locked;
        _levelImage.color = _selectButton.colors.disabledColor;
        _selectButton.interactable = false;
    }

    public void Unlock()
    {
        _locked = false;
        _lockImage.enabled = _locked;
        _levelImage.color = _selectButton.colors.normalColor;
        _selectButton.interactable = !_locked;
    }
}
