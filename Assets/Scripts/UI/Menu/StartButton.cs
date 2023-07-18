using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    [SerializeField] private LevelsPanel _levelsPanel;

    private Button _startButton;

    private void Awake()
    {
        _startButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(LoadLevel);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(LoadLevel);
    }

    private void LoadLevel()
    {
        int sceneNumber = _levelsPanel.SelectedStage.Number;
        SceneManager.LoadScene(sceneNumber);
    }
}
