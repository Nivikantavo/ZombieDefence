using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private List<LevelLeaderboard> _leaderboards;
    [SerializeField] private int _leaderboardsLenth;

    private float _delay = 0.01f;

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
                PlayerAccount.Authorize(ShowLevelLeaderbord, null);
            }
            else
            {
                ShowLevelLeaderbord();
            }
        }
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
        StartCoroutine(SetLeaderboardsData());
    }

    private IEnumerator SetLeaderboardsData()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);

        foreach (var leaderboard in _leaderboards)
        {
            if (leaderboard.EntryesLoaded == false)
            {
                FillLeaderboard(leaderboard);
            }

            while (leaderboard.EntryesLoaded == false)
            {
                yield return delay;
            }
        }
    }

    private void FillLeaderboard(LevelLeaderboard leaderboard)
    {
        leaderboard.gameObject.SetActive(true);
        Leaderboard.GetEntries(leaderboard.Name, (result) =>
        {
            leaderboard.FillEntryesData(result, _leaderboardsLenth);
            
        }, OnGetEntriesError, _leaderboardsLenth);
    }

    private void OnGetEntriesError(string error)
    {
        Debug.Log("ERROR: " + error);
    }

}
