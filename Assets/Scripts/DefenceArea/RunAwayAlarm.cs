using UnityEngine;

public class RunAwayAlarm : MonoBehaviour
{
    [SerializeField] private Transform _track;
    [SerializeField] private Transform _player;
    [SerializeField] private FarAwayAlarmHandler _alarmPanel;
    [SerializeField] private GameObject _zoneLine;
    [SerializeField] private float _levelLimitDistance;

    private void Awake()
    {
        transform.position = new Vector3(_track.position.x, transform.position.y, _track.position.z);
        _zoneLine.transform.localScale = new Vector3(_levelLimitDistance, _zoneLine.transform.localScale.y, _levelLimitDistance);
    }

    private void Update()
    {
        float currentDistance = Vector3.Distance(_player.position, _track.position);

        if (currentDistance >= _levelLimitDistance)
        {
            _alarmPanel.gameObject.SetActive(true);
        }
        else
        {
            _alarmPanel.gameObject.SetActive(false);
        }
    }
}
