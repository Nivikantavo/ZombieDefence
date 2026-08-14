using InfimaGames.LowPolyShooterPack;
using UnityEngine;

public class WeaponItem : Item
{
    public override string Name => _weapon.WeaponName;
    public Weapon Weapon => _weapon;
    public bool CanUpgrade => _disableUpgrade == false;

    [SerializeField] private Weapon _weapon;
    [SerializeField] private bool _disableUpgrade;

    private void Awake()
    {
        ApplyItemCount();
    }

    private void OnValidate()
    {
        ApplyItemCount();
    }

    private void ApplyItemCount()
    {
        ItemsCount = CanUpgrade ? WeaponUpgradeLevels.MaxLevel : 1;
    }
}
