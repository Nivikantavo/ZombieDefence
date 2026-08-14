using Lean.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_WEBGL
using Playgama;
#endif

public class LanguageChanger : MonoBehaviour
{
    private const string En = "English";
    private const string Ru = "Russian";
    private const string Tr = "Turkish";

    [SerializeField] private Sprite[] _flags;
    [SerializeField] private Button _currentLanguage;
    [SerializeField] private LeanLocalization _localizator;

    private int _flagIndex;

    private void Awake()
    {
        ApplyFlagFromCurrentLanguage();
    }

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#else
        while (Bridge.instance == null)
            yield return null;

        ApplyPlatformLanguage(Bridge.platform.language);
#endif
    }

    public void SetNextLanguage()
    {
        if (_localizator.CurrentLanguage == En)
        {
            _localizator.SetCurrentLanguage(Ru);
            _flagIndex = 1;
        }
        else if (_localizator.CurrentLanguage == Ru)
        {
            _localizator.SetCurrentLanguage(Tr);
            _flagIndex = 2;
        }
        else
        {
            _localizator.SetCurrentLanguage(En);
            _flagIndex = 0;
        }

        _currentLanguage.image.sprite = _flags[_flagIndex];
    }

    private void ApplyPlatformLanguage(string language)
    {
        string code = language == null ? string.Empty : language.ToLowerInvariant();

        if (IsRussianFamily(code))
        {
            _localizator.SetCurrentLanguage(Ru);
            _flagIndex = 1;
        }
        else if (code == "tr" || code.StartsWith("tr-"))
        {
            _localizator.SetCurrentLanguage(Tr);
            _flagIndex = 2;
        }
        else
        {
            _localizator.SetCurrentLanguage(En);
            _flagIndex = 0;
        }

        _currentLanguage.image.sprite = _flags[_flagIndex];
    }

    private void ApplyFlagFromCurrentLanguage()
    {
        if (_localizator.CurrentLanguage == En)
            _flagIndex = 0;
        else if (_localizator.CurrentLanguage == Ru)
            _flagIndex = 1;
        else if (_localizator.CurrentLanguage == Tr)
            _flagIndex = 2;

        _currentLanguage.image.sprite = _flags[_flagIndex];
    }

    private static bool IsRussianFamily(string code)
    {
        return code == "ru" || code.StartsWith("ru-")
            || code == "be" || code.StartsWith("be-")
            || code == "kk" || code.StartsWith("kk-")
            || code == "uk" || code.StartsWith("uk-")
            || code == "uz" || code.StartsWith("uz-");
    }
}
