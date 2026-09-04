using UnityEngine;
using UnityEngine.Audio;

public class StartVolumeSoundSetter : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    private void OnEnable()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.DataLoaded)
            AudioSettingsApplier.Apply(_audioMixer);

        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DataUpdated += OnDataUpdated;
    }

    private void OnDisable()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DataUpdated -= OnDataUpdated;
    }

    private void OnDataUpdated()
    {
        AudioSettingsApplier.Apply(_audioMixer);
    }
}
