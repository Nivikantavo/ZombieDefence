using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorHitBox : HitBox
{
    [SerializeField] private ArmorElement _helmet;

    protected override void Awake()
    {
        HitTarget = _helmet;
    }
}
