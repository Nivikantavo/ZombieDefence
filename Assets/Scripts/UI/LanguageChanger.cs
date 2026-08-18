using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] _flags;
    [SerializeField] private Button _currentLanguage;
    [SerializeField] private LeanLocalization _localizator;

    private int _flagIndex;

    private void OnEnable()
    {
        LeanLocalization.OnLocalizationChanged += ApplyFlagFromCurrentLanguage;
        ApplyFlagFromCurrentLanguage();
    }

    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= ApplyFlagFromCurrentLanguage;
    }

    public void SetNextLanguage()
    {
        string nextLanguage;
        if (_localizator.CurrentLanguage == GameLanguage.English)
            nextLanguage = GameLanguage.Russian;
        else if (_localizator.CurrentLanguage == GameLanguage.Russian)
            nextLanguage = GameLanguage.Turkish;
        else
            nextLanguage = GameLanguage.English;

        GameLanguage.SetManual(nextLanguage);
        ApplyFlagFromCurrentLanguage();
    }

    private void ApplyFlagFromCurrentLanguage()
    {
        if (_localizator == null || _currentLanguage == null || _flags == null || _flags.Length < 3)
            return;

        if (_localizator.CurrentLanguage == GameLanguage.English)
            _flagIndex = 0;
        else if (_localizator.CurrentLanguage == GameLanguage.Russian)
            _flagIndex = 1;
        else if (_localizator.CurrentLanguage == GameLanguage.Turkish)
            _flagIndex = 2;

        _currentLanguage.image.sprite = _flags[_flagIndex];
    }
}
