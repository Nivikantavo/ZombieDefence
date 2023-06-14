using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackFillingViewer : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private Slider _fuelView;

    private void Awake()
    {
        _fuelView.maxValue = _track.MaxFuel;
    }

    private void Update()
    {
        _fuelView.value = _track.CurrentFuel;
    }
}
