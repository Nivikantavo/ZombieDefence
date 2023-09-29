using Agava.WebUtility;
using Agava.YandexGames;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;
    [SerializeField] private Character _character;
    [SerializeField] private InventorySetter _inventorySetter;

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();

        bool isMobile;

        if (Device.IsMobile)
        {
            isMobile = true;
            _inventorySetter.RemoveWeaponsSpread();
        }
        else
        {
            isMobile = false; 
        }

        _mobileUI.SetActive(true);
        _character.SetMobileInput(isMobile);
    }
}
