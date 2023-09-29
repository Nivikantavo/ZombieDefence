using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyViewer : MonoBehaviour
{
    [SerializeField] private MoneyCollecter _moneyCollecter;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private float _settingTime;

    private float _currentValue;
    private float _addingMultiplayer = 10;
    private Coroutine _settingValue;

    private void OnEnable()
    {
        OnMoneyLoaded(_moneyCollecter.Money);
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
        float step = 0.1f;
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(step);

        for (float i = 0; i < 1; i += step)
        {
            _currentValue = Mathf.Round(Mathf.Lerp(_currentValue, newValue, i));
            _moneyText.text = _currentValue.ToString();
            yield return delay;
        }
    }

    public void OnMoneyLoaded(int money)
    {
        _currentValue = money;
        _moneyText.text = _currentValue.ToString();
    }
}
