using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : Target
{
    public float MaxFuel => _maxFuel;
    public float CurrentFuel => _currentFuel;

    [SerializeField] private BoxCollider _collider;
    [SerializeField] private float _maxFuel;
    [SerializeField] private float _fillingTime;

    private float _currentFuel;

    private void Awake()
    {
        _currentFuel = 0;
    }

    private void Start()
    {
        StartCoroutine(FillUpTank());
    }

    public override Vector3 GetClosesetPositin(Vector3 position)
    {
        return _collider.ClosestPointOnBounds(position);
    }

    private IEnumerator FillUpTank()
    {
        WaitForSeconds fillDelay = new WaitForSeconds(_fillingTime / 100);

        while(_currentFuel < _maxFuel)
        {
            _currentFuel++;
            yield return fillDelay;
        }
        
    }
}
