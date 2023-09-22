using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieCount : MonoBehaviour
{
    [SerializeField] private ZombieSpawner _zombieSpawner;
    [SerializeField] private TMP_Text _text;

    private void Update()
    {
        _text.text = $"{_zombieSpawner.DeadZombieCount} / {_zombieSpawner.ZombieCount}";
    }
}
