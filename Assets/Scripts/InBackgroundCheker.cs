using Agava.WebUtility;
using UnityEngine;
using UnityEngine.EventSystems;

public class InBackgroundCheker : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;
    [SerializeField] private UIInput _ui;

    private bool _adsBeingShown = false;


    private void OnEnable()
    {
        WebApplication.InBackgroundChangeEvent += OnInBackgroundChange;
    }

    private void OnDisable()
    {
        WebApplication.InBackgroundChangeEvent -= OnInBackgroundChange;
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
        
        if (Device.IsMobile)
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
