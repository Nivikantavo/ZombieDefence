using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLeaderboard : MonoBehaviour
{
    [SerializeField] private string _leaderboardName;
    [SerializeField] private PlayerEntryView _scoreViewPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private int _leaderboardsLenth;
    [SerializeField] private List<Sprite> _backgrounds;
    [SerializeField] private GameObject _emptySlot;

    public void InitializePlayerEntries()
    {
        Leaderboard.GetEntries(_leaderboardName, OnGetEntriesSuccess, null, _leaderboardsLenth);
    }

    private void OnGetEntriesSuccess(LeaderboardGetEntriesResponse result)
    {
        LeaderboardEntryResponse playerEntry = GetPlayerEntry();

        for (int i = 0; i < result.entries.Length; i++)
        {
            FillLeaderboard(result.entries[i], _backgrounds[i]);
        }

        if (playerEntry.rank > _leaderboardsLenth)
        {
            Instantiate(_emptySlot, _content);

            FillLeaderboard(playerEntry, _backgrounds[_backgrounds.Count - 1]);
        }
    }

    private void FillLeaderboard(LeaderboardEntryResponse entry, Sprite background)
    {
        var view = Instantiate(_scoreViewPrefab, _content);
        view.Initialize(entry.rank, entry.score, entry.player.publicName, background);
    }

    private LeaderboardEntryResponse GetPlayerEntry()
    {
        LeaderboardEntryResponse playerEntry = null;
        if (PlayerAccount.IsAuthorized)
        {
            Leaderboard.GetPlayerEntry(_leaderboardName, (result) =>
            {
                if (result != null)
                {
                    playerEntry = result;
                }
            });
        }

        return playerEntry;
    }
}
