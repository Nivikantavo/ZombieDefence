using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private Transform _coinsRoot;
    [SerializeField] private ProductView _currencyItemPrefab;

    private PlayerData _playerData;
    private IapPurchaseService _iap;
    private bool _currencyViewsCreated;
    private string _pendingProductId;
    private int _startTruckHealth = 300;
    private int _startGranadeCount = 1;
    private float _checkDataDelay = 0.25f;

    private static readonly string[] CurrencyProductIds =
    {
        IapProductIds.Coins1000,
        IapProductIds.Coins7000,
        IapProductIds.Coins10000
    };

    public event UnityAction ItemBought;

    private void OnEnable()
    {
        foreach (var itemView in _itemViews)
        {
            itemView.ViewClick += TrySellItem;
        }
        foreach (var productView in _productsView)
        {
            productView.PoductViewClick += TrySellProduct;
        }
        if (PlatformServices.Auth != null)
            PlatformServices.Auth.Authorized += OnAuthorized;
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DataUpdated += OnSaveDataUpdated;
        PlatformServices.Banners?.ShowMainMenu();
        if (SaveSystem.Instance != null && SaveSystem.Instance.DataLoaded)
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
        if (PlatformServices.Auth != null)
            PlatformServices.Auth.Authorized -= OnAuthorized;
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DataUpdated -= OnSaveDataUpdated;
    }

    private IEnumerator Start()
    {
        while (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(_checkDataDelay);
        }

        if (isActiveAndEnabled)
        {
            SaveSystem.Instance.DataUpdated -= OnSaveDataUpdated;
            SaveSystem.Instance.DataUpdated += OnSaveDataUpdated;
            UpdateData();
        }
    }

    private void OnSaveDataUpdated()
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
            return;

        _playerData = SaveSystem.Instance.GetData();
        MarkAllBoughtItem();
        RefreshIapViews();
    }

    private void UpdateData()
    {
        _playerData = SaveSystem.Instance.GetData();
        _iap = SaveSystem.Instance.IapPurchases;
        if (_iap != null)
            _iap.Initialize();
        MarkAllBoughtItem();
        CreateCurrencyViews();
        RefreshIapViews();
    }

    private void CreateCurrencyViews()
    {
        if (_currencyViewsCreated || _coinsRoot == null || _currencyItemPrefab == null)
            return;

        for (int i = 0; i < CurrencyProductIds.Length; i++)
        {
            ProductView view = Instantiate(_currencyItemPrefab, _coinsRoot);
            view.gameObject.name = CurrencyProductIds[i];
            view.Configure(CurrencyProductIds[i]);
            view.PoductViewClick += TrySellProduct;
            _productsView.Add(view);
        }

        _currencyViewsCreated = true;
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

    private void TrySellProduct(Item item, string id)
    {
        if (_iap == null)
            return;

        if (PlatformServices.Auth != null
            && PlatformServices.Auth.RequiresAuthForIap
            && PlatformServices.Auth.IsAuthorized == false)
        {
            _pendingProductId = id;
            if (_authorizePanel != null)
                _authorizePanel.SetActive(true);
            return;
        }

        _iap.Purchase(id);
    }

    private void OnAuthorized()
    {
        if (_authorizePanel != null)
            _authorizePanel.SetActive(false);

        if (string.IsNullOrEmpty(_pendingProductId) || _iap == null)
            return;

        string productId = _pendingProductId;
        _pendingProductId = null;
        _iap.Purchase(productId);
    }

    private void RefreshIapViews()
    {
        bool supported = _iap != null && _iap.IsSupported;
        foreach (ProductView view in _productsView)
        {
            if (view == null)
                continue;

            view.gameObject.SetActive(supported);
            if (supported)
            {
                view.SetIapService(_iap);
                view.Refresh();
            }
        }
    }

    private void TrySellWeapon(WeaponItem weapon)
    {
        if (weapon.Purchases == 0)
        {
            if (_moneyCollecter.TrySpendMoney(weapon.SellingPrice, false))
            {
                weapon.Sell();
                AddBoughtWeapon(weapon);
            }
        }
        else if(weapon.CanUpgrade && weapon.Purchases < weapon.NumberOfItems && _moneyCollecter.TrySpendMoney(weapon.SellingPrice, false))
        {
            weapon.Sell();
            AddWeaponUpgrade(weapon);
        }
    }

    private void TrySellImprovment(ImproveItem improvment)
    {
        if (_moneyCollecter.TrySpendMoney(improvment.SellingPrice, false))
        {
            improvment.Sell();
            AddBoughtImprovement(improvment);
        }
    }

    private void TrySellForce(ForceItem force)
    {
        if (_moneyCollecter.TrySpendMoney(force.SellingPrice, false))
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
        List<string> upgradeWeapons = _playerData.UpgradeWeapons != null
            ? _playerData.UpgradeWeapons.ToList()
            : new List<string>();
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
    }

    private void MarkBoughtWeapon()
    {
        List<string> boughtWeapons = _playerData.Weapons != null
            ? _playerData.Weapons.ToList()
            : new List<string>();
        List<string> boughtWeaponsUpgrade = _playerData.UpgradeWeapons != null
            ? _playerData.UpgradeWeapons.ToList()
            : new List<string>();

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

    public void DeleteProducts()
    {
        if (_iap != null)
            _iap.DeleteConsumablePurchases();
    }
}
