using Agava.YandexGames;
using System.Collections;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();

        gameObject.SetActive(false);
    }
}
