using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelNumber;
    [SerializeField] private Image _levelImage;

    public void Initialize(string levelNumber, Sprite levelImage)
    {
        _levelNumber.text = levelNumber;
        _levelImage.sprite = levelImage;
    }
}
