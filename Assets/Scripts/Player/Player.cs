using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
    [SerializeField] private Force _pushForce;
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private Inventory _inventory;

    private PlayerData _playerData;

    public void TryUsePushForce()
    {
        _pushForce.UseForce();
    }

    public void Initialize()
    {

    }
}
