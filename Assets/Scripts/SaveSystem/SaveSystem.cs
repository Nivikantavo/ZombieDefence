using Agava.YandexGames;
using System.Collections;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public bool DataLoaded { get; private set; }

    private PlayerData _playerData;

    public static SaveSystem Instance;

    private void Awake()
    {
        DataLoaded = false;
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        yield return YandexGamesSdk.Initialize();
        Load();
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(_playerData);
        PlayerAccount.SetCloudSaveData(jsonData);
    }

    public void Load()
    {
        Debug.Log("Load");
        PlayerAccount.GetCloudSaveData(OnLoadDataSuccess, OnLoadDataError);
    }

    public PlayerData GetData()
    {
        if (YandexGamesSdk.IsInitialized == false)
            return null;
        Debug.Log("Get data");
        return _playerData;
    }

    private void OnLoadDataSuccess(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            _playerData = new PlayerData();
        }
        else
        {
            _playerData = JsonUtility.FromJson<PlayerData>(data);
        }
        Debug.Log("Success callback : " + _playerData);
        DataLoaded = true;
    }

    private void OnLoadDataError(string errorMessage)
    {
        Debug.Log("Error callback : " + errorMessage);
    }
}
