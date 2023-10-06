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
        ShowWarning(_playerWarnings);
    }

    private void ShowWarning(Warning warning)
    {
        warning.gameObject.SetActive(true);
        warning.Show();
    }
}
