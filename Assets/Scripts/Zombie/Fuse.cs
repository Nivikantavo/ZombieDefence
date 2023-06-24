using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : MonoBehaviour
{
    [SerializeField] private Bomb _bomb;

    public void BlowYoureselfUp()
    {
        _bomb.BlowUp();
    }
}
