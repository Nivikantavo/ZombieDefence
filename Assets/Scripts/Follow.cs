using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private void FixedUpdate()
    {
        transform.position = _player.position;
    }
}
