using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour
{
    public int Count => _count;

    [SerializeField] private int _count;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Collect()
    {
        _rigidbody.Sleep();
    }

    public void Spawn()
    {
        _rigidbody.WakeUp();
    }
}
