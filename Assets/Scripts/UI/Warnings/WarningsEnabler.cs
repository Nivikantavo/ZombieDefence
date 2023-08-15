using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningsEnabler : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private Player _player;

    [SerializeField] private Warning _truckWarning;
    [SerializeField] private Warning _playerWarnings;

    private void OnEnable()
    {
        _track.HasAttacked += OnTruckUnderAttack;
        _player.HasAttacked += OnPlayerUnderAttack;
    }

    private void OnDisable()
    {
        _track.HasAttacked -= OnTruckUnderAttack;
        _player.HasAttacked -= OnPlayerUnderAttack;
    }

    public void OnTruckUnderAttack()
    {
        ShowWarning(_truckWarning);
    }

    public void OnPlayerUnderAttack()
    {
        Debug.Log("OnPlayerUnderAttack");
        ShowWarning(_playerWarnings);
    }

    private void ShowWarning(Warning warning)
    {
        Debug.Log("ShowWarning" + warning.name);
        warning.gameObject.SetActive(true);
        warning.Show();
    }
}
