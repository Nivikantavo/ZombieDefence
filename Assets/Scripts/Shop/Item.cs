using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public virtual string Name => ItemName;
    public virtual int SellingPrice => Price;
    public bool IsBought => Bought;
    public bool IsSoldOnce => SoldOnce;

    [SerializeField] protected int Price;
    [SerializeField] protected string ItemName;
    [SerializeField] protected bool Bought;
    [SerializeField] protected bool SoldOnce;

    public event UnityAction ItemBought;

    public virtual void Sell()
    {
        if(Bought == false || SoldOnce == false)
        {
            Bought = true;
            ItemBought?.Invoke();
        }
    }

    public virtual void MarkAsBought()
    {
        Bought = true;
        ItemBought?.Invoke();
    }
}
