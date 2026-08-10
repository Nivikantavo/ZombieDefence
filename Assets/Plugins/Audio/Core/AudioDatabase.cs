using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Audio.Core
{
    [CreateAssetMenu(fileName = "Audio Database", menuName = "Audio/Database")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeField] private List<AudioData> _items = new List<AudioData>();


        private Dictionary<string, AudioData> _data = new Dictionary<string, AudioData>();
        public List<AudioData> Items => _items;

        public void Initialize()
        {
            foreach (AudioData item in _items)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                _data[item.Key] = item;
            }
        }

        public void AddData()
        {
            Items.Add(new AudioData());
        }

        public void DeleteDta(int index)
        {
            Items.RemoveAt(index);
        }

        public string GetClipPath(string key)
        {
            AudioData data = GetData(key);
            return data.FolderPath + "/" + data.Name;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        public AudioClip GetClip(string key)
        {
            AudioData data = GetData(key);
            return data.Clip;
        }
#endif

        public AudioData GetData(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_data.TryGetValue(key, out AudioData data))
                {
                    return data;
                }

                // SourceAudio often passes AudioClip.name (without extension),
                // while database keys may include ".wav".
                if (_data.TryGetValue(key + ".wav", out data))
                {
                    return data;
                }

                foreach (KeyValuePair<string, AudioData> pair in _data)
                {
                    string entryKey = pair.Key;
                    if (entryKey == key)
                    {
                        return pair.Value;
                    }

                    int dot = entryKey.LastIndexOf('.');
                    if (dot > 0 && entryKey.Substring(0, dot) == key)
                    {
                        return pair.Value;
                    }
                }
            }

            throw new Exception("Audio Database not found data at key: " + key);
        }

#if UNITY_EDITOR
        public void AddData(string assetPath, AudioClip audioClip)
        {
            AudioData data = new AudioData();
            data.Key = assetPath;
            data.Clip = audioClip;
            
            Items.Add(data);
        }
#endif
     

        public void Clear()
        {
            _items.Clear();
        }
    }
}