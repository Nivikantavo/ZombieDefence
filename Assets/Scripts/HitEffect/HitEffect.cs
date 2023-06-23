using UnityEngine;

public abstract class HitEffect : MonoBehaviour
{
    [SerializeField] private float _maxIntensity;
    [SerializeField] private float _duration;
    [SerializeField] private Color _color;

    protected float CurrentIntensity;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private MeshRenderer[] _meshRenderers;
    private float _effectTimer;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected virtual void Update()
    {
        float lerp = Mathf.Clamp01(_effectTimer / _duration);
        CurrentIntensity = (lerp * _maxIntensity) + 1;
        _effectTimer -= Time.deltaTime;
    }

    protected void OnHitEffect()
    {
        _effectTimer = _duration;
    }

    protected void SetSkinEffect(float intensity)
    {
        _skinnedMeshRenderer.material.color = Color.white * intensity;
    }
    
    protected void SetAttachEffect(float intensity)
    {
        foreach (var mesh in _meshRenderers)
        {
            foreach(var material in mesh.materials)
            {
                material.color = _color * intensity;
            }
        }
    }
}
