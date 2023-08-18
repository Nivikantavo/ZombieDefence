using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveItem : Item
{
    public int ImproveStep => _improveStep;
    public override int SellingPrice 
    { 
        get { 
            if(BoughtCount > 0)
                return Price + Price * BoughtCount; 
            else 
                return Price; 
            } 
    }

    [SerializeField] private int _improveStep;

    public override void Sell()
    {
        base.Sell();
    }
}
