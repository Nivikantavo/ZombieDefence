using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_WEBGL
using Playgama;
#endif

public class SaveSystem : MonoBehaviour
{
    private const string PlayerDataKey = "player_data";

    public bool DataLoaded { get; private set; }
    public IapPurchaseService IapPurchases { get; private set; }

    private PlayerData _playerData;
    private string file = "PlayerData.txt";

    public static SaveSystem Instance;

    public event UnityAction DataUpdated;

    private void Awake()
    {
        DataLoaded = false;
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            IapPurchases = GetComponent<IapPurchaseService>();
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
#else
        while (Bridge.instance == null)
            yield return null;

        Load();
#endif
    }

    public void Save()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string jsonData = JsonUtility.ToJson(_playerData);
        Bridge.storage.Set(PlayerDataKey, jsonData);
#endif
#if UNITY_EDITOR
        string json = JsonUtility.ToJson(_playerData);
        WriteToFile(file, json);
#endif
    }

    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.storage.Get(PlayerDataKey, OnStorageGetCompleted);
#endif
#if UNITY_EDITOR
        _playerData = new PlayerData();
        string json = ReadFromFile(file);
        JsonUtility.FromJsonOverwrite(json, _playerData);
        PersistWeaponUpgradeMigration();
        DataLoaded = true;
        RestoreIapPurchases();
        DataUpdated?.Invoke();
#endif
    }

    public PlayerData GetData()
    {
        if (_playerData != null)
        {
            PersistWeaponUpgradeMigration();
        }

        return _playerData;
    }

    private void PersistWeaponUpgradeMigration()
    {
        if (_playerData == null)
        {
            return;
        }

        bool migrated = _playerData.MigrateWeaponUpgradesIfNeeded();
        _playerData.EnsureProgressArrays();
        _playerData.EnsureIapArrays();
        if (migrated)
        {
            Save();
        }
    }

    public void SaveAndNotify()
    {
        Save();
        DataUpdated?.Invoke();
    }

    public void DeleteData()
    {
        _playerData = new PlayerData();

        Save();
    }

    [ContextMenu("Unlock All Levels")]
    public void UnlockAllLevels()
    {
        if (_playerData == null)
        {
            _playerData = new PlayerData();
        }

        _playerData.EnsureProgressArrays();

        for (int i = 0; i < _playerData.CompletedLevelsPerStage.Length; i++)
        {
            _playerData.CompletedLevelsPerStage[i] = Stage.LevelsPerStage;
        }

        _playerData.ComplitedStages = PlayerData.StagesCount;
        _playerData.ComplitedLevelsOnStage = Stage.LevelsPerStage;
        Save();
        DataUpdated?.Invoke();
    }

    public void SetMoneyValue(int money)
    {
        if(money != _playerData.Money)
        {
            if (money >= 0)
            {
                _playerData.Money = money;
            }
            Save();
        }
    }

    public void SetProgress(int complitedLevelNumber, int stageNumber)
    {
        _playerData.EnsureProgressArrays();

        int stageIndex = stageNumber - 1;
        if (stageIndex < 0 || stageIndex >= _playerData.CompletedLevelsPerStage.Length)
        {
            return;
        }

        if (complitedLevelNumber <= _playerData.CompletedLevelsPerStage[stageIndex])
        {
            return;
        }

        _playerData.CompletedLevelsPerStage[stageIndex] = complitedLevelNumber;
        _playerData.ComplitedLevelsOnStage = complitedLevelNumber;

        if (complitedLevelNumber >= Stage.LevelsPerStage)
        {
            if (stageNumber > _playerData.ComplitedStages)
            {
                _playerData.ComplitedStages = stageNumber;
            }
        }

        Save();
    }

    public void SetSensetiveValue(float sensetive)
    {
        if(_playerData.Sensetive != sensetive)
        {
            _playerData.Sensetive = sensetive;
            Save();
        }
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
        if(_playerData.SurvivalMode != enabled)
        {
            _playerData.SurvivalMode = enabled;
            Save();
        }
    }

    public void SetSurvivelRecord(float newRecord)
    {
        _playerData.SurviveTimeRecord = newRecord;
        Save();
    }

    public void SetSelectedLevel(int selectedLevel)
    {
        if(_playerData.SelectedLevel != selectedLevel)
        {
            _playerData.SelectedLevel = selectedLevel;
            Save();
        }
    }

    public void SetSelectedStage(int selectedStage)
    {
        if( _playerData.SelectedStage != selectedStage)
        {
            _playerData.SelectedStage = selectedStage;
            Save();
        }
    }

    public void SetBoughtProduct(string productID)
    {
        _playerData.EnsureIapArrays();
        List<string> products = _playerData.ProductsID.ToList();

        if(products.Contains(productID) == false)
        {
            products.Add(productID);
        }

        _playerData.ProductsID = products.ToArray();
        Save();
    }

    public void SetTrainingCompleted(bool complited)
    {
        _playerData.TrainingCompleted = complited;
        Save();
    }

    public void SetSoundsValue(float musicVolume, float soundVolume)
    {
        if(_playerData.MusicVolume != musicVolume || _playerData.SoundsVolume != soundVolume)
        {
            _playerData.MusicVolume = musicVolume;
            _playerData.SoundsVolume = soundVolume;
            Save();
        }
    }

    public void SetLanguage(string language)
    {
        if (_playerData.Language == language)
            return;

        _playerData.Language = language;
        Save();
    }

#if UNITY_WEBGL
    private void OnStorageGetCompleted(bool success, string data)
    {
        if (success == false)
        {
            OnLoadDataError("Failed to load player data from Playgama storage");
            _playerData = new PlayerData();
            DataLoaded = true;
            RestoreIapPurchases();
            DataUpdated?.Invoke();
            return;
        }

        OnLoadDataSuccess(data);
        DataUpdated?.Invoke();
    }
#endif

    private void OnLoadDataSuccess(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            _playerData = new PlayerData();
        }
        else
        {
            _playerData = JsonUtility.FromJson<PlayerData>(data);
            PersistWeaponUpgradeMigration();
        }
        DataLoaded = true;
        RestoreIapPurchases();
    }

    private void RestoreIapPurchases()
    {
        if (IapPurchases == null)
            IapPurchases = GetComponent<IapPurchaseService>();

        if (IapPurchases == null)
            return;

        IapPurchases.Initialize();
        IapPurchases.LoadCatalog();
        IapPurchases.RestorePurchases();
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
