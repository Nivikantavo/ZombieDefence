using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    [SerializeField] private Image[] _images;
    [SerializeField] private TMP_Text[] _texts;
    [SerializeField] private float _duration;

    private float _fadeDelay = 0.1f;
    private int _maxColorValue = 255;

    private Color _startImageColor;
    private Color _startTextColor;
    private Coroutine _coroutine;


    private void Awake()
    {
        _startImageColor = _images.Length > 0 ? _images[0].color : Color.white;
        _startTextColor = _texts.Length > 0 ? _texts[0].color : Color.white;
    }

    public void Show()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        SetStartColor();
        yield return new WaitForSeconds(_duration);

        int step = 5;
        Color imageColor = _startImageColor;
        Color textColor = _startTextColor;
        WaitForSeconds delay = new WaitForSeconds(_fadeDelay);

        for (int i = 0; i < _maxColorValue; i += step)
        {
            imageColor.a -= step;
            textColor.a -= step;
            foreach (var image in _images)
            {
                image.color = imageColor;
            }
            foreach (var text in _texts)
            {
                text.color = textColor;
            }
            yield return delay;
        }

        gameObject.SetActive(false);
    }

    private void SetStartColor()
    {
        foreach (var image in _images)
        {
            Color color = image.color;
            color.a = _maxColorValue;
            image.color = color;
        }

        foreach (var text in _texts)
        {
            Color color = text.color;
            color.a = _maxColorValue;
            text.color = color;
        }
    }
}
