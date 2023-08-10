using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    private const string PushForce = "ForwardDash";
    private const string SlamForce = "JumpDash";
    private const string ShieldForce = "Shield";

    [SerializeField] private Shop _shop;
    [SerializeField] private TMP_Text _granagesCountText;
    [SerializeField] private TMP_Text _truckHealthText;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _normalColor;

    [SerializeField] private Image _pushForce;
    [SerializeField] private Image _slamForce;
    [SerializeField] private Image _shieldForce;

    private int _granadesCount;
    private int _truckHealth;
    private bool _hasPushForce;
    private bool _hasSlamForce;
    private bool _hasShieldForce;

    private IEnumerator Start()
    {
        while (SaveSystem.Instance.DataLoaded == false)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }
        LoadData();
    }

    private void OnEnable()
    {
        _shop.ItemBought += LoadData;
    }

    private void OnDisable()
    {
        _shop.ItemBought -= LoadData;
    }

    private void LoadData()
    {
        if (SaveSystem.Instance.DataLoaded)
        {
            PlayerData playerData = SaveSystem.Instance.GetData();
            SetData(playerData);
        }
    }

    private void SetData(PlayerData playerData)
    {
        _granadesCount = playerData.GranadesCount;
        _truckHealth = playerData.TruckHealth;

        if (playerData.Forces.Contains(PushForce))
        {
            _hasPushForce = true;
        }
        if (playerData.Forces.Contains(SlamForce))
        {
            _hasSlamForce = true;
        }
        if (playerData.Forces.Contains(ShieldForce))
        {
            _hasShieldForce = true;
        }

        Renderer();
    }

    private void Renderer()
    {
        _granagesCountText.text = _granadesCount.ToString();
        _truckHealthText.text = _truckHealth.ToString();

        _pushForce.color = _hasPushForce ? _normalColor : _disabledColor;
        _slamForce.color = _hasSlamForce ? _normalColor : _disabledColor;
        _shieldForce.color = _hasShieldForce ? _normalColor : _disabledColor;
    }
}
