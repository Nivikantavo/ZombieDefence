using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
#if UNITY_WEBGL
using Playgama;
#endif

public static class PlaygamaAdPause
{
    private const string MusicParameter = "Music";
    private const string SoundsParameter = "Sounds";
    private const string MixerResource = "AudioMixer";

    private static int _depth;
    private static float _timeScaleBeforePause = 1f;
    private static bool _keyboardWasEnabled;
    private static AudioMixer _audioMixer;

    public static void Begin()
    {
        if (_depth == 0)
        {
            InBackgroundCheker backgroundChecker = InBackgroundCheker.Active;
            if (backgroundChecker != null)
                backgroundChecker.SetAdsShown(true);

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;

            if (Keyboard.current != null && Keyboard.current.enabled)
            {
                _keyboardWasEnabled = true;
                InputSystem.DisableDevice(Keyboard.current);
            }
            else
            {
                _keyboardWasEnabled = false;
            }
        }

        _depth++;
    }

    public static void End()
    {
        if (_depth <= 0)
            return;

        _depth--;
        if (_depth > 0)
            return;

        Time.timeScale = _timeScaleBeforePause;

        if (_keyboardWasEnabled && Keyboard.current != null)
            InputSystem.EnableDevice(Keyboard.current);

        InBackgroundCheker backgroundChecker = InBackgroundCheker.Active;
        if (backgroundChecker != null)
            backgroundChecker.SetAdsShown(false);
        else
            ApplyPlatformAudio();

        RestoreMixerFromSave();
    }

    public static void ApplyPlatformAudio()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        bool audioEnabled = Bridge.instance == null || Bridge.platform.isAudioEnabled;
        AudioListener.pause = audioEnabled == false;
#else
        AudioListener.pause = false;
#endif
    }

    public static void RestoreMixerFromSave()
    {
        if (_audioMixer == null)
            _audioMixer = Resources.Load<AudioMixer>(MixerResource);

        if (_audioMixer == null)
            return;

        if (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
            return;

        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return;

        _audioMixer.SetFloat(MusicParameter, data.MusicVolume);
        _audioMixer.SetFloat(SoundsParameter, data.SoundsVolume);
    }
}
