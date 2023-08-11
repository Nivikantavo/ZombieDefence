using Agava.YandexGames;
using Agava.YandexGames.Samples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<ItemView> _itemViews;
    [SerializeField] private List<ProductView> _productsView;
    [SerializeField] private ImproveItem _granadeItem;
    [SerializeField] private ImproveItem _truckHealthItem;
    [SerializeField] private MoneyCollecter _moneyCollecter;

    private PlayerData _playerData;

    public event UnityAction ItemBought;

    private void OnEnable()
    {
        Billing.GetProductCatalog(productCatalogReponse => UpdateProductCatalog(productCatalogReponse.products));
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick += TrySellItem;
        }
        foreach (var productView in _productsView)
        {
            productView.PoductViewClick += TrySellProduct;
        }
    }

    private void OnDisable()
    {
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick -= TrySellItem;
        }
        foreach (var productView in _productsView)
        {
            productView.PoductViewClick -= TrySellProduct;
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
        ItemBought?.Invoke();
    }

    private void UpdateProductCatalog(CatalogProduct[] products)
    {
        for (int i = 0; i < products.Length; i++)
        {
            _productsView[i].Product = products[i];
        }
    }

    private void TrySellProduct(Item item, string id)
    {
        Billing.PurchaseProduct(id, (purchaseProduct) =>
        {
            SaveSystem.Instance.SetBoughtProduct(purchaseProduct.purchaseData.productID);

            foreach (ProductView view in _productsView)
            {
                if (view.ProductID == purchaseProduct.purchaseData.productID)
                {
                    view.OnSellSuccessfully();
                    TrySellItem(item);
                }
            }
        });
    }

    private void TrySellWeapon(WeaponItem weapon)
    {
        if (weapon.Purchases == 0)
        {
            if (_moneyCollecter.TrySpendMoney(weapon.SellingPrice))
            {
                weapon.Sell();
                AddBoughtWeapon(weapon);
            }
        }
        else if(weapon.Purchases < weapon.NumberOfItems)
        {
            weapon.Sell();
            AddWeaponUpgrade(weapon);
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

    private void AddWeaponUpgrade(WeaponItem weaponItem)
    {
        List<string> upgradeWeapons = _playerData.UpgradeWeapons.ToList();
        upgradeWeapons.Add(weaponItem.Weapon.WeaponName);
        SaveSystem.Instance.SetWeaponsUpgradeArrey(upgradeWeapons.ToArray());
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
        MarkBoughtWeapon();
        MarkBoughtForces();
        MarkBoughtImpruvment();
        GetBoughtProducts();
    }

    private void MarkBoughtWeapon()
    {
        List<string> boughtWeapons = _playerData.Weapons.ToList();
        List<string> boughtWeaponsUpgrade = _playerData.UpgradeWeapons.ToList();

        int boughtCount;

        foreach (var view in _itemViews)
        {
            boughtCount = 0;
            foreach (var boughtWeapon in boughtWeapons)
            {
                if (view.ItemName == boughtWeapon)
                {
                    boughtCount++;
                }
            }

            foreach (var boughtUpgrade in boughtWeaponsUpgrade)
            {
                if (view.ItemName == boughtUpgrade)
                {
                    boughtCount++;
                }
            }
            view.MarkItemAsBought(boughtCount);
        }
    }

    private void MarkBoughtForces()
    {
        List<string> boughtForces = _playerData.Forces.ToList();

        foreach (var items in boughtForces)
        {
            foreach (var view in _itemViews)
            {
                if (view.ItemName == items)
                {
                    view.MarkItemAsBought(1);
                }
            }
        }
    }

    private void MarkBoughtImpruvment()
    {
        int granadeBought = (_playerData.GranadesCount - 1) / _granadeItem.ImproveStep;
        int truckHealthBiught = (_playerData.TruckHealth - 300) / _truckHealthItem.ImproveStep;

        foreach (var view in _itemViews)
        {
            if (view.ItemName == _granadeItem.Name)
            {
                view.MarkItemAsBought(granadeBought);
            }

            if (view.ItemName == _truckHealthItem.Name)
            {
                view.MarkItemAsBought(truckHealthBiught);
            }
        }
    }

    private void GetBoughtProducts()
    {
        Billing.GetPurchasedProducts(purchasedProductsResponse => MarkBoughtProduct(purchasedProductsResponse.purchasedProducts));
    }

    private void MarkBoughtProduct(PurchasedProduct[] products)
    {
        foreach (ProductView view in _productsView)
        {
            foreach(var product in products)
            {
                if(product.productID == view.ProductID)
                {
                    view.OnSellSuccessfully();
                }
            }
        }
    }
}
