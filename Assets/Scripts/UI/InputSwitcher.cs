using Agava.WebUtility;
using Agava.YandexGames;
using System.Collections;
using UnityEngine;

public class InputSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();

        if (Device.IsMobile)
        {
            _mobileUI.SetActive(true);
        }
        else
        {
            _mobileUI.SetActive(false);
        }
    }
}
