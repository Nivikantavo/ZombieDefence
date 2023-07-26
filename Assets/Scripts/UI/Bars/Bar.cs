using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] protected Slider Slider;

    protected float _setValueDelay = 0.01f;
    protected float _step = 0.05f;
    protected Coroutine _setValueCoroutine;

    protected void OnTrackingValueChanged(float newValue)
    {
        if (_setValueCoroutine != null)
        {
            StopCoroutine(_setValueCoroutine);
        }
        _setValueCoroutine = StartCoroutine(SetBarValue(newValue));
    }

    protected virtual IEnumerator SetBarValue(float newValue)
    {
        WaitForSeconds delay = new WaitForSeconds(_setValueDelay);

        float currentValue = Slider.value;
        float percentage = 0;
        while (Slider.value != newValue)
        {
            currentValue = Mathf.Lerp(Slider.value, newValue, percentage);
            Slider.value = currentValue;
            percentage += _step;
            yield return delay;
        }
    }
}
