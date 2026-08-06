using GameAnalyticsSDK;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_WEBGL
using Playgama;
#endif

public class Shop : MonoBehaviour
{
    private const string TruckHealth = "TruckHealth";
    private const string Granade = "Granade";

    [SerializeField] private List<ItemView> _itemViews;
    [SerializeField] private List<ProductView> _productsView;
    [SerializeField] private ImproveItem _granadeItem;
    [SerializeField] private ImproveItem _truckHealthItem;
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private GameObject _authorizePanel;

    private PlayerData _playerData;
    private int _startTruckHealth = 300;
    private int _startGranadeCount = 1;
    private float _checkDataDelay = 0.25f;

    public event UnityAction ItemBought;

    private void OnEnable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Bridge.payments.isSupported)
            Bridge.payments.GetCatalog(OnGetCatalogCompleted);
#endif
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick += TrySellItem;
        }
        foreach (var productView in _productsView)
        {
            productView.PoductViewClick += TrySellProduct;
        }
        if (SaveSystem.Instance.DataLoaded)
        {
            UpdateData();
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
            yield return new WaitForSecondsRealtime(_checkDataDelay);
        }
        UpdateData();
    }

    private void UpdateData()
    {
        _playerData = SaveSystem.Instance.GetData();
        MarkAllBoughtItem();
    }

    private void TrySellItem(Item item)
    {
        if(item.Purchases < item.NumberOfItems)
        {
            if (item is WeaponItem)
            {
                TrySellWeapon(item as WeaponItem);
            }
            else if (item is ImproveItem)
            {
                TrySellImprovment(item as ImproveItem);
            }
            else if (item is ForceItem)
            {
                TrySellForce(item as ForceItem);
            }
            ItemBought?.Invoke();
        }
    }

    private void UpdateProductCatalog(List<CatalogProduct> products)
    {
        for (int i = 0; i < products.Count && i < _productsView.Count; i++)
        {
            _productsView[i].Product = products[i];
        }
    }

    private void TrySellProduct(Item item, string id)
    {
#if UNITY_WEBGL
        Bridge.payments.Purchase(id, (success, purchase) =>
        {
            if (success == false || purchase == null)
                return;

            string productId = purchase.TryGetValue("id", out string value) ? value : id;
            SaveSystem.Instance.SetBoughtProduct(productId);
            AddBoughtWeapon(item as WeaponItem);

            foreach (ProductView view in _productsView)
            {
                if (view.ProductID == productId)
                {
                    view.OnSellSuccessfully();
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, item.Name, 1, item.Name, productId);
                }
            }
        });
#endif
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
        else if(weapon.Purchases < weapon.NumberOfItems && _moneyCollecter.TrySpendMoney(weapon.SellingPrice))
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
        if(improveItem.Name == TruckHealth)
        {
            int truckHealth = _playerData.TruckHealth;
            truckHealth += improveItem.ImproveStep;
            SaveSystem.Instance.SetTruckHealth(truckHealth);
        }
        else if(improveItem.Name == Granade)
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
        SaveSystem.Instance.SetForcesArrey(forces.ToArray());
    }

    private void MarkAllBoughtItem()
    {
        MarkBoughtWeapon();
        MarkBoughtForces();
        MarkBoughtImpruvment();
#if UNITY_WEBGL && !UNITY_EDITOR
        GetBoughtProducts();
#endif
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
        int granadeBought = (_playerData.GranadesCount - _startGranadeCount) / _granadeItem.ImproveStep;
        int truckHealthBiught = (_playerData.TruckHealth - _startTruckHealth) / _truckHealthItem.ImproveStep;

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

#if UNITY_WEBGL
    private void OnGetCatalogCompleted(bool success, List<Dictionary<string, string>> catalog)
    {
        if (success == false || catalog == null)
            return;

        List<CatalogProduct> products = new List<CatalogProduct>();
        foreach (var item in catalog)
        {
            CatalogProduct product = CatalogProduct.FromDictionary(item);
            if (product != null)
                products.Add(product);
        }

        UpdateProductCatalog(products);
    }

    private void GetBoughtProducts()
    {
        Bridge.payments.GetPurchases((success, purchases) =>
        {
            if (success && purchases != null)
                MarkBoughtProduct(purchases);
        });
    }

    private void MarkBoughtProduct(List<Dictionary<string, string>> products)
    {
        foreach (ProductView view in _productsView)
        {
            foreach (var product in products)
            {
                string productId = product.TryGetValue("id", out string id) ? id : null;
                if (productId == view.ProductID)
                {
                    view.OnSellSuccessfully();
                }
            }
        }
    }
#endif

    public void DeleteProducts()
    {
#if UNITY_WEBGL
        Bridge.payments.GetPurchases((success, purchases) =>
        {
            if (success && purchases != null)
                DeleteAllPurchasedProducts(purchases);
        });
#endif
    }

#if UNITY_WEBGL
    private void DeleteAllPurchasedProducts(List<Dictionary<string, string>> products)
    {
        foreach (var product in products)
        {
            string productId = product.TryGetValue("id", out string id) ? id : null;
            if (string.IsNullOrEmpty(productId) == false)
            {
                Bridge.payments.ConsumePurchase(productId, null);
            }
        }
    }
#endif
}
