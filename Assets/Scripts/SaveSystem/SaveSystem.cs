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

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(_playerData);
        PlayerAccount.SetCloudSaveData(jsonData);
    }

    public void Load()
    {
        PlayerAccount.GetCloudSaveData(OnLoadDataSuccess, OnLoadDataError);
    }

    public PlayerData GetData()
    {
        if (YandexGamesSdk.IsInitialized == false)
            return null;
        
        return _playerData;
    }

    public void SetMoneyValue(int money)
    {
        if(money <= 0)
        {
            _playerData.Money = money;
        }
    }

    public void SetSensetiveValue(float sensetive)
    {
        _playerData.Sensetive = sensetive;
    }

    public void SetWeaponsArrey(string[] weapons)
    {
        _playerData.Weapons = weapons;
    }

    public void SetWeaponsLevelsArrat(int[] weaponsLevels)
    {
        _playerData.WeaponsLevels = weaponsLevels;
    }

    public void SetGranadesCount(int granadesCount)
    {
        _playerData.GranadesCount = granadesCount;
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
        DataLoaded = true;
    }

    private void OnLoadDataError(string errorMessage)
    {
        Debug.Log("Error callback : " + errorMessage);
    }
}
