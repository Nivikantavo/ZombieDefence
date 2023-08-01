using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DifficultyPanel : MonoBehaviour
{
    [SerializeField] private List<Button> _levelButtons;

    public event UnityAction DifficaltySelected;

    public void Initialize(int levelsAvailable)
    {
        for (int i = 0; i < _levelButtons.Count; i++)
        {
            _levelButtons[i].interactable = i <= levelsAvailable;
        }
    }

    public void SelectDifficulty(int difficulty)
    {
        SaveSystem.Instance.SetSelectedLevel(difficulty);
        SaveSystem.Instance.SetSurvivalModeEnabled(false);
        DifficaltySelected?.Invoke();
    }

    public void SetSurvivalMode()
    {
        SaveSystem.Instance.SetSurvivalModeEnabled(true);
        DifficaltySelected?.Invoke();
    }
}
