using Agava.YandexGames;
using Lean.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        if (_localizator.CurrentLanguage == "English") 
        {
            _flagIndex = 0;
        }
        else if (_localizator.CurrentLanguage == "Russian")
        {
            _flagIndex = 1;
        }
        else if (_localizator.CurrentLanguage == "Turkish")
        {
            _flagIndex = 2;
        }
        _currentLanguage.image.sprite = _flags[_flagIndex];
    }

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();
        string language = YandexGamesSdk.Environment.i18n.lang;

        if (language == "ru")
        {
            _localizator.SetCurrentLanguage(Ru);
            _flagIndex = 1;
        }
        else if (language == "tr")
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

    public void SetNextLanguage()
    {
        if (_localizator.CurrentLanguage == "English")
        {
            _localizator.SetCurrentLanguage("Russian");
            _flagIndex = 1;
        }
        else if (_localizator.CurrentLanguage == "Russian")
        {
            _localizator.SetCurrentLanguage("Turkish");
            _flagIndex = 2;
        }
        else if (_localizator.CurrentLanguage == "Turkish")
        {
            _localizator.SetCurrentLanguage("English");
            _flagIndex = 0;
        }
        _currentLanguage.image.sprite = _flags[_flagIndex];
    }
}
