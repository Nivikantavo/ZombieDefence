using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<ItemView> _itemViews;
    [SerializeField] private MoneyCollecter _moneyCollecter;

    private void OnEnable()
    {
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick += TrySellItem;
        }
    }

    private void OnDisable()
    {
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick -= TrySellItem;
        }
    }

    private void TrySellItem(Item item)
    {
        if (item.IsBought == false)
        {
            if (_moneyCollecter.TrySpendMoney(item.SellingPrice))
            {
                item.Sell();
            }
        }
    }
}
