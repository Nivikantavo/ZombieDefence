using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayAlarm : MonoBehaviour
{
    [SerializeField] private TrackPosition _trackCoordinate;
    [SerializeField] private PlayerPosition _playerCoordinate;
    public float LevelLimitDistance;
   
    void Update()
    {
        float currentDistance = Vector3.Distance(_playerCoordinate.transform.position,_trackCoordinate.transform.position);
        Debug.Log(currentDistance);

        if (currentDistance >= LevelLimitDistance) 
        {
            Debug.Log("daleko daleko");
        }
    }
}
