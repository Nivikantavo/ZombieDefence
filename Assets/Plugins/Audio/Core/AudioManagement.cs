using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Plugins.Audio.Core
{
    public class AudioManagement : MonoBehaviour
    {
        public static AudioManagement Instance { get; private set; }

        private Dictionary<string, AudioClip> _cechAudio = new Dictionary<string, AudioClip>();
        private AudioConfiguration _configuration;
        private AudioDatabase _database;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            GameObject instance = new GameObject("Audio Management");
            Instance = instance.AddComponent<AudioManagement>();

            DontDestroyOnLoad(instance); 
        }

        private void Awake()
        {
            _configuration = AudioConfiguration.GetInstance();
            if (_configuration != null && _configuration.HasDatabase())
            {
                _database = _configuration.GetDatabase();
                _database.Initialize();
                PreloadAudio();
            }
            else
            {
                Debug.LogError("Audio Management: AudioDatabase is not assigned in AudioManagementSettings.");
            }
        }

        public IEnumerator GetClip(string key, Action<AudioClip> result)
        {
            if (_cechAudio.TryGetValue(key, out AudioClip clip))
            {
                result.Invoke(clip);
                yield break;
            }

            if (_database == null)
            {
                Debug.LogError("Audio Management: database is null. Cannot load clip: " + key);
                result?.Invoke(null);
                yield break;
            }

            AudioData audioData = null;
            try
            {
                audioData = _database.GetData(key);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                result?.Invoke(null);
                yield break;
            }

            if (audioData == null)
            {
                Debug.LogError("Audio Management: clip is missing for key: " + key);
                result?.Invoke(null);
                yield break;
            }
                
#if UNITY_EDITOR || !UNITY_WEBGL
            if (audioData.Clip == null)
            {
                Debug.LogError("Audio Management: clip is missing for key: " + key);
                result?.Invoke(null);
                yield break;
            }

            result.Invoke(audioData.Clip);
            yield break;
#else
            if (string.IsNullOrEmpty(audioData.Name))
            {
                Debug.LogError("Audio Management: clip name is missing for key: " + key);
                result?.Invoke(null);
                yield break;
            }

            string path = Application.streamingAssetsPath + "/Audio/" + audioData.FolderPath + audioData.Name;
            yield return LoadClip(path, audioData.Key, result);
#endif
        }

        private void PreloadAudio()
        {
            if (_database == null)
            {
                return;
            }

#if UNITY_EDITOR || !UNITY_WEBGL
            foreach (AudioData audioData in _database.Items)
            {
                if (audioData.Preload == false)
                {
                    continue;
                }
                
                _cechAudio[audioData.Key] = audioData.Clip;
            }
            
            return;
#else
            foreach (AudioData audioData in _database.Items)
            {
                if (audioData.Preload == false)
                {
                    continue;
                }
                
                string path = Application.streamingAssetsPath + "/Audio/" + audioData.FolderPath + audioData.Name;
                StartCoroutine(LoadClip(path, audioData.Key));
            }
#endif
        }

        private IEnumerator LoadClip(string path, string key, Action<AudioClip> result = null)
        {
            float startTime = Time.time;

            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                    _cechAudio[key] = audioClip;

                    result?.Invoke(audioClip);
                    
                    Debug.Log("Audio clip loaded: " + key + " time: " + (Time.time - startTime));
                }
            }
        }
    }
}
