using InfimaGames.LowPolyShooterPack;
using System.Collections;
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
        _weaponList = _playerInventory.GetComponentsInChildren<Weapon>(true).ToList();
        TakeExtraWeapons();
        UpgradeWeapons();
        _playerInventory.Init(_startWeaponIndex);
        _character.RefreshWeaponSetup();
    }

    public void TakeExtraWeapons()
    {
        string[] equipedWeapons = SaveSystem.Instance.GetData().Weapons;
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
        string[] upgradeWeapons = SaveSystem.Instance.GetData().UpgradeWeapons;

        foreach (var weapon in _weaponList)
        {
            WeaponAttachmentManager attachments = weapon.GetComponent<WeaponAttachmentManager>();

            if (upgradeWeapons.Contains(weapon.WeaponName))
            {
                attachments.SetUpgradeAttachments();
                weapon.SetUpgrades();
            }
            else
            {
                attachments.SetDefultOrRandomAttachments();
            }
        }
    }

    public void RemoveWeaponsSpread()
    {
        foreach (var weapon in _weaponList)
        {
            weapon.SetMobileSpread();
        }
    }
}
