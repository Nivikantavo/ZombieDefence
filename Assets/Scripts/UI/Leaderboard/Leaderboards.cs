using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private List<LevelLeaderboard> _leaderboards;

    private IEnumerator Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        yield return YandexGamesSdk.Initialize();
#endif
        yield return null;
    }

    public void ShowLeaderboards()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (YandexGamesSdk.IsInitialized)
        {
            if (PlayerAccount.IsAuthorized == false)
            {
                Debug.Log("IsAuthorized == false");
                PlayerAccount.Authorize(ShowLevelLeaderbord, null);
            }
            else
            {
                Debug.Log("IsAuthorized == true");
                ShowLevelLeaderbord();
            }
        }
#endif

#if UNITY_EDITOR
        ShowLevelLeaderbord();
#endif
    }

    public void HideLeaderboards()
    {
        foreach (var leaderboard in _leaderboards)
        {
            leaderboard.gameObject.SetActive(false);
        }
    }

    private void ShowLevelLeaderbord()
    {
        foreach (var leaderboard in _leaderboards)
        {
            leaderboard.gameObject.SetActive(true);
#if UNITY_WEBGL && !UNITY_EDITOR
            leaderboard.InitializePlayerEntries();
#endif
        }
    }
}
