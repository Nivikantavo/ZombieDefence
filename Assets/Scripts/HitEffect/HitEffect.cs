using UnityEngine;

public abstract class HitEffect : MonoBehaviour
{
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] private float _maxIntensity;
    [SerializeField] private float _duration;
    [SerializeField] private Color _color;

    protected float CurrentIntensity = 1f;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private float _effectTimer;
    private bool _isPlaying;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected virtual void Update()
    {
        if (!_isPlaying)
            return;

        if (_effectTimer <= 0f)
        {
            CurrentIntensity = 1f;
            ApplyEffects(CurrentIntensity);
            _isPlaying = false;
            return;
        }

        float lerp = Mathf.Clamp01(_effectTimer / _duration);
        CurrentIntensity = (lerp * _maxIntensity) + 1f;
        _effectTimer -= Time.deltaTime;
        ApplyEffects(CurrentIntensity);
    }

    protected void OnHitEffect()
    {
        _effectTimer = _duration;
        _isPlaying = true;
    }

    protected virtual void ApplyEffects(float intensity)
    {
        SetSkinEffect(intensity);
        SetAttachEffect(intensity);
    }

    protected void SetSkinEffect(float intensity)
    {
        if (_skinnedMeshRenderer == null)
            return;

        _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(ColorId, Color.white * intensity);
        _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    protected void SetAttachEffect(float intensity)
    {
        if (_meshRenderers == null)
            return;

        Color color = _color * intensity;
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            MeshRenderer mesh = _meshRenderers[i];
            mesh.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorId, color);
            mesh.SetPropertyBlock(_propertyBlock);
        }
    }
}
