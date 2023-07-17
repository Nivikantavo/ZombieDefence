using UnityEngine;
using UnityEngine.UI;

public class SoundSwitch : MonoBehaviour
{
    [SerializeField] private Button _onOffButton;
    [SerializeField] private Sprite _soundOnIcon;
    [SerializeField] private Sprite _soundOffIcon;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("SoundOn", 1) == 1)
        {
            _onOffButton.image.sprite = _soundOnIcon;
            AudioListener.volume = 1f;
        }
        else
        {
            _onOffButton.image.sprite = _soundOffIcon;
            AudioListener.volume = 0f;
        }
    }
    public void ChangeVolume()
    {
        if (PlayerPrefs.GetInt("SoundOn", 1) == 1)
        {
            _onOffButton.image.sprite = _soundOffIcon;
            AudioListener.volume = 0f;
            PlayerPrefs.SetInt("SoundOn", 0);
        }
        else if (PlayerPrefs.GetInt("SoundOn") == 0)
        {
            _onOffButton.image.sprite = _soundOnIcon;
            AudioListener.volume = 1f;
            PlayerPrefs.SetInt("SoundOn", 1);
        }
    }
}
