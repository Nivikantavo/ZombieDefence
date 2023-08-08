using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private List<LevelLeaderboard> _leaderboards;
    [SerializeField] private int _leaderboardsLenth;

    private IEnumerator Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        yield return YandexGamesSdk.Initialize();
#endif
        yield return null;
    }

    public void ShowLeaderboards()
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
        if (YandexGamesSdk.IsInitialized)
        {
            if (PlayerAccount.IsAuthorized == false)
            {
                PlayerAccount.Authorize(ShowLevelLeaderbord, null);
            }
            else
            {
                ShowLevelLeaderbord();
            }
        }
//#endif

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
        //bool canEnableNext = false;
        //foreach (var leaderboard in _leaderboards)
        //{
        //    leaderboard.gameObject.SetActive(true);
        //    if(leaderboard.EntryesLoaded == false)
        //    {
        //        FillLeaderboard(leaderboard);
        //    }
        //}

        StartCoroutine(SetLeaderboardsData());
    }

    private void FillLeaderboard(LevelLeaderboard leaderboard)
    {
        Debug.Log("FillLeaderboard " + leaderboard.Name);
        Leaderboard.GetEntries(leaderboard.Name, (result) =>
        {
            leaderboard.FillEntryesData(result, _leaderboardsLenth);
            
        }, OnGetEntriesError, _leaderboardsLenth);
    }

    private void OnGetEntriesError(string error)
    {
        Debug.Log("ERROR: " + error);
    }

    private IEnumerator SetLeaderboardsData()
    {
        foreach (var leaderboard in _leaderboards)
        {
            leaderboard.gameObject.SetActive(true);
            Leaderboard.GetEntries(leaderboard.Name, (result) =>
            {
                leaderboard.FillEntryesData(result, _leaderboardsLenth);

            }, OnGetEntriesError, _leaderboardsLenth);

            while (leaderboard.EntryesLoaded == false)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
