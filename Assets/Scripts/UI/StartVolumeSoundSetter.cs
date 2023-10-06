using UnityEngine;
using UnityEngine.Audio;

public class StartVolumeSoundSetter : MonoBehaviour
{
    private const string Music = "Music";
    private const string Sounds = "Sounds";

    [SerializeField] private AudioMixer _audioMixer;

    private void OnEnable()
    {
        if (SaveSystem.Instance.DataLoaded)
        {
            PlayerData data = SaveSystem.Instance.GetData();

            _audioMixer.SetFloat(Music, data.MusicVolume);
            _audioMixer.SetFloat(Sounds, data.SoundsVolume);
        }
    }
}
