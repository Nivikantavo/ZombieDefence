using UnityEngine;

public class WaveProgressBar : Bar
{
    [SerializeField] private ZombieSpawner _spawner;

    private void OnEnable()
    {
        _spawner.ZombiesCounted += OnZombyCounted;
        _spawner.ZombieDied += OnTrackingValueChanged;
    }

    private void OnZombyCounted()
    {
        Slider.maxValue = _spawner.ZombieCount;
        Slider.value = _spawner.DeadZombieCount;
    }
}
