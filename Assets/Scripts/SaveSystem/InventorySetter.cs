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
        _playerInventory.Init(_startWeaponIndex);
        _character.RefreshWeaponSetup();
    }
}
