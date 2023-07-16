using Agava.YandexGames;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public PlayerData PlayerData;

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(PlayerData);
        PlayerAccount.SetCloudSaveData(jsonData);
    }

    public void Load()
    {
        PlayerAccount.GetCloudSaveData(OnLoadDataSuccess);
    }

    private void OnLoadDataSuccess(string data)
    {
        PlayerData = JsonUtility.FromJson<PlayerData>(data);
    }
}
