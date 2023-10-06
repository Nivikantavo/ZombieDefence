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

    [SerializeField] private Shop _shop;
    [SerializeField] private TMP_Text _granagesCountText;
    [SerializeField] private TMP_Text _truckHealthText;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _normalColor;

    [SerializeField] private Image _pushForce;
    [SerializeField] private Image _slamForce;

    private int _granadesCount;
    private int _truckHealth;
    private bool _hasPushForce;
    private bool _hasSlamForce;

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
        if(SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DataUpdated += LoadData;
        }
    }

    private void OnDisable()
    {
        _shop.ItemBought -= LoadData;
        SaveSystem.Instance.DataUpdated -= LoadData;
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

        Renderer();
    }

    private void Renderer()
    {
        _granagesCountText.text = _granadesCount.ToString();
        _truckHealthText.text = _truckHealth.ToString();

        _pushForce.color = _hasPushForce ? _normalColor : _disabledColor;
        _slamForce.color = _hasSlamForce ? _normalColor : _disabledColor;
    }
}
