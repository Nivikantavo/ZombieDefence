using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DieState : State
{
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] private Zombie _zombie;
    [SerializeField] private GameObject _minimapSign;
    [SerializeField] private float _fadeDelay;

    private SkinnedMeshRenderer _skinRenderer;
    private MeshRenderer[] _detailsRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private float _fadeStep = 0.01f;
    private WaitForSeconds _fadeDelayWait;
    private WaitForSeconds _fadeStepWait;

    public event UnityAction ZombieDied;
    public event UnityAction<Vector3> NeedSpawnCoin;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _detailsRenderers = GetComponentsInChildren<MeshRenderer>();
        _skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _fadeDelayWait = new WaitForSeconds(_fadeDelay);
        _fadeStepWait = new WaitForSeconds(_fadeStep);
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
        yield return _fadeDelayWait;

        Color skinFade = _skinRenderer.sharedMaterial.color;
        float startAlpha = skinFade.a;
        int steps = Mathf.Max(1, Mathf.FloorToInt(startAlpha));

        for (int i = 0; i < steps; i++)
        {
            skinFade.a -= 1f;
            ApplyFadeColor(skinFade);
            yield return _fadeStepWait;
        }

        enabled = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ClearFade();
    }

    private void ClearFade()
    {
        if (_skinRenderer != null)
            _skinRenderer.SetPropertyBlock(null);

        if (_detailsRenderers == null)
            return;

        for (int i = 0; i < _detailsRenderers.Length; i++)
        {
            MeshRenderer detail = _detailsRenderers[i];
            if (detail != null)
                detail.SetPropertyBlock(null);
        }
    }

    private void ApplyFadeColor(Color color)
    {
        if (_skinRenderer != null)
        {
            _skinRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorId, color);
            _skinRenderer.SetPropertyBlock(_propertyBlock);
        }

        if (_detailsRenderers == null)
            return;

        for (int i = 0; i < _detailsRenderers.Length; i++)
        {
            MeshRenderer detail = _detailsRenderers[i];
            detail.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorId, color);
            detail.SetPropertyBlock(_propertyBlock);
        }
    }
}
