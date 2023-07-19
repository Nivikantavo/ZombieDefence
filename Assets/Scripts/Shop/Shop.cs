using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (item is WeaponItem)
        {
            TrySellWeapon(item as WeaponItem);
        }
        else if(item is ImproveItem)
        {
            TrySellImprovment(item as ImproveItem);
        }
    }

    private void TrySellWeapon(WeaponItem weapon)
    {
        
        if (weapon.IsBought == false)
        {
            if (_moneyCollecter.TrySpendMoney(weapon.SellingPrice))
            {
                weapon.Sell();
                AddBoughtWeapon(weapon);
            }
        }
    }

    private void TrySellImprovment(ImproveItem improvment)
    {

        if (improvment.IsBought == false)
        {
            if (_moneyCollecter.TrySpendMoney(improvment.SellingPrice))
            {
                improvment.Sell();
            }
        }
    }

    private void AddBoughtWeapon(WeaponItem weaponItem)
    {
        List<string> weapons = SaveSystem.Instance.GetData().Weapons.ToList();
        weapons.Add(weaponItem.Weapon.WeaponName);
        SaveSystem.Instance.SetWeaponsArrey(weapons.ToArray());
    }
}
