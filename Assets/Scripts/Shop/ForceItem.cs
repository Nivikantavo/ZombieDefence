using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceItem : Item
{
    public string ForceName => _forceName;

    [SerializeField] private string _forceName;
}
