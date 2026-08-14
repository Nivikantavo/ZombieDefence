using System;
using UnityEngine;

public enum IapProductKind
{
    PermanentWeapon,
    PermanentUpgrade,
    ConsumableCurrency
}

[Serializable]
public class IapProductDefinition
{
    [Tooltip("Must match playgama-bridge-config.json and the Yandex Games console.")]
    public string Id;
    public IapProductKind Kind;
    [Tooltip("Weapon.WeaponName for weapon and upgrade products.")]
    public string WeaponName;
    [Tooltip("1 = first upgrade (level 2). 0 for weapons and currency.")]
    public int UpgradeIndex;
    [Tooltip("Coins granted for consumable packs.")]
    public int CoinAmount;
    [Tooltip("Fallback price label in Editor. On Playgama this is Gam, not USD.")]
    public string EditorPriceLabel;

    public bool IsConsumable => Kind == IapProductKind.ConsumableCurrency;
}
