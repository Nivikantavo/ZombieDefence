using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public virtual string Name => ItemName;
    public int SellingPrice => Price;
    public bool IsBought => Bought;

    [SerializeField] protected int Price;
    [SerializeField] protected string ItemName;
    [SerializeField] protected bool Bought;

    public event UnityAction ItemBought;

    public virtual void Sell()
    {
        if(Bought == false)
        {
            Bought = true;
            ItemBought?.Invoke();
        }
    }
}
