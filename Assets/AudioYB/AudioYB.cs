//2.2
using System.Collections;

using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioYB : MonoBehaviour
{
    private bool playLoop;
    private AudioSource _source;
    private bool load;
    private bool play;
    public bool _pausa;
    private bool _focus;

    private float _pausedTime = 0f;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_source.loop)
        { 
            _source.loop = false; 
            loop = true; 
        }
        Loop(loop);      
    }
    void PlayAfter()
    {
        play = false;
        playLoop = true;
        ZeroTime();
        _source.Play();
    }
    public IEnumerator EndFile(string name)
    {
        yield return new WaitForSeconds(0.1f);
        if (_source.time > 0f)
        {
            SourseTime();
            ZeroTime();
        }
        
        yield return new WaitForSeconds(0.02f);

        Clip clip = AudioStreamCash.Find(name);
        if (clip == null) Debug.LogError($"Не найден клип {name}");
        load = false;
        play = true;
        StartCoroutine(clip.GetFile(LoadAfter));
    }
    public void Play(string name)
    {
        StartCoroutine(EndFile(name));
    }
    void LoadAfter(AudioClip clip)
    {
        _source.clip = clip;
        load = true;
        if (play) PlayAfter();
    }
    private void ZeroTime()
    {
        _source.time = 0f;
        _pausedTime = 0f;
    }
    public void Play()
    {
        if (load) 
        {
            _source.Play();
            playLoop = true;
        }
        else play = true;
    }


    public void PlayOneShot(string clipName, float volumeScale = 1f)
    {
        var clip = AudioStreamCash.Find(clipName);
        if (clip == null)
        {
            Debug.LogError("Нет такого клипа! Проверь название " + "Name:" + " " + name);
            Debug.Break();
            return;
        }
        StartCoroutine(clip.GetFile(delegate (AudioClip audioClip) { LoadShotAfter(audioClip, volumeScale); }));
    }
    private void LoadShotAfter(AudioClip clip, float volumeScale)
    {
        _source.PlayOneShot(clip, volumeScale);
    }
    private void Loop(bool enable)
    {
        if (_source.time == 0f && enable && playLoop  && !_pausa && _focus) { ZeroTime(); _source.Play(); }
    }
    public void Pause()
    {
        _pausa = true;
        if(_source.time !=0) _pausedTime = _source.time;
         SourseTime();
    }

    public void UnPause()
    {
        if (_pausedTime != 0f)
        {
            _pausa = false;
            _source.time = _pausedTime;
            _pausedTime = 0;
            _source.Play();
        }
        else
        {
            _pausa = false;
            _source.Play();
        }
       
    }
    public void Stop()
    {
        playLoop = false;
        SourseTime();
    }

    private void SourseTime()
    {
       if(_source.clip != null) _source.time = _source.clip.length - 0.01f;
    }
    public bool isPlaying { get => _source.isPlaying; }
    public bool loop { get; set; }
    public float volume { get => _source.volume; set => _source.volume = value; }
    public bool mute { get => _source.mute; set => _source.mute = value; }
    public float time { get => _source.time; set => _source.time = value; }
    public bool Enabled { get => _source.enabled; set => _source.enabled = value; }
    public string clip { get => _source.clip.name; set => _source.clip = AudioStreamCash.Find(value).Cash; }
    public int timeSamples { get => _source.timeSamples; set => _source.timeSamples = value; }
    public bool Focus { get => _focus;}

    public float ClipLength()
    {
        float length;
        if (_source.clip != null)
        {
            length = _source.clip.length;
        }
        else length = 0f;
        return length;
    }
    private void OnApplicationFocus(bool focus)
    {
        _focus = focus;
    }

}

