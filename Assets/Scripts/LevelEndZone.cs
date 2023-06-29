using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEndZone : MonoBehaviour
{
    [SerializeField] private GameObject _endLevelPanel;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.enabled = false;
            _endLevelPanel.SetActive(true);
        }
    }
}
