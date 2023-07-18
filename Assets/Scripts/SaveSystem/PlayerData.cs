public class PlayerData
{
    public int Money;
    public int ComplitedStages;
    public int ComplitedLevelsOnStage;
    public float Sensetive;
    public string[] Weapons;
    public int[] WeaponsLevels;
    public int GranadesCount;
    public string[] Forces;

    public PlayerData()
    {
        Money = 0;
        ComplitedStages = 0;
        ComplitedLevelsOnStage = 0;
        Weapons = new string[1];
        Weapons[0] = "SMG 01";
        WeaponsLevels = new int[1];
        WeaponsLevels[0] = 1;
        GranadesCount = 1;
    }
}
