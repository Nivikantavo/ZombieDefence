using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelWaves : MonoBehaviour
{
    [SerializeField] private ZombieSpawner _zombieSpawner;
    private List<Wave> _waves = new List<Wave>();

    private void Awake()
    {
        _waves = GetComponentsInChildren<Wave>().ToList();

        _zombieSpawner.SetLevelWaves(_waves);
    }
}
