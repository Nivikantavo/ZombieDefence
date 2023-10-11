using Agava.YandexGames;
using System.Collections;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        return;
#endif
        StartCoroutine(CheckGameReady());
    }

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();
    }

    private IEnumerator CheckGameReady() 
    {
        while(YandexGamesSdk.IsInitialized == false && SaveSystem.Instance.DataLoaded == false)
        {
            yield return null;
        }
        YandexGamesSdk.GameReady();
        gameObject.SetActive(false);
    }
}
