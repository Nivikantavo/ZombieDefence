using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_WEBGL
using Playgama;
#endif

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private List<LevelLeaderboard> _leaderboards;
    [SerializeField] private int _leaderboardsLenth;
    [SerializeField] private GameObject _authorizePanel;
    [SerializeField] private GameObject _showButton;
    [SerializeField] private GameObject _hideButton;
    [SerializeField] private Image _background;

    private Coroutine _loadRoutine;
    private const float LoadTimeoutSeconds = 8f;

    public void ShowLeaderboards()
    {
#if UNITY_WEBGL
        if (Bridge.player.isAuthorized == false)
        {
            _authorizePanel.SetActive(true);
            return;
        }
#endif
        ShowLevelLeaderbord();
    }

    public void Authorize()
    {
#if UNITY_WEBGL
        Bridge.player.Authorize(new Dictionary<string, object>(), success =>
        {
            if (success)
                OnAuthotizeSuccess();
        });
#endif
    }

    private void OnAuthotizeSuccess()
    {
        _authorizePanel.SetActive(false);
        SaveSystem.Instance.Load();
        ShowLevelLeaderbord();
    }

    public void HideLeaderboards()
    {
        StopLoading();
        foreach (var leaderboard in _leaderboards)
        {
            leaderboard.gameObject.SetActive(false);
        }
        _background.enabled = false;
        _showButton.SetActive(true);
        _hideButton.SetActive(false);
    }

    private void OnDisable()
    {
        StopLoading();
    }

    private void ShowLevelLeaderbord()
    {
        StopLoading();
        _showButton.SetActive(false);
        _hideButton.SetActive(true);
        _loadRoutine = StartCoroutine(SetLeaderboardsData());
    }

    private void StopLoading()
    {
        if (_loadRoutine == null)
            return;

        StopCoroutine(_loadRoutine);
        _loadRoutine = null;
    }

    private IEnumerator SetLeaderboardsData()
    {
        _background.enabled = true;

        foreach (var leaderboard in _leaderboards)
            leaderboard.gameObject.SetActive(true);

        foreach (var leaderboard in _leaderboards)
        {
            if (leaderboard.EntryesLoaded == false)
            {
                FillLeaderboard(leaderboard);
                yield return WaitUntilLoaded(leaderboard);
            }

            yield return null;
        }

        _loadRoutine = null;
    }

    private IEnumerator WaitUntilLoaded(LevelLeaderboard leaderboard)
    {
        float elapsed = 0f;
        while (leaderboard.EntryesLoaded == false && elapsed < LoadTimeoutSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (leaderboard.EntryesLoaded == false)
            leaderboard.MarkEntriesFailed();
    }

    private void FillLeaderboard(LevelLeaderboard leaderboard)
    {
        leaderboard.gameObject.SetActive(true);
#if UNITY_WEBGL
        Bridge.leaderboards.GetEntries(leaderboard.Name, (success, entries) =>
        {
            if (success == false || entries == null)
            {
                OnGetEntriesError("Failed to load leaderboard entries");
                leaderboard.MarkEntriesFailed();
                return;
            }

            leaderboard.FillEntryesData(entries, _leaderboardsLenth);
        });
#else
        leaderboard.MarkEntriesFailed();
#endif
    }

    private void OnGetEntriesError(string error)
    {
        Debug.Log("ERROR: " + error);
    }
}
