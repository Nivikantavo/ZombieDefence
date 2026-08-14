using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_WEBGL
using Playgama;
#endif

public class InBackgroundCheker : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;
    [SerializeField] private UIInput _ui;

    private bool _adsBeingShown;
    private bool _platformPaused;
    private bool _audioEnabled = true;

    public static InBackgroundCheker Active { get; private set; }

    private void OnEnable()
    {
        Active = this;
#if UNITY_WEBGL
        if (Bridge.instance != null)
            Subscribe();
        else
            StartCoroutine(WaitForBridgeAndSubscribe());
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        Unsubscribe();
#endif
        if (Active == this)
            Active = null;
    }

    public void SetAdsShown(bool adsShown)
    {
        _adsBeingShown = adsShown;
        if (adsShown == false)
            ApplyPlatformState();
    }

#if UNITY_WEBGL
    private IEnumerator WaitForBridgeAndSubscribe()
    {
        while (Bridge.instance == null)
            yield return null;

        if (isActiveAndEnabled == false)
            yield break;

        Subscribe();
    }

    private void Subscribe()
    {
        Unsubscribe();
        Bridge.platform.pauseStateChanged += OnPauseStateChanged;
        Bridge.platform.audioStateChanged += OnAudioStateChanged;
        _platformPaused = false;
        _audioEnabled = Bridge.platform.isAudioEnabled;
        ApplyPlatformState();
    }

    private void Unsubscribe()
    {
        if (Bridge.instance == null)
            return;

        Bridge.platform.pauseStateChanged -= OnPauseStateChanged;
        Bridge.platform.audioStateChanged -= OnAudioStateChanged;
    }

    private void OnPauseStateChanged(bool paused)
    {
        _platformPaused = paused;
        ApplyPlatformState();
    }

    private void OnAudioStateChanged(bool enabled)
    {
        _audioEnabled = enabled;
        ApplyPlatformState();
    }
#endif

    private void ApplyPlatformState()
    {
        if (_adsBeingShown)
            return;

#if UNITY_WEBGL
        if (Bridge.instance == null)
            return;
#endif

        if (PlaygamaAds.IsMobileDevice() && _mobileUI != null)
            _mobileUI.SetActive(_platformPaused == false);

        if (_platformPaused)
        {
            Time.timeScale = 0f;
            if (_ui != null)
                _ui.SetPaused(true);
        }

        if (EventSystem.current != null)
            EventSystem.current.UpdateModules();

        AudioListener.pause = _platformPaused || _audioEnabled == false;

        if (_platformPaused == false && _audioEnabled)
            PlaygamaAdPause.RestoreMixerFromSave();
    }
}
