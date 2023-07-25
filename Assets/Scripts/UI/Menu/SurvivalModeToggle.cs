using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalModeToggle : MonoBehaviour
{
    [SerializeField] private Image _toggleImage;
    [SerializeField] private LevelsPanel _levelsPanel;

    private Button _surviveModeButton;
    private bool _survivelModeEnabled = false;

    private void Awake()
    {
        _surviveModeButton = GetComponent<Button>();
    }

    private IEnumerator Start()
    {
        while (SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        SwitchEnabled(SaveSystem.Instance.GetData().SurvivalMode);
    }

    private void OnEnable()
    {
        _surviveModeButton.onClick.AddListener(OnToggleSurvivalModeClick);
        _levelsPanel.StageSelected += OnStageSelected;
    }

    private void OnDisable()
    {
        _surviveModeButton.onClick.RemoveListener(OnToggleSurvivalModeClick);
        _levelsPanel.StageSelected -= OnStageSelected;
    }

    private void OnStageSelected()
    {
        if(_levelsPanel.SelectedStage.ComplitedLevels < 3)
        {
            _surviveModeButton.interactable = false;
            SwitchEnabled(false);
        }
        else
        {
            _surviveModeButton.interactable = true;
        }
    }

    private void SwitchEnabled(bool enabled)
    {
        _survivelModeEnabled = enabled;
        _toggleImage.enabled = _survivelModeEnabled;
        SaveSystem.Instance.SetSurvivalModeEnabled(_survivelModeEnabled);
    }

    private void OnToggleSurvivalModeClick()
    {
        SwitchEnabled(!_survivelModeEnabled);
    }
}
