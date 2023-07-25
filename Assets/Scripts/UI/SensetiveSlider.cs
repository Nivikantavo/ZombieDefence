using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.UI;

public class SensetiveSlider : MonoBehaviour
{
    [SerializeField] private Character _player;
    [SerializeField] private float _minSensetive;
    [SerializeField] private float _maxSensetive;

    private Slider _sensetiveSlider;

    private void Awake()
    {
        _sensetiveSlider = GetComponent<Slider>();
        _sensetiveSlider.minValue = _minSensetive;
        _sensetiveSlider.maxValue = _maxSensetive;
    }

    private void OnEnable()
    {
        _sensetiveSlider.onValueChanged.AddListener(OnSensetiveChanged);
    }

    private void OnDisable()
    {
        _sensetiveSlider.onValueChanged.RemoveListener(OnSensetiveChanged);
        SaveSystem.Instance.SetSensetiveValue(_sensetiveSlider.value);
    }

    private void OnSensetiveChanged(float newSensetive)
    {
        _player.ChangeSensetive(newSensetive);
    }
}
