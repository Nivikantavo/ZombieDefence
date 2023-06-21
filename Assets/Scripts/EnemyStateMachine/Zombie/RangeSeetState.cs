using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSeetState : SeekState
{
    [SerializeField] private GameObject _missile;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _throwingAngle;
    [SerializeField] private float _yOffset;

    private float _gravityForce = Physics.gravity.y;

    private void OnEnable()
    {
        Movment.SetStoppingDistance(AttackDistance);
    }

    protected override void Attack()
    {
        base.Attack();
        _shotPoint.localEulerAngles = new Vector3(-_throwingAngle, 0, 0);

    }

    public void ThrowMissile()
    {
        Vector3 fromTo = Zombie.Target.transform.position - transform.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, _yOffset, fromTo.z);

        _shotPoint.transform.rotation = Quaternion.LookRotation(fromToXZ, Vector3.up);


        float XDistance = fromToXZ.magnitude;
        float YDistance = fromTo.y;

        float angelInRadians = _throwingAngle * Mathf.PI / 180;

        float throwingSpeedSquare = (_gravityForce * XDistance * XDistance) / (2 * (YDistance - Mathf.Tan(angelInRadians) * XDistance) * Mathf.Pow(Mathf.Cos(angelInRadians), 2));
        float throwingSpeed = Mathf.Sqrt(Mathf.Abs(throwingSpeedSquare));

        GameObject missile = Instantiate(_missile, _shotPoint.position, Quaternion.identity);
        missile.GetComponent<Rigidbody>().velocity = _shotPoint.forward * throwingSpeed;
    }
}
