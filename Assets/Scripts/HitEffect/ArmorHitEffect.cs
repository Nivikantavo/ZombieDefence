using UnityEngine;

public class ArmorHitEffect : HitEffect
{
    [SerializeField] private ArmorElement _armor;

    private void OnEnable()
    {
        _armor.Hit += OnHitEffect;
    }

    private void OnDisable()
    {
        _armor.Hit -= OnHitEffect;
    }

    protected override void Update()
    {
        base.Update();
        SetAttachEffect(CurrentIntensity);
    }
}
