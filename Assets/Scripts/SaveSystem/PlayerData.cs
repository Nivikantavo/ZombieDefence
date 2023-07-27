public class PlayerData
{
    public int Money;
    public int ComplitedStages;
    public int ComplitedLevelsOnStage;
    public float Sensetive;
    public string[] Weapons;
    public int GranadesCount;
    public string[] Forces;
    public int TruckHealth;
    public bool SurvivalMode;
    public float SurviveTimeRecord;

    public PlayerData()
    {
        Money = 0;
        ComplitedStages = 0;
        ComplitedLevelsOnStage = 0;
        Weapons = new string[1];
        Weapons[0] = "SMG 01";
        GranadesCount = 1;
        Forces = new string[1] {""};
        TruckHealth = 300;
        SurvivalMode = false;
        Sensetive = 1;
        SurviveTimeRecord = 0;
    }
}
