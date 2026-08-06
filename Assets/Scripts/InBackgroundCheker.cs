using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_WEBGL
using Playgama;
#endif

public class InBackgroundCheker : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;
    [SerializeField] private UIInput _ui;

    private bool _adsBeingShown = false;


    private void OnEnable()
    {
#if UNITY_WEBGL
        Bridge.platform.pauseStateChanged += OnInBackgroundChange;
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        if (Bridge.instance != null)
            Bridge.platform.pauseStateChanged -= OnInBackgroundChange;
#endif
    }

    public void SetAdsShown(bool adsShown)
    {
        _adsBeingShown = adsShown;
    }

    private void OnInBackgroundChange(bool inBackground)
    {
        if (_adsBeingShown)
        {
            return;
        }
        
        if (PlaygamaAds.IsMobileDevice())
        {
            if(_mobileUI != null)
            {
                _mobileUI.SetActive(!inBackground);
            }
        }
        if(_ui != null)
        {
            _ui.SetPaused(true);
        }
        EventSystem.current.UpdateModules();
        AudioListener.pause = inBackground;
        AudioListener.volume = inBackground ? 0f : 1f;
    }
}
