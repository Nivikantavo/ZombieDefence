using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelEndZone : MonoBehaviour
{
    public event UnityAction PlayerInLevelEndZone;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.enabled = false;
            PlayerInLevelEndZone?.Invoke();
        }
    }
}
