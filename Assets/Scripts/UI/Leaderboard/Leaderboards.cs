using Agava.YandexGames;
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
//#if UNITY_WEBGL && !UNITY_EDITOR
        if (YandexGamesSdk.IsInitialized)
        {
            if (PlayerAccount.IsAuthorized == false)
            {
                _authorizePanel.SetActive(true);
            }
            else
            {
                ShowLevelLeaderbord();
            }
        }
//#endif
    }

    public void Authorize()
    {
        PlayerAccount.Authorize(OnAuthotizeSuccess, null);
    }

    private void OnAuthotizeSuccess()
    {
        _authorizePanel.SetActive(false);
        SaveSystem.Instance.Load();
        ShowLevelLeaderbord();
    }

    public void HideLeaderboards()
    {
        foreach (var leaderboard in _leaderboards)
        {
            leaderboard.gameObject.SetActive(false);
        }
        _showButton.SetActive(true);
        _hideButton.SetActive(false);
    }

    private void ShowLevelLeaderbord()
    {
        StartCoroutine(SetLeaderboardsData());
        _showButton.SetActive(false);
        _hideButton.SetActive(true);
    }

    private IEnumerator SetLeaderboardsData()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        _background.enabled = true;
        foreach (var leaderboard in _leaderboards)
        {
            if (leaderboard.EntryesLoaded == false)
            {
                FillLeaderboard(leaderboard);
            }
            else
            {
                leaderboard.gameObject.SetActive(true);
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
