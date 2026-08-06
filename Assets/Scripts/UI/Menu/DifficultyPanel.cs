using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPanel : MonoBehaviour
{
    private const int GridColumns = 4;
    private const float CellWidth = 160f;
    private const float CellHeight = 80f;
    private const float CellSpacing = 16f;

    [SerializeField] private List<DifficultyButton> _difficultButtons;
    [SerializeField] private DifficultyButton _buttonTemplate;
    [SerializeField] private Transform _buttonsRoot;
    [SerializeField] private int _levelsCount = Stage.LevelsPerStage;

    public event UnityEngine.Events.UnityAction DifficaltySelected;

    private bool _buttonsBuilt;

    private void Awake()
    {
        EnsureLevelButtons();
    }

    public void Initialize(int levelsAvailable)
    {
        EnsureLevelButtons();

        for (int i = 0; i < _difficultButtons.Count; i++)
        {
            if (i <= levelsAvailable)
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

    private void EnsureLevelButtons()
    {
        if (_buttonsBuilt)
        {
            return;
        }

        if (_difficultButtons == null)
        {
            _difficultButtons = new List<DifficultyButton>();
        }

        if (_buttonTemplate == null && _difficultButtons.Count > 0)
        {
            _buttonTemplate = _difficultButtons[0];
        }

        if (_buttonTemplate == null)
        {
            return;
        }

        Transform gridRoot = EnsureGridRoot();
        List<DifficultyButton> oldButtons = new List<DifficultyButton>(_difficultButtons);

        _buttonTemplate.transform.SetParent(gridRoot, false);
        _buttonTemplate.gameObject.SetActive(true);
        _buttonTemplate.SetLevelNumber(1);
        ConfigureButtonClick(_buttonTemplate, 0);

        _difficultButtons.Clear();
        _difficultButtons.Add(_buttonTemplate);

        for (int i = 0; i < oldButtons.Count; i++)
        {
            DifficultyButton button = oldButtons[i];
            if (button == null || button == _buttonTemplate)
            {
                continue;
            }

            Destroy(button.gameObject);
        }

        for (int levelIndex = 1; levelIndex < _levelsCount; levelIndex++)
        {
            DifficultyButton button = Instantiate(_buttonTemplate, gridRoot);
            button.name = $"LevelButton ({levelIndex + 1})";
            button.SetLevelNumber(levelIndex + 1);
            ConfigureButtonClick(button, levelIndex);
            _difficultButtons.Add(button);
        }

        _buttonsBuilt = true;
    }

    private Transform EnsureGridRoot()
    {
        if (_buttonsRoot != null)
        {
            EnsureGridLayout(_buttonsRoot.gameObject);
            return _buttonsRoot;
        }

        Transform existing = transform.Find("LevelsGrid");
        if (existing != null)
        {
            _buttonsRoot = existing;
            EnsureGridLayout(existing.gameObject);
            return existing;
        }

        GameObject gridObject = new GameObject("LevelsGrid", typeof(RectTransform), typeof(GridLayoutGroup));
        RectTransform rect = gridObject.GetComponent<RectTransform>();
        rect.SetParent(transform, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 40f);
        rect.sizeDelta = new Vector2(720f, 280f);

        EnsureGridLayout(gridObject);
        _buttonsRoot = rect;
        return rect;
    }

    private static void EnsureGridLayout(GameObject gridObject)
    {
        GridLayoutGroup grid = gridObject.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = gridObject.AddComponent<GridLayoutGroup>();
        }

        grid.cellSize = new Vector2(CellWidth, CellHeight);
        grid.spacing = new Vector2(CellSpacing, CellSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = GridColumns;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
    }

    private void ConfigureButtonClick(DifficultyButton button, int levelIndex)
    {
        Button uiButton = button.GetComponent<Button>();
        if (uiButton == null)
        {
            return;
        }

        uiButton.onClick.RemoveAllListeners();
        int capturedIndex = levelIndex;
        uiButton.onClick.AddListener(() => SelectDifficulty(capturedIndex));
    }
}
