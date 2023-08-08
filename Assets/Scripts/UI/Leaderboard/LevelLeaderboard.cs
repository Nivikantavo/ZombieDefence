using Agava.YandexGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLeaderboard : MonoBehaviour
{
    public string Name => _name;
    public bool EntryesLoaded => _entryesLoaded;

    [SerializeField] private string _name;
    [SerializeField] private PlayerEntryView _scoreViewPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private List<Sprite> _backgrounds;
    [SerializeField] private GameObject _emptySlot;

    private List<GameObject> _entryViews = new List<GameObject>();
    private bool _entryesLoaded = false;

    public void FillEntryesData(LeaderboardGetEntriesResponse entryesData, int lenth)
    {
        Debug.Log(gameObject.name + " " + _name);
        Debug.Log("LEADERBORD TITLE: " + entryesData.leaderboard.name + " Game Object: " + gameObject.name);

//#if UNITY_WEBGL && !UNITY_EDITOR

        for (int i = 0; i < entryesData.entries.Length; i++)
        {
            FillView(entryesData.entries[i], _backgrounds[i]);
        }

        if (entryesData.userRank > lenth - 1)
        {
            LeaderboardEntryResponse playerEntry = GetPlayerEntry();
            var emptySlot = Instantiate(_emptySlot, _content);
            _entryViews.Add(emptySlot.gameObject);
            FillView(playerEntry, _backgrounds[_backgrounds.Count - 1]);
        }

        _entryesLoaded = true;
//#endif
    }


    private void FillView(LeaderboardEntryResponse entry, Sprite background)
    {
        var view = Instantiate(_scoreViewPrefab, _content);
        view.Initialize(entry.rank, entry.score / 1000, entry.player.publicName, background);
        _entryViews.Add(view.gameObject);
    }

    private LeaderboardEntryResponse GetPlayerEntry()
    {
        LeaderboardEntryResponse playerEntry = null;
        Leaderboard.GetPlayerEntry(_name, (result) =>
        {
            if (result != null)
            {
                playerEntry = result;
            }
        });

        return playerEntry;
    }
}
