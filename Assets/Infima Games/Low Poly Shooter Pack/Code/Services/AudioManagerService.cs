//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Agava.YandexGames;
using Plugins.Audio.Core;

namespace InfimaGames.LowPolyShooterPack
{

    /// <summary>
    /// Manages the spawning and playing of sounds.
    /// </summary>
    public class AudioManagerService : MonoBehaviour, IAudioManagerService
    {
        private const string Music = "Music";
        private const string Sounds = "Sounds";
        private const string AudioMixer = "AudioMixer";
        
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _soundsGroup;
        /// <summary>
        /// Contains data related to playing a OneShot audio.
        /// </summary>
        /// 

        private void Awake()
        {
            _audioMixer = Resources.Load<AudioMixer>(AudioMixer);
            _soundsGroup = _audioMixer.FindMatchingGroups(Sounds)[0];
        }

        private IEnumerator Start()
        {
#if !UNITY_WEBGL || UNITY_EDITOR

            yield break;
#endif
            yield return YandexGamesSdk.Initialize();
            SetStartSoundsSettings();
        }

        private readonly struct OneShotCoroutine
        {
            /// <summary>
            /// Audio Clip.
            /// </summary>
            public AudioClip Clip { get; }
            /// <summary>
            /// Audio Settings.
            /// </summary>
            public AudioSettings Settings { get; }
            /// <summary>
            /// Delay.
            /// </summary>
            public float Delay { get; }
            
            /// <summary>
            /// Constructor.
            /// </summary>
            public OneShotCoroutine(AudioClip clip, AudioSettings settings, float delay)
            {
                //Clip.
                Clip = clip;
                //Settings
                Settings = settings;
                //Delay.
                Delay = delay;
            }
        }
        private void SetStartSoundsSettings()
        {
            PlayerData data = SaveSystem.Instance.GetData();
            float musicVolume = data.MusicVolume;
            float soundVolume = data.SoundsVolume;

            _audioMixer.SetFloat(Music, musicVolume);
            _audioMixer.SetFloat(Sounds, soundVolume);
        }
        /// <summary>
        /// Checks if an AudioSource is valid, and playing!
        /// </summary>
        private bool IsPlayingSource(AudioSource source)
        {
            //Make sure we still have a source!
            if (source == null)
                return false;
            //Return.
            return source.isPlaying;
        }

        /// <summary>
        /// Destroys the audio source once it has finished playing.
        /// </summary>
        private IEnumerator DestroySourceWhenFinished(AudioSource source)
        {
            //Wait for the audio source to complete playing the clip.
            yield return new WaitWhile(() => IsPlayingSource(source));
            //Destroy the audio game object, since we're not using it anymore.
            //This isn't really too great for performance, but it works, for now.
            if (source != null)
                DestroyImmediate(source.gameObject);
        }

        private IEnumerator DestroySourceWhenFinished(AudioSource source, float time)
        {
            //Wait for the audio source to complete playing the clip.
            yield return new WaitForSeconds(time);
            //Destroy the audio game object, since we're not using it anymore.
            //This isn't really too great for performance, but it works, for now.
            if (source != null)
                DestroyImmediate(source.gameObject);
        }

        /// <summary>
        /// Waits for a certain amount of time before starting to play a one shot sound.
        /// </summary>
        private IEnumerator PlayOneShotAfterDelay(OneShotCoroutine value)
        {
            //Wait for the delay.
            yield return new WaitForSeconds(value.Delay);
            //Play.
            PlayOneShot_Internal(value.Clip, value.Settings);
        }
        
        /// <summary>
        /// Internal PlayOneShot. Basically does the whole function's name!
        /// </summary>
        private void PlayOneShot_Internal(AudioClip clip, AudioSettings settings)
        {
            //No need to do absolutely anything if the clip is null.
            if (clip == null)
                return;
            
            //Spawn a game object for the audio source.
            var newSourceObject = new GameObject($"Audio Source -> {clip.name}");
            //Add an audio source component to that object.
            var newAudioSource = newSourceObject.AddComponent<AudioSource>();
            var audioYB = newSourceObject.AddComponent<SourceAudio>();
            newAudioSource.outputAudioMixerGroup = _soundsGroup;
            _audioMixer.GetFloat(Sounds, out float volume);
            //newAudioSource.volume = volume;
            //Set spatial blend.
            newAudioSource.spatialBlend = settings.SpatialBlend;
            //Play the clip!
            //newAudioSource.PlayOneShot(clip, volume);
            audioYB.Play(clip.name);
            
            //Start a coroutine that will destroy the whole object once it is done!
            if(settings.AutomaticCleanup)
                StartCoroutine(DestroySourceWhenFinished(newAudioSource, clip.length));
        }

        #region Audio Manager Service Interface

        public void PlayOneShot(AudioClip clip, AudioSettings settings = default)
        {
            //Play.
            PlayOneShot_Internal(clip, settings);
        }

        public void PlayOneShotDelayed(AudioClip clip, AudioSettings settings = default, float delay = 1.0f)
        {
            //Play.
            StartCoroutine(nameof(PlayOneShotAfterDelay), new OneShotCoroutine(clip, settings, delay));
        }

        #endregion
    }
}