using System.Collections;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Platform;
#endif

public class StartScreen : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        gameObject.SetActive(false);
        return;
#endif
        StartCoroutine(CheckGameReady());
    }

    private IEnumerator CheckGameReady() 
    {
        while (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
        {
            yield return null;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        while (Bridge.instance == null)
            yield return null;

        if (PlatformServices.Instance != null)
            PlatformServices.Instance.NotifyInitialLoadComplete();

        Bridge.platform.SendMessage(PlatformMessage.GameReady);
#endif
        gameObject.SetActive(false);
    }
}
