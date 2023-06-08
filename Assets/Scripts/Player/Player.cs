using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
    [SerializeField] private Force _force;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            _force.UseForce();
        }
    }
}
