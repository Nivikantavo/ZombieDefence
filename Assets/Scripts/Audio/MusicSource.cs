using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    private const string MusicClipName = "LevelMusic";

    [SerializeField] private AudioYB _audioYB;

    private void Start()
    {
        _audioYB.loop = true;
        _audioYB.Play(MusicClipName);
    }
}
