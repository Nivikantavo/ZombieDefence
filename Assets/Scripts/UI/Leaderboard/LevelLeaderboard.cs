using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
#endif

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

    public void FillEntryesData(List<Dictionary<string, string>> entries, int length)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int count = Mathf.Min(entries.Count, length);
        Dictionary<string, string> playerEntry = null;

        for (int i = 0; i < count; i++)
        {
            Dictionary<string, string> entry = entries[i];
            Sprite background = i < _backgrounds.Count ? _backgrounds[i] : _backgrounds[_backgrounds.Count - 1];
            FillView(entry, background);

            if (IsCurrentPlayer(entry))
                playerEntry = entry;
        }

        if (playerEntry == null)
            playerEntry = FindPlayerEntry(entries);

        if (playerEntry != null)
        {
            int playerRank = ParseInt(playerEntry, "rank");
            if (playerRank > length)
            {
                var emptySlot = Instantiate(_emptySlot, _content);
                _entryViews.Add(emptySlot.gameObject);
                FillView(playerEntry, _backgrounds[_backgrounds.Count - 1]);
            }
        }

        _entryesLoaded = true;
#else
        _entryesLoaded = true;
#endif
    }

    public void MarkEntriesFailed()
    {
        _entryesLoaded = true;
    }

    private void FillView(Dictionary<string, string> entry, Sprite background)
    {
        var view = Instantiate(_scoreViewPrefab, _content);
        int rank = ParseInt(entry, "rank");
        int score = ParseInt(entry, "score");
        string nickname = entry.TryGetValue("name", out string name) ? name : null;
        view.Initialize(rank, score / 1000, nickname, background);
        _entryViews.Add(view.gameObject);
    }

    private Dictionary<string, string> FindPlayerEntry(List<Dictionary<string, string>> entries)
    {
        foreach (var entry in entries)
        {
            if (IsCurrentPlayer(entry))
                return entry;
        }

        return null;
    }

    private bool IsCurrentPlayer(Dictionary<string, string> entry)
    {
#if UNITY_WEBGL
        if (entry.TryGetValue("id", out string entryId)
            && string.IsNullOrEmpty(Bridge.player.id) == false
            && entryId == Bridge.player.id)
        {
            return true;
        }

        if (entry.TryGetValue("name", out string entryName)
            && string.IsNullOrEmpty(Bridge.player.name) == false
            && entryName == Bridge.player.name)
        {
            return true;
        }
#endif
        return false;
    }

    private static int ParseInt(Dictionary<string, string> entry, string key)
    {
        if (entry.TryGetValue(key, out string value) && int.TryParse(value, out int result))
            return result;

        return 0;
    }
}
