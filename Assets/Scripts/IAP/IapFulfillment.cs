using System.Collections.Generic;

public class IapFulfillment
{
    private readonly IapCatalog _catalog;

    public IapFulfillment(IapCatalog catalog)
    {
        _catalog = catalog;
    }

    public bool ApplyPermanentProducts(IReadOnlyList<string> ownedProductIds)
    {
        if (ownedProductIds == null || ownedProductIds.Count == 0)
            return false;

        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return false;

        data.EnsureIapArrays();

        Dictionary<string, int> upgradeByWeapon = new Dictionary<string, int>();
        bool changed = false;

        for (int i = 0; i < ownedProductIds.Count; i++)
        {
            string productId = ownedProductIds[i];
            IapProductDefinition definition = _catalog.GetById(productId);
            if (definition == null || definition.IsConsumable)
                continue;

            changed |= AddUnique(ref data.ProductsID, productId);

            if (string.IsNullOrEmpty(definition.WeaponName) == false)
            {
                changed |= AddUnique(ref data.Weapons, definition.WeaponName);

                if (definition.Kind == IapProductKind.PermanentUpgrade)
                {
                    if (upgradeByWeapon.TryGetValue(definition.WeaponName, out int current) == false
                        || definition.UpgradeIndex > current)
                    {
                        upgradeByWeapon[definition.WeaponName] = definition.UpgradeIndex;
                    }
                }
            }
        }

        foreach (KeyValuePair<string, int> pair in upgradeByWeapon)
        {
            changed |= SetMinUpgradeCount(data, pair.Key, pair.Value);
        }

        if (changed)
            SaveSystem.Instance.SaveAndNotify();

        return changed;
    }

    public bool TryGrantConsumable(string productId, string token)
    {
        IapProductDefinition definition = _catalog.GetById(productId);
        if (definition == null || definition.IsConsumable == false)
            return false;

        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return false;

        data.EnsureIapArrays();

        if (HasToken(data, token))
            return false;

        if (definition.CoinAmount > 0)
            data.Money += definition.CoinAmount;

        AddUnique(ref data.ProcessedPurchaseTokens, token);
        SaveSystem.Instance.SaveAndNotify();
        return true;
    }

    public void RemovePendingToken(string token)
    {
        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return;

        data.EnsureIapArrays();
        if (RemoveValue(ref data.ProcessedPurchaseTokens, token) == false)
            return;

        SaveSystem.Instance.Save();
    }

    public bool HasProcessedToken(string token)
    {
        PlayerData data = SaveSystem.Instance.GetData();
        if (data == null)
            return false;

        data.EnsureIapArrays();
        return HasToken(data, token);
    }

    public static string ResolveToken(Dictionary<string, string> purchase, string productId)
    {
        string token = GetValue(purchase, "purchaseToken");
        if (string.IsNullOrEmpty(token))
            token = GetValue(purchase, "token");

        if (string.IsNullOrEmpty(token) == false && token != productId)
            return token;

        return "pending_" + productId;
    }

    public static bool IsPendingToken(string token)
    {
        return string.IsNullOrEmpty(token) == false && token.StartsWith("pending_");
    }

    public static string GetProductId(Dictionary<string, string> purchase, string fallbackId)
    {
        string productId = GetValue(purchase, "id");
        if (string.IsNullOrEmpty(productId))
            productId = GetValue(purchase, "productId");
        if (string.IsNullOrEmpty(productId))
            productId = GetValue(purchase, "productID");

        return string.IsNullOrEmpty(productId) ? fallbackId : productId;
    }

    private static bool SetMinUpgradeCount(PlayerData data, string weaponName, int requiredCount)
    {
        int current = data.GetWeaponUpgradeCount(weaponName);
        if (current >= requiredCount)
            return false;

        List<string> upgrades = data.UpgradeWeapons != null
            ? new List<string>(data.UpgradeWeapons)
            : new List<string>();

        int missing = requiredCount - current;
        for (int i = 0; i < missing; i++)
            upgrades.Add(weaponName);

        data.UpgradeWeapons = upgrades.ToArray();
        return true;
    }

    private static bool HasToken(PlayerData data, string token)
    {
        if (string.IsNullOrEmpty(token) || data.ProcessedPurchaseTokens == null)
            return false;

        for (int i = 0; i < data.ProcessedPurchaseTokens.Length; i++)
        {
            if (data.ProcessedPurchaseTokens[i] == token)
                return true;
        }

        return false;
    }

    private static bool AddUnique(ref string[] values, string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (values == null)
        {
            values = new[] { value };
            return true;
        }

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == value)
                return false;
        }

        string[] next = new string[values.Length + 1];
        values.CopyTo(next, 0);
        next[values.Length] = value;
        values = next;
        return true;
    }

    private static bool RemoveValue(ref string[] values, string value)
    {
        if (values == null || string.IsNullOrEmpty(value))
            return false;

        int index = -1;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == value)
            {
                index = i;
                break;
            }
        }

        if (index < 0)
            return false;

        string[] next = new string[values.Length - 1];
        if (index > 0)
            System.Array.Copy(values, 0, next, 0, index);
        if (index < values.Length - 1)
            System.Array.Copy(values, index + 1, next, index, values.Length - index - 1);

        values = next;
        return true;
    }

    private static string GetValue(Dictionary<string, string> source, string key)
    {
        if (source == null)
            return null;

        return source.TryGetValue(key, out string value) ? value : null;
    }
}
