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
                return StartPrice + StartPrice * BoughtCount; 
            else 
                return StartPrice; 
            } 
    }

    [SerializeField] private int _improveStep;
}
