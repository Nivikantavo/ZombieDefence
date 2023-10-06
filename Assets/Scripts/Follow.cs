using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private void FixedUpdate()
    {
        transform.position = _player.position;
    }
}
