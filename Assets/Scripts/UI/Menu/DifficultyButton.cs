using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _difficultyText;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Button _button;

    public void Lock()
    {
        _difficultyText.gameObject.SetActive(false);
        _lockImage.gameObject.SetActive(true);
        _button.interactable = false;
    }

    public void Unlock()
    {
        _difficultyText.gameObject.SetActive(true);
        _lockImage.gameObject.SetActive(false);
        _button.interactable = true;
    }
}
