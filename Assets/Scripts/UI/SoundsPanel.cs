using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundsPanel : MonoBehaviour
{
    private const string Music = "Music";
    private const string Sounds = "Sounds";

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;

    private float _musicVolume;
    private float _soundVolume;

    private void Awake()
    {
        PlayerData data = SaveSystem.Instance.GetData();
        OnMusicSliderValueChanged(data.MusicVolume);
        OnSoundSliderValueChanged(data.SoundsVolume);
    }

    private void OnEnable()
    {
        _musicSlider.value = _musicVolume;
        _soundSlider.value = _soundVolume;

        _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
    }

    private void OnDisable()
    {
        SaveSoundsParameters();
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
        _soundSlider.onValueChanged.RemoveListener(OnSoundSliderValueChanged);
    }

    private void SaveSoundsParameters()
    {
        SaveSystem.Instance.SetSoundsValue(_musicVolume, _soundVolume);
    }

    private void OnMusicSliderValueChanged(float newVolume)
    {
        _musicVolume = newVolume;
        _audioMixer.SetFloat(Music, _musicVolume);
    }

    private void OnSoundSliderValueChanged(float newVolume)
    {
        _soundVolume = newVolume;
        _audioMixer.SetFloat(Sounds, _soundVolume);
    }
}
