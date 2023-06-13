using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;
    [SerializeField] private float _effectIntensity;
    [SerializeField] private float _effectDuration;
    [SerializeField] private Color _effectColor;

    private SkinnedMeshRenderer _skinnedMeshRenderers;
    private MeshRenderer[] _meshRenderers;
    private float _effectTimer;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _skinnedMeshRenderers = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnEnable()
    {
        _zombie.Hit += OnHitEffect;
    }

    private void OnDisable()
    {
        _zombie.Hit -= OnHitEffect;
    }

    private void Update()
    {
        float lerp = Mathf.Clamp01(_effectTimer / _effectDuration);
        float intensity = (lerp * _effectIntensity) + 1;

        _skinnedMeshRenderers.material.color = Color.white * intensity;
        foreach (var mesh in _meshRenderers)
        {
            mesh.material.color = _effectColor * intensity;
        }

        _effectTimer -= Time.deltaTime;
    }

    private void OnHitEffect()
    {
        _effectTimer = _effectDuration;
    }
}
