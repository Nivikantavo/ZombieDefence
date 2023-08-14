using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ForceUIView : MonoBehaviour
{
    public string ForceName => _force.Name;

    [SerializeField] private Force _force;
    [SerializeField] private Slider _cooldownSlider;
    [SerializeField] private Image _cooldownImage;
    private float _viewDelay = 0.01f;

    private void Start()
    {
        _cooldownImage.fillAmount = 0;
        _cooldownSlider.value = 1;
    }

    private void OnEnable()
    {
        _force.ForceUsed += ShowCooldown;
    }

    private void OnDisable()
    {
        _force.ForceUsed -= ShowCooldown;
    }

    private void ShowCooldown()
    {
        StartCoroutine(CooldownView());
    }

    private IEnumerator CooldownView()
    {
        WaitForSeconds delay = new WaitForSeconds(_viewDelay);

        while(_force.CooldownRemains < _force.CooldownDuration)
        {
            _cooldownImage.fillAmount =  Mathf.InverseLerp(0, _force.CooldownDuration, _force.CooldownRemains);
            _cooldownSlider.value = Mathf.InverseLerp(_force.CooldownDuration, 0, _force.CooldownRemains);
            yield return delay;
        }
    }
}
