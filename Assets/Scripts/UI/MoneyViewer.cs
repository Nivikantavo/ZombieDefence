using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyViewer : MonoBehaviour
{
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private float _settingTime;

    private int _currentValue;
    private Coroutine _settingValue;

    private void OnEnable()
    {
        _moneyCollecter.MoneyCountChanged += OnCoinsCountChanged;
        _moneyCollecter.MoneyLoaded += OnMoneyLoaded;
    }

    private void OnDisable()
    {
        _moneyCollecter.MoneyCountChanged -= OnCoinsCountChanged;
        _moneyCollecter.MoneyLoaded -= OnMoneyLoaded;
    }

    private void OnCoinsCountChanged(int newValue)
    {
        if(_settingValue != null)
        {
            StopCoroutine(_settingValue);
        }

        _settingValue = StartCoroutine(SetValueSmoothly(newValue));
    }

    private IEnumerator SetValueSmoothly(int newValue)
    {
        float difference = newValue - _currentValue;
        WaitForSeconds delaye = new WaitForSeconds(_settingTime / difference);

        for (int i = 0; i < difference; i++)
        {
            _currentValue++;
            _moneyText.text = _currentValue.ToString();
            yield return delaye;
        }
    }

    public void OnMoneyLoaded(int money)
    {
        _currentValue = money;
        _moneyText.text = _currentValue.ToString();
    }
}
