using Agava.WebUtility;
using UnityEngine;
using UnityEngine.EventSystems;

public class InBackgroundCheker : MonoBehaviour
{
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
        AudioListener.pause = inBackground;
        AudioListener.volume = inBackground ? 0f : 1f;
        EventSystem.current.UpdateModules();
    }
}
