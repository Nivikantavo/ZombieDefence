using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelNumber;
    [SerializeField] private Image _levelImage;
    [SerializeField] private List<Image> _stars;
    [SerializeField] private Sprite _standartSprite;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _lockSprite;
    [SerializeField] private Button _selectButton;

    private Stage _stage;
    private bool _selected;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(Select);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(Select);
    }

    public void Initialize(Stage stage)
    {
        _stage = stage;
        _levelNumber.text = _stage.Number.ToString();
        _levelImage.sprite = _selected ? _selectedSprite : _standartSprite;

        for (int i = 0; i < _stage.ComplitedLevels; i++)
        {
            _stars[i].enabled = true;
        }
    }

    public void Select()
    {
        _selected = true;
        _levelImage.sprite = _selectedSprite;
    }

    public void Deselect()
    {
        _selected = false;
        _levelImage.sprite = _standartSprite;
    }

    public void Lock()
    {
        Deselect();
        _levelImage.sprite = _lockSprite;
        _selectButton.interactable = false;
    }

    public void Unlock()
    {
        _levelImage.sprite = _standartSprite;
        _selectButton.interactable = true;
    }
}
