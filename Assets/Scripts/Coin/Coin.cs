using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour
{
    public int Count => _count;

    [SerializeField] private int _count;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Sleep();
    }

    public void Sleep()
    {
        _rigidbody.isKinematic = true;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.Sleep();
    }

    public void WakeUp()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
