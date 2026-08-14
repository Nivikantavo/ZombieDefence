using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
#endif

public class PlaygamaPaymentsGateway : IPaymentsGateway
{
    private readonly IapCatalog _catalog;

    public PlaygamaPaymentsGateway(IapCatalog catalog)
    {
        _catalog = catalog;
    }

    public static bool IsPlaygamaPlatform
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Bridge.instance == null)
                return false;

            string platformId = Bridge.platform.id;
            return string.IsNullOrEmpty(platformId) == false
                && platformId.IndexOf("playgama", StringComparison.OrdinalIgnoreCase) >= 0;
#else
            return false;
#endif
        }
    }

    public bool IsSupported
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Bridge.payments.isSupported;
#else
            return true;
#endif
        }
    }

    public void Purchase(string productId, Action<bool, Dictionary<string, string>> onComplete)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.payments.Purchase(productId, (success, purchase) =>
        {
            onComplete?.Invoke(success, purchase);
        });
#else
        Debug.Log($"IAP purchase skipped in Editor: {productId}");
        onComplete?.Invoke(false, null);
#endif
    }

    public void GetCatalog(Action<bool, List<Dictionary<string, string>>> onComplete)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.payments.GetCatalog((success, catalog) =>
        {
            onComplete?.Invoke(success, catalog);
        });
#else
        onComplete?.Invoke(true, CreateEditorCatalog());
#endif
    }

    public void GetPurchases(Action<bool, List<Dictionary<string, string>>> onComplete)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.payments.GetPurchases((success, purchases) =>
        {
            onComplete?.Invoke(success, purchases);
        });
#else
        onComplete?.Invoke(true, new List<Dictionary<string, string>>());
#endif
    }

    public void Consume(string productId, Action<bool, Dictionary<string, string>> onComplete)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.payments.ConsumePurchase(productId, (success, purchase) =>
        {
            onComplete?.Invoke(success, purchase);
        });
#else
        onComplete?.Invoke(true, null);
#endif
    }

    private List<Dictionary<string, string>> CreateEditorCatalog()
    {
        List<Dictionary<string, string>> catalog = new List<Dictionary<string, string>>();
        if (_catalog == null)
            return catalog;

        IReadOnlyList<IapProductDefinition> products = _catalog.Products;
        for (int i = 0; i < products.Count; i++)
        {
            IapProductDefinition product = products[i];
            if (product == null || string.IsNullOrEmpty(product.Id))
                continue;

            catalog.Add(new Dictionary<string, string>
            {
                { "id", product.Id },
                { "price", product.EditorPriceLabel },
                { "priceValue", product.EditorPriceLabel }
            });
        }

        return catalog;
    }
}
