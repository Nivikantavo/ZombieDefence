using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DieState : State
{
    [SerializeField] private Zombie _zombie;
    [SerializeField] private GameObject _minimapSign;
    [SerializeField] private float _fadeDelay;

    private SkinnedMeshRenderer _skinRenderer;
    private MeshRenderer[] _detailsRenderers;
    private float _fadeStep = 0.01f;

    public event UnityAction ZombieDied;
    public event UnityAction<Vector3> NeedSpawnCoin;

    private void Awake()
    {
        _detailsRenderers = GetComponentsInChildren<MeshRenderer>();
        _skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnEnable()
    {
        ZombieDied?.Invoke();
        NeedSpawnCoin?.Invoke(transform.position);
        _minimapSign.SetActive(false);
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        WaitForSeconds delay = new WaitForSeconds(_fadeDelay);
        yield return delay;
        delay = new WaitForSeconds(_fadeStep);
        Color skinFade = _skinRenderer.material.color;
        float startAlpha = _skinRenderer.material.color.a;

        for (int i = 0; i < startAlpha; i++)
        {
            skinFade.a -= 1;
            _skinRenderer.material.color = skinFade;

            if (_detailsRenderers != null)
            {
                foreach (var detail in _detailsRenderers)
                {
                    detail.material.color = skinFade;
                }
            }
            yield return delay;
        }
        gameObject.SetActive(false);
    }
}
