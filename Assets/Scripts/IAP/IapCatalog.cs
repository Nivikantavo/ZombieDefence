using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IapCatalog", menuName = "IAP/Catalog")]
public class IapCatalog : ScriptableObject
{
    [SerializeField] private List<IapProductDefinition> _products = new List<IapProductDefinition>();

    public IReadOnlyList<IapProductDefinition> Products => _products;

    public bool TryGetById(string id, out IapProductDefinition definition)
    {
        definition = GetById(id);
        return definition != null;
    }

    public IapProductDefinition GetById(string id)
    {
        if (string.IsNullOrEmpty(id) || _products == null)
            return null;

        for (int i = 0; i < _products.Count; i++)
        {
            IapProductDefinition product = _products[i];
            if (product != null && product.Id == id)
                return product;
        }

        return null;
    }

    public IapProductDefinition GetWeaponProduct(string weaponName)
    {
        if (string.IsNullOrEmpty(weaponName) || _products == null)
            return null;

        for (int i = 0; i < _products.Count; i++)
        {
            IapProductDefinition product = _products[i];
            if (product != null
                && product.Kind == IapProductKind.PermanentWeapon
                && product.WeaponName == weaponName)
            {
                return product;
            }
        }

        return null;
    }

    public IapProductDefinition GetUpgrade(string weaponName, int upgradeIndex)
    {
        if (string.IsNullOrEmpty(weaponName) || _products == null)
            return null;

        for (int i = 0; i < _products.Count; i++)
        {
            IapProductDefinition product = _products[i];
            if (product != null
                && product.Kind == IapProductKind.PermanentUpgrade
                && product.WeaponName == weaponName
                && product.UpgradeIndex == upgradeIndex)
            {
                return product;
            }
        }

        return null;
    }

    public int GetMaxUpgradeIndex(string weaponName)
    {
        int maxIndex = 0;
        if (string.IsNullOrEmpty(weaponName) || _products == null)
            return maxIndex;

        for (int i = 0; i < _products.Count; i++)
        {
            IapProductDefinition product = _products[i];
            if (product != null
                && product.Kind == IapProductKind.PermanentUpgrade
                && product.WeaponName == weaponName
                && product.UpgradeIndex > maxIndex)
            {
                maxIndex = product.UpgradeIndex;
            }
        }

        return maxIndex;
    }

    public bool IsConsumable(string id)
    {
        IapProductDefinition definition = GetById(id);
        return definition != null && definition.IsConsumable;
    }
}
