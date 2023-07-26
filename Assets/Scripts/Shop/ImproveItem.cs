using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveItem : Item
{
    public int ImproveStep => _improveStep;
    public override int SellingPrice 
    { 
        get { 
            if(_sellsNumber > 0)
                return Price + Price * _sellsNumber; 
            else 
                return Price; 
            } 
    }

    [SerializeField] private int _improveStep;

    private int _sellsNumber = 0;

    public void SetSellsNumber(int sellsNumber)
    {
        _sellsNumber = sellsNumber;
    }

    public override void Sell()
    {
        SetSellsNumber(_sellsNumber + 1);
        base.Sell();
    }
}
