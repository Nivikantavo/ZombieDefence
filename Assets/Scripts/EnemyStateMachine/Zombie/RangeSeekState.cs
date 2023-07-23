using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSeekState : SeekState
{
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _throwingAngle;
    [SerializeField] private float _yOffset;

    private MissilePool _missilePool;
    private Missile _missile;
    private float _gravityForce = Physics.gravity.y;

    private void OnEnable()
    {
        Movment.SetStoppingDistance(AttackDistance);
    }

    protected override void Update()
    {
        base.Update();
        if(_missile != null && _missile.gameObject.activeSelf)
        {
            _missile.transform.position = _shotPoint.position;
        }
    }

    public void ThrowMissile()
    {
        Vector3 fromTo = Zombie.Target.transform.position - transform.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0, fromTo.z);

        _shotPoint.transform.rotation = Quaternion.LookRotation(new Vector3(fromTo.x, fromTo.y + _yOffset, fromTo.z));

        float XDistance = fromToXZ.magnitude;
        float YDistance = fromTo.y;

        float angelInRadians = _throwingAngle * Mathf.PI / 180;

        float throwingSpeedSquare = (_gravityForce * XDistance * XDistance) / (2 * (YDistance - Mathf.Tan(angelInRadians) * XDistance) * Mathf.Pow(Mathf.Cos(angelInRadians), 2));
        float throwingSpeed = Mathf.Sqrt(Mathf.Abs(throwingSpeedSquare)) * 1.3f;
        
        _missile.transform.parent = _missilePool.Container.transform;
        _missile.GetComponent<Rigidbody>().velocity = _shotPoint.forward * throwingSpeed * _yOffset;
        _missile.Throw();
        _missile = null;
    }

    public void TryGetObjectFromPool()
    {
        if (_missilePool.TryGetObject(out GameObject missile))
        {
            missile.GetComponent<Rigidbody>().velocity = Vector3.zero;
            missile.transform.position = _shotPoint.position;
            _missile = missile.GetComponent<Missile>();
            _missile.gameObject.SetActive(true);
        }
    }

    public void SetMissilePool(MissilePool missilePool)
    {
        _missilePool = missilePool;
    }
}
