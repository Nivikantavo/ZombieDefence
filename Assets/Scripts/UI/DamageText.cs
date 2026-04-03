using System.Collections;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMesh _text;
    [SerializeField] private float _disableTime;

    public void Initialize(float damage)
    {
        _text.text = ((int)damage).ToString();
    }

    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(_disableTime);
        gameObject.SetActive(false);
    }
}
