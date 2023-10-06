using UnityEngine;

public class ForceItem : Item
{
    public string ForceName => _forceName;

    [SerializeField] private string _forceName;
}
