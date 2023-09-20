public class PlayerData
{
    public int Money;
    public int ComplitedStages;
    public int ComplitedLevelsOnStage;
    public int SelectedLevel;
    public int SelectedStage;
    public float Sensetive;
    public float MusicVolume;
    public float SoundsVolume;
    public string[] Weapons;
    public string[] UpgradeWeapons;
    public string[] ProductsID;
    public int GranadesCount;
    public string[] Forces;
    public int TruckHealth;
    public bool SurvivalMode;
    public bool TrainingCompleted;
    public float SurviveTimeRecord;

    public PlayerData()
    {
        Money = 0;
        ComplitedStages = 0;
        ComplitedLevelsOnStage = 0;
        Weapons = new string[1];
        Weapons[0] = "SMG 01";
        UpgradeWeapons = new string[1];
        ProductsID = new string[1];
        GranadesCount = 1;
        Forces = new string[1];
        TruckHealth = 300;
        SurvivalMode = false;
        TrainingCompleted = false;
        Sensetive = 1;
        MusicVolume = 0f;
        SoundsVolume = 0f;
        SurviveTimeRecord = 0;
    }
}
