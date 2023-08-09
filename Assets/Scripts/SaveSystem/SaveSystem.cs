using Agava.YandexGames;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public bool DataLoaded { get; private set; }

    private PlayerData _playerData;
    private string file = "PlayerData.txt";

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
#if UNITY_EDITOR

        Load();
#endif

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
#if UNITY_WEBGL && !UNITY_EDITOR

        string jsonData = JsonUtility.ToJson(_playerData);
        PlayerAccount.SetCloudSaveData(jsonData);
#endif
#if UNITY_EDITOR
        string json = JsonUtility.ToJson(_playerData);
        WriteToFile(file, json);
#endif
    }

    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerAccount.GetCloudSaveData(OnLoadDataSuccess, OnLoadDataError);
#endif
#if UNITY_EDITOR
        _playerData = new PlayerData();
        string json = ReadFromFile(file);
        JsonUtility.FromJsonOverwrite(json, _playerData);
        DataLoaded = true;
#endif
    }

    public PlayerData GetData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (YandexGamesSdk.IsInitialized == false)
            return null;
#endif
        return _playerData;
    }

    public void SetMoneyValue(int money)
    {
        if(money >= 0)
        {
            _playerData.Money = money;
        }
        Save();
    }

    public void SetProgress(int complitedLevelNumber, int stageNumber)
    {
        if(stageNumber <= _playerData.ComplitedStages)
        {
            return;
        }
        _playerData.ComplitedLevelsOnStage = complitedLevelNumber;

        if (complitedLevelNumber == 3)
        {
            if (stageNumber > _playerData.ComplitedStages)
            {
                _playerData.ComplitedStages = stageNumber;
                _playerData.ComplitedLevelsOnStage = 0;
            }
        }
        Save();
    }

    public void SetSensetiveValue(float sensetive)
    {
        _playerData.Sensetive = sensetive;
        Save();
    }

    public void SetWeaponsArrey(string[] weapons)
    {
        _playerData.Weapons = weapons;
        Save();
    }

    public void SetWeaponsUpgradeArrey(string[] upgradeWeapons)
    {
        _playerData.UpgradeWeapons = upgradeWeapons;
        Save();
    }

    public void SetForcesArrey(string[] forces)
    {
        _playerData.Forces = forces;
        Save();
    }

    public void SetGranadesCount(int granadesCount)
    {
        _playerData.GranadesCount = granadesCount;
        Save();
    }

    public void SetTruckHealth(int truckHealth)
    {
        _playerData.TruckHealth = truckHealth;
        Save();
    }

    public void SetSurvivalModeEnabled(bool enabled)
    {
        _playerData.SurvivalMode = enabled;
        Save();
    }

    public void SetSurvivelRecord(float newRecord)
    {
        _playerData.SurviveTimeRecord = newRecord;
        Save();
    }

    public void SetSelectedLevel(int selectedLevel)
    {
        _playerData.SelectedLevel = selectedLevel;
        Save();
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

    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("File not founded");
        }
        return "";
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
}
