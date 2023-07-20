using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    public override string Name => _weapon.WeaponName;
    public Weapon Weapon => _weapon;
    [SerializeField] private Weapon _weapon;
}
