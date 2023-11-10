using Plugins.Audio.Core;
using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    [SerializeField] private SourceAudio _source;

    private void Start()
    {
        _source.Play();
    }
}
