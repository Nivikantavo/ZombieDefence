using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : Target
{
    [SerializeField] private BoxCollider _collider;

    public override Vector3 GetClosesetPositin(Vector3 position)
    {
        return _collider.ClosestPointOnBounds(position);
    }
}
