using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifficultyPanel : MonoBehaviour
{
    [SerializeField] private List<DifficultyButton> _difficultButtons;

    public event UnityAction DifficaltySelected;

    public void Initialize(int levelsAvailable)
    {
        for (int i = 0; i < _difficultButtons.Count; i++)
        {
            if(i <= levelsAvailable)
            {
                _difficultButtons[i].Unlock();
            }
            else
            {
                _difficultButtons[i].Lock();
            }
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
