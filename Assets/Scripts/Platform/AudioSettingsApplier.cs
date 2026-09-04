using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioSettingsApplier
{
    private const string Music = "Music";
    private const string Sounds = "Sounds";
    private const string MixerResource = "AudioMixer";

    public static void Apply(AudioMixer audioMixer)
    {
        if (audioMixer == null)
            return;

        if (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
            return;

        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return;

        audioMixer.SetFloat(Music, data.MusicVolume);
        audioMixer.SetFloat(Sounds, data.SoundsVolume);
    }

    public static IEnumerator ApplyWhenReady(AudioMixer audioMixer)
    {
        AudioMixer mixer = audioMixer;
        if (mixer == null)
            mixer = Resources.Load<AudioMixer>(MixerResource);

        while (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
            yield return null;

        Apply(mixer);
    }
}
