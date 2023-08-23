using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public virtual string Name => ItemName;
    public virtual int SellingPrice
    {
        get
        {
            if (BoughtCount > 0)
                return Price + Price * BoughtCount;
            else
                return Price;
        }
    }
    public int Purchases => BoughtCount;
    public int NumberOfItems => ItemsCount;

    [SerializeField] protected int Price;
    [SerializeField] protected string ItemName;
    [SerializeField] protected int BoughtCount;
    [SerializeField] protected int ItemsCount;

    public event UnityAction ItemBought;

    public virtual void Sell()
    {
        if(BoughtCount < ItemsCount)
        {
            BoughtCount++;
            ItemBought?.Invoke();
        }
    }

    public virtual void MarkAsBought(int bougtCount)
    {
        BoughtCount = bougtCount;
        ItemBought?.Invoke();
    }
}
