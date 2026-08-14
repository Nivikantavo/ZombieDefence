using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Events;

public class IapPurchaseService : MonoBehaviour
{
    [SerializeField] private IapCatalog _catalog;

    private IPaymentsGateway _gateway;
    private IapFulfillment _fulfillment;
    private readonly Dictionary<string, CatalogProduct> _catalogById = new Dictionary<string, CatalogProduct>();
    private bool _initialized;

    public IapCatalog Catalog => _catalog;
    public bool IsSupported => _gateway != null && _gateway.IsSupported;
    public IReadOnlyDictionary<string, CatalogProduct> CatalogById => _catalogById;
    public string CurrencyIconUrl { get; private set; }

    public event UnityAction StateChanged;

    public void Initialize()
    {
        if (_initialized)
            return;

        if (_catalog == null)
            _catalog = Resources.Load<IapCatalog>("IapCatalog");

        if (_catalog == null)
        {
            Debug.LogError("IapCatalog is missing. Add Assets/Resources/IapCatalog.asset.");
            return;
        }

        _gateway = new PlaygamaPaymentsGateway(_catalog);
        _fulfillment = new IapFulfillment(_catalog);
        _initialized = true;
    }

    public void LoadCatalog()
    {
        Initialize();
        if (_gateway == null || _gateway.IsSupported == false)
            return;

        _gateway.GetCatalog(OnCatalogReceived);
    }

    public void RestorePurchases()
    {
        Initialize();
        if (_gateway == null || _gateway.IsSupported == false)
            return;

        _gateway.GetPurchases(OnPurchasesReceived);
    }

    public void Purchase(string productId)
    {
        Initialize();
        if (_gateway == null || string.IsNullOrEmpty(productId) || _gateway.IsSupported == false)
            return;

        _gateway.Purchase(productId, (success, purchase) =>
        {
            if (success == false)
                return;

            ProcessPurchase(productId, purchase, true);
        });
    }

    public string GetOfferedProductId(string baseProductId)
    {
        Initialize();
        if (_catalog == null)
            return null;

        IapProductDefinition definition = _catalog.GetById(baseProductId);
        if (definition == null)
            return null;

        if (definition.IsConsumable)
            return definition.Id;

        PlayerData data = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;
        if (data == null)
            return definition.Id;

        if (definition.Kind == IapProductKind.PermanentWeapon)
        {
            if (HasWeapon(data, definition.WeaponName) == false)
                return definition.Id;

            int currentUpgrades = data.GetWeaponUpgradeCount(definition.WeaponName);
            IapProductDefinition nextUpgrade = _catalog.GetUpgrade(definition.WeaponName, currentUpgrades + 1);
            return nextUpgrade != null ? nextUpgrade.Id : null;
        }

        return HasOwnedProduct(data, definition.Id) ? null : definition.Id;
    }

    public bool TryGetCatalogProduct(string productId, out CatalogProduct product)
    {
        return _catalogById.TryGetValue(productId, out product);
    }

    public void DeleteConsumablePurchases()
    {
        Initialize();
        if (_gateway == null || _catalog == null)
            return;
        _gateway.GetPurchases((success, purchases) =>
        {
            if (success == false || purchases == null)
                return;

            for (int i = 0; i < purchases.Count; i++)
            {
                string productId = IapFulfillment.GetProductId(purchases[i], null);
                if (string.IsNullOrEmpty(productId) == false && _catalog.IsConsumable(productId))
                    _gateway.Consume(productId, null);
            }
        });
    }

    private void OnCatalogReceived(bool success, List<Dictionary<string, string>> catalog)
    {
        if (success == false || catalog == null)
            return;

        _catalogById.Clear();
        CurrencyIconUrl = null;
        for (int i = 0; i < catalog.Count; i++)
        {
            CatalogProduct product = CatalogProduct.FromDictionary(catalog[i]);
            if (product == null || string.IsNullOrEmpty(product.id))
                continue;

            _catalogById[product.id] = product;
            if (string.IsNullOrEmpty(CurrencyIconUrl) && string.IsNullOrEmpty(product.priceCurrencyImage) == false)
                CurrencyIconUrl = product.priceCurrencyImage;
        }

        StateChanged?.Invoke();
    }

    private void OnPurchasesReceived(bool success, List<Dictionary<string, string>> purchases)
    {
        if (success == false || purchases == null)
            return;

        List<string> ownedPermanentIds = new List<string>();
        for (int i = 0; i < purchases.Count; i++)
            ProcessPurchase(null, purchases[i], false, ownedPermanentIds);

        _fulfillment.ApplyPermanentProducts(ownedPermanentIds);
        StateChanged?.Invoke();
    }

    private void ProcessPurchase(
        string requestedId,
        Dictionary<string, string> purchase,
        bool notify,
        List<string> ownedPermanentIds = null)
    {
        string productId = IapFulfillment.GetProductId(purchase, requestedId);
        if (string.IsNullOrEmpty(productId) || _catalog.TryGetById(productId, out IapProductDefinition definition) == false)
            return;

        if (definition.IsConsumable)
        {
            string token = IapFulfillment.ResolveToken(purchase, productId);
            bool granted = _fulfillment.TryGrantConsumable(productId, token);
            if (granted && GameAnalytics.Initialized)
            {
                GameAnalytics.NewResourceEvent(
                    GAResourceFlowType.Source,
                    "Coins",
                    definition.CoinAmount,
                    "iap",
                    productId);
            }

            _gateway.Consume(productId, (consumeSuccess, _) =>
            {
                if (consumeSuccess && IapFulfillment.IsPendingToken(token))
                    _fulfillment.RemovePendingToken(token);
            });
        }
        else if (ownedPermanentIds != null)
        {
            if (ownedPermanentIds.Contains(productId) == false)
                ownedPermanentIds.Add(productId);
        }
        else
        {
            _fulfillment.ApplyPermanentProducts(new[] { productId });
            if (GameAnalytics.Initialized)
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, definition.WeaponName, 1, "iap", productId);
        }

        if (notify)
            StateChanged?.Invoke();
    }

    private static bool HasWeapon(PlayerData data, string weaponName)
    {
        if (data.Weapons == null || string.IsNullOrEmpty(weaponName))
            return false;

        for (int i = 0; i < data.Weapons.Length; i++)
        {
            if (data.Weapons[i] == weaponName)
                return true;
        }

        return false;
    }

    private static bool HasOwnedProduct(PlayerData data, string productId)
    {
        if (data.ProductsID == null || string.IsNullOrEmpty(productId))
            return false;

        for (int i = 0; i < data.ProductsID.Length; i++)
        {
            if (data.ProductsID[i] == productId)
                return true;
        }

        return false;
    }
}
