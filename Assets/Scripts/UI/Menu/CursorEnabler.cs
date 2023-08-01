using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorEnabler : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
