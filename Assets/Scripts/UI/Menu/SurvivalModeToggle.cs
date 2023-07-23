using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalModeToggle : MonoBehaviour
{
    [SerializeField] private Image _toggleImage;

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
        _survivelModeEnabled = SaveSystem.Instance.GetData().SurvivalMode;
        _toggleImage.enabled = _survivelModeEnabled;
    }

    private void OnEnable()
    {
        _surviveModeButton.onClick.AddListener(OnToggleSurvivalModeClick);
    }

    private void OnDisable()
    {
        _surviveModeButton.onClick.RemoveListener(OnToggleSurvivalModeClick);
    }

    private void OnToggleSurvivalModeClick()
    {
        _survivelModeEnabled = !_survivelModeEnabled;
        _toggleImage.enabled = _survivelModeEnabled;
        SaveSystem.Instance.SetSurvivalModeEnabled(_survivelModeEnabled);
    }
}
