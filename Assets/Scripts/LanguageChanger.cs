using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] _flags;
    [SerializeField] private Button _currentLanguage;
    [SerializeField] private LeanLocalization _localizator;

    private int _flagIndex;
    
    private void Awake()
    {
        Debug.Log(_flagIndex);
        if (PlayerPrefs.GetString("Lang", "No Lang") == "No Lang")
        {
            PlayerPrefs.SetString("Lang", "English");
        }

        _localizator.SetCurrentLanguage(PlayerPrefs.GetString("Lang"));
       
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
        Debug.Log(_flagIndex);
        _currentLanguage.image.sprite = _flags[_flagIndex];
    }

    public void SetNextLanguage()
    {
        Debug.Log(1);
        if (_localizator.CurrentLanguage == "English")
        {
            _localizator.SetCurrentLanguage("Russian");
            PlayerPrefs.SetString("Lang", "Russian");
            _flagIndex = 1;
        }
        else if (_localizator.CurrentLanguage == "Russian")
        {
            _localizator.SetCurrentLanguage("Turkish");
            PlayerPrefs.SetString("Lang", "Turkish");
            _flagIndex = 2;
        }
        else if (_localizator.CurrentLanguage == "Turkish")
        {
            _localizator.SetCurrentLanguage("English");
            PlayerPrefs.SetString("Lang", "English");
            _flagIndex = 0;
        }
        Debug.Log(2);
        Debug.Log(_flagIndex);
        Debug.Log(_localizator.CurrentLanguage);
        Debug.Log(PlayerPrefs.GetString("Lang"));

        _currentLanguage.image.sprite = _flags[_flagIndex];
        Debug.Log(3);
    }
}
