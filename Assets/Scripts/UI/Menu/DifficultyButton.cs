using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _difficultyText;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Button _button;

    public void SetLevelNumber(int levelNumber)
    {
        if (_difficultyText != null)
        {
            // Disable Lean Localization (or similar) so numbered labels are not overwritten.
            MonoBehaviour[] behaviours = _difficultyText.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour is TMPro.TMP_Text)
                {
                    continue;
                }

                string typeName = behaviour.GetType().Name;
                if (typeName.Contains("Localiz") || typeName.Contains("Lean"))
                {
                    behaviour.enabled = false;
                }
            }

            _difficultyText.text = levelNumber.ToString();
        }
    }

    public void Lock()
    {
        if (_difficultyText != null)
        {
            _difficultyText.gameObject.SetActive(false);
        }

        if (_lockImage != null)
        {
            _lockImage.gameObject.SetActive(true);
        }

        if (_button != null)
        {
            _button.interactable = false;
        }
    }

    public void Unlock()
    {
        if (_difficultyText != null)
        {
            _difficultyText.gameObject.SetActive(true);
        }

        if (_lockImage != null)
        {
            _lockImage.gameObject.SetActive(false);
        }

        if (_button != null)
        {
            _button.interactable = true;
        }
    }
}
