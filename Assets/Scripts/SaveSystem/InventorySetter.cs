using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySetter : MonoBehaviour
{
    [SerializeField] private Inventory _playerInventory;

    private List<Weapon> _weaponList;
    private int _startWeapon = 0;

    private void Awake()
    {
        _weaponList = _playerInventory.GetComponentsInChildren<Weapon>(true).ToList();
        foreach (Weapon weapon in _weaponList)
        {
            weapon.transform.parent = transform;
        }

        //SetWeapons();
    }

    public void SetWeapons()
    {
        string[] equipedWeapons = SaveSystem.Instance.GetData().Weapons;
        List<Weapon> removeWeapons = new List<Weapon>();

        for (int i = 0; i < equipedWeapons.Length; i++)
        {
            foreach (var weapon in _weaponList)
            {
                if (equipedWeapons[i] == weapon.WeaponName)
                {
                    weapon.transform.parent = _playerInventory.transform;
                }
                weapon.gameObject.SetActive(false);
            }
        }

        //_playerInventory.Init(_startWeapon);
    }
}
