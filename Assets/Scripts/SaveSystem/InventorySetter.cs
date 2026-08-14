using InfimaGames.LowPolyShooterPack;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySetter : MonoBehaviour
{
    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private Character _character;

    private List<Weapon> _weaponList;
    private int _startWeaponIndex = 0;

    private void Awake()
    {
        if (_playerInventory == null || _character == null)
            return;

        _weaponList = _playerInventory.GetComponentsInChildren<Weapon>(true).ToList();
        TakeExtraWeapons();
        UpgradeWeapons();
        _playerInventory.Init(_startWeaponIndex);
        _character.RefreshWeaponSetup();
    }

    public void TakeExtraWeapons()
    {
        PlayerData playerData = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;
        string[] equipedWeapons = playerData != null ? playerData.Weapons : null;

        if (equipedWeapons == null || equipedWeapons.Length == 0)
        {
            equipedWeapons = new[] { "SMG 01" };
        }

        bool inList = false;
        foreach (var weapon in _weaponList)
        {
            for (int i = 0; i < equipedWeapons.Length; i++)
            {
                if (equipedWeapons[i] == weapon.WeaponName)
                {
                    inList = true;
                }
            }
            if(inList == false)
            {
                weapon.transform.parent = transform;
                weapon.gameObject.SetActive(false);
            }
            inList = false;
        }
    }

    private void UpgradeWeapons()
    {
        PlayerData playerData = SaveSystem.Instance != null ? SaveSystem.Instance.GetData() : null;
        if (playerData == null)
        {
            return;
        }

        foreach (var weapon in _weaponList)
        {
            int level = WeaponUpgradeLevels.GetLevelFromUpgradeCount(
                playerData.GetWeaponUpgradeCount(weapon.WeaponName));

            weapon.ApplyLevel(level);

            WeaponAttachmentManager attachments = weapon.GetComponent<WeaponAttachmentManager>();
            if (attachments != null)
            {
                attachments.ApplyLevel(level);
            }
        }
    }

    public void RemoveWeaponsSpread()
    {
        if (_weaponList == null)
            return;

        foreach (var weapon in _weaponList)
        {
            weapon.SetMobileSpread();
        }
    }
}
