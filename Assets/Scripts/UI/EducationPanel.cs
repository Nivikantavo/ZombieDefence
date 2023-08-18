using InfimaGames.LowPolyShooterPack.Interface;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EducationPanel : Element
{
    [SerializeField] private List<GameObject> _tips;
    [SerializeField] private Button _nextTipButton;
    [SerializeField] private Button _previousTipButton;
    [SerializeField] private Button _closeButton;

    private int _currentTipindex = -1;

    private void Start()
    {
        ShowNextTip();
    }

    private void OnEnable()
    {
        characterBehaviour.LockCursor(false);
        Time.timeScale = 0.0f;

        _nextTipButton.onClick.AddListener(ShowNextTip);
        _previousTipButton.onClick.AddListener(ShowPreviousTip);
        _closeButton.onClick.AddListener(CloseEducationPanel);
    }

    private void OnDisable()
    {
        characterBehaviour.LockCursor(true);
        Time.timeScale = 1.0f;

        _nextTipButton.onClick.RemoveListener(ShowNextTip);
        _previousTipButton.onClick.RemoveListener(ShowPreviousTip);
        _closeButton.onClick.RemoveListener(CloseEducationPanel);
    }

    private void ShowNextTip()
    {
        _currentTipindex++;
        Renderer();

        InitButtons();
    }

    private void ShowPreviousTip()
    {
        _currentTipindex--;
        Renderer();

        InitButtons();
    }

    private void CloseEducationPanel()
    {
        SaveSystem.Instance.SetTrainingCompleted(true);
        gameObject.SetActive(false);
    }

    private void Renderer()
    {
        for (int i = 0; i < _tips.Count; i++)
        {
            if (i != _currentTipindex)
            {
                _tips[i].SetActive(false);
            }
        }
        _tips[_currentTipindex].gameObject.SetActive(true);
    }

    private void InitButtons()
    {
        _previousTipButton.interactable = _currentTipindex != 0;

        bool isLastTip = _currentTipindex == _tips.Count - 1;
        _nextTipButton.gameObject.SetActive(!isLastTip);
        _closeButton.gameObject.SetActive(isLastTip);
    }
}
