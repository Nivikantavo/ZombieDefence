using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] private List<LevelLeaderboard> _leaderboards;
    [SerializeField] private int _leaderboardsLenth;
    [SerializeField] private GameObject _authorizePanel;
    [SerializeField] private GameObject _showButton;
    [SerializeField] private GameObject _hideButton;
    [SerializeField] private Image _background;

    private Coroutine _loadRoutine;
    private bool _openAfterAuth;
    private const float LoadTimeoutSeconds = 8f;

    private IEnumerator Start()
    {
        while (PlatformServices.Instance == null)
            yield return null;
#if UNITY_WEBGL && !UNITY_EDITOR
        while (Playgama.Bridge.instance == null)
            yield return null;
#endif

        bool available = PlatformServices.Leaderboards != null && PlatformServices.Leaderboards.IsAvailable;
        if (_showButton != null)
            _showButton.SetActive(available);
        if (available == false && _hideButton != null)
            _hideButton.SetActive(false);
    }

    public void ShowLeaderboards()
    {
        if (PlatformServices.Leaderboards == null || PlatformServices.Leaderboards.IsAvailable == false)
            return;

        if (PlatformServices.Auth != null && PlatformServices.Auth.IsAuthorized == false)
        {
            _openAfterAuth = true;
            if (_authorizePanel != null)
                _authorizePanel.SetActive(true);
            return;
        }

        ShowLevelLeaderbord();
    }

    public void Authorize()
    {
        if (PlatformServices.Auth == null)
            return;

        PlatformServices.Auth.ShowAuthPrompt(success =>
        {
            if (success == false)
                return;

            if (_authorizePanel != null)
                _authorizePanel.SetActive(false);
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.Save();

            if (_openAfterAuth == false)
                return;

            _openAfterAuth = false;
            ShowLevelLeaderbord();
        });
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
        if (PlatformServices.Leaderboards == null)
        {
            leaderboard.MarkEntriesFailed();
            return;
        }

        PlatformServices.Leaderboards.GetEntries(leaderboard.Name, (success, entries) =>
        {
            if (success == false || entries == null)
            {
                OnGetEntriesError("Failed to load leaderboard entries");
                leaderboard.MarkEntriesFailed();
                return;
            }

            leaderboard.FillEntryesData(entries, _leaderboardsLenth);
        });
    }

    private void OnGetEntriesError(string error)
    {
        Debug.Log("ERROR: " + error);
    }
}
