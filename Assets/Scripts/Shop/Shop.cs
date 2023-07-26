using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<ItemView> _itemViews;
    [SerializeField] private List<WeaponItem> _weapons;
    [SerializeField] private ImproveItem _granadeItem;
    [SerializeField] private ImproveItem _truckHealthItem;
    [SerializeField] private MoneyCollecter _moneyCollecter;

    private PlayerData _playerData;

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

    private IEnumerator Start()
    {
        while (SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        _playerData = SaveSystem.Instance.GetData();
        MarkAllBoughtItem();
        Debug.Log(_playerData.GranadesCount);
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
        else if(item is ForceItem)
        {
            TrySellForce(item as ForceItem);
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
        if (_moneyCollecter.TrySpendMoney(improvment.SellingPrice))
        {
            improvment.Sell();
            AddBoughtImprovement(improvment);
        }
    }

    private void TrySellForce(ForceItem force)
    {
        if (_moneyCollecter.TrySpendMoney(force.SellingPrice))
        {
            force.Sell();
            AddBoughtForce(force);
        }
    }

    private void AddBoughtWeapon(WeaponItem weaponItem)
    {
        List<string> weapons = _playerData.Weapons.ToList();
        weapons.Add(weaponItem.Weapon.WeaponName);
        SaveSystem.Instance.SetWeaponsArrey(weapons.ToArray());
    }

    private void AddBoughtImprovement(ImproveItem improveItem)
    {
        if(improveItem.Name == "TruckHealth")
        {
            int truckHealth = _playerData.TruckHealth;
            truckHealth += improveItem.ImproveStep;
            SaveSystem.Instance.SetTruckHealth(truckHealth);
        }
        else if(improveItem.Name == "Granade")
        {
            int granadesCount = _playerData.GranadesCount;
            granadesCount += improveItem.ImproveStep;
            SaveSystem.Instance.SetGranadesCount(granadesCount);
        }
    }

    private void AddBoughtForce(ForceItem forceItem)
    {
        List<string> forces = _playerData.Forces.ToList();
        forces.Add(forceItem.ForceName);
        for (int i = 0; i < forces.Count; i++)
        {
            Debug.Log(forces[i]);
        }
        SaveSystem.Instance.SetForcesArrey(forces.ToArray());
    }

    private void MarkAllBoughtItem()
    {
        List<string> boughtItems = new List<string>();
        boughtItems.AddRange(_playerData.Weapons.ToList());
        boughtItems.AddRange(_playerData.Forces.ToList());
        boughtItems.Add(_granadeItem.Name);
        boughtItems.Add(_truckHealthItem.Name);

        int granadeBought = (_playerData.GranadesCount - 1) / _granadeItem.ImproveStep;
        int truckHealthBiught = (_playerData.TruckHealth - 300) / _truckHealthItem.ImproveStep ;
        _granadeItem.SetSellsNumber(granadeBought);
        _truckHealthItem.SetSellsNumber(truckHealthBiught);

        foreach (var items in boughtItems)
        {
            foreach (var view in _itemViews)
            {
                if (view.ItemName == items)
                {
                    view.MarkItemAsBought();
                }
            }
        }
        
    }
}
