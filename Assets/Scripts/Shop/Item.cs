using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int SellingPrice => Price;
    public bool IsBought => Bought;

    [SerializeField] protected int Price;
    [SerializeField] protected string ItemName;
    [SerializeField] protected bool Bought;

    public virtual void Sell()
    {
        if(Bought == false)
        {
            Bought = true;
        }
    }
}
