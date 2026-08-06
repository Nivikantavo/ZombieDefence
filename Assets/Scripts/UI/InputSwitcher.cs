using InfimaGames.LowPolyShooterPack;
using System.Collections;
using UnityEngine;

public class InputSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _mobileUI;
    [SerializeField] private Character _character;
    [SerializeField] private InventorySetter _inventorySetter;

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#else
        bool isMobile = PlaygamaAds.IsMobileDevice();

        if (isMobile)
        {
            _inventorySetter.RemoveWeaponsSpread();
        }

        _mobileUI.SetActive(isMobile);
        _character.SetMobileInput(isMobile);
        yield break;
#endif
    }
}
