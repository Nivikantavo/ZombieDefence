using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayAlarm : MonoBehaviour
{
    [SerializeField] private TrackPosition _trackCoordinate;
    [SerializeField] private PlayerPosition _playerCoordinate;
    [SerializeField] private FarAwayAlarm _alarmPanel;
    public float LevelLimitDistance;
   
    void Update()
    {
        float currentDistance = Vector3.Distance(_playerCoordinate.transform.position,_trackCoordinate.transform.position);
        Debug.Log(currentDistance);

        if (currentDistance >= LevelLimitDistance) 
        {
            _alarmPanel.gameObject.SetActive(true);
        }
        else
        {
            _alarmPanel.gameObject.SetActive(false);
        }
    }
}
