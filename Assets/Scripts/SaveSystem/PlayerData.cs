[System.Serializable]
public class PlayerData
{
    public const int StagesCount = 4;

    public int Money;
    public int ComplitedStages;
    public int ComplitedLevelsOnStage;
    public int[] CompletedLevelsPerStage;
    public int SelectedLevel;
    public int SelectedStage;
    public float Sensetive;
    public float MusicVolume;
    public float SoundsVolume;
    public string[] Weapons;
    public string[] UpgradeWeapons;
    public int WeaponUpgradeFormatVersion;
    public string[] ProductsID;
    public string[] ProcessedPurchaseTokens;
    public int GranadesCount;
    public string[] Forces;
    public int TruckHealth;
    public bool SurvivalMode;
    public bool TrainingCompleted;
    public float SurviveTimeRecord;
    public string Language;

    public PlayerData()
    {
        Money = 0;
        ComplitedStages = 0;
        ComplitedLevelsOnStage = 0;
        CompletedLevelsPerStage = new int[StagesCount];
        Weapons = new string[1];
        Weapons[0] = "SMG 01";
        UpgradeWeapons = new string[0];
        WeaponUpgradeFormatVersion = 0;
        ProductsID = new string[0];
        ProcessedPurchaseTokens = new string[0];
        GranadesCount = 1;
        Forces = new string[1];
        TruckHealth = 300;
        SurvivalMode = false;
        TrainingCompleted = false;
        Sensetive = 1;
        MusicVolume = 0f;
        SoundsVolume = 0f;
        SurviveTimeRecord = 0;
        Language = string.Empty;
    }

    public void EnsureProgressArrays()
    {
        if (CompletedLevelsPerStage == null || CompletedLevelsPerStage.Length != StagesCount)
        {
            CompletedLevelsPerStage = new int[StagesCount];
        }

        if (Weapons == null || Weapons.Length == 0)
            Weapons = new[] { "SMG 01" };

        if (Forces == null)
            Forces = new string[0];

        if (UpgradeWeapons == null)
            UpgradeWeapons = new string[0];

        MigrateWeaponUpgradesIfNeeded();
        EnsureIapArrays();
    }

    public void EnsureIapArrays()
    {
        if (ProductsID == null)
            ProductsID = new string[0];

        if (ProcessedPurchaseTokens == null)
            ProcessedPurchaseTokens = new string[0];
    }

    public bool MigrateWeaponUpgradesIfNeeded()
    {
        if (WeaponUpgradeFormatVersion >= WeaponUpgradeLevels.CurrentFormatVersion)
        {
            return false;
        }

        if (UpgradeWeapons != null)
        {
            int validCount = 0;
            for (int i = 0; i < UpgradeWeapons.Length; i++)
            {
                if (string.IsNullOrEmpty(UpgradeWeapons[i]) == false)
                {
                    validCount++;
                }
            }

            string[] migrated = new string[validCount * 2];
            int writeIndex = 0;
            for (int i = 0; i < UpgradeWeapons.Length; i++)
            {
                if (string.IsNullOrEmpty(UpgradeWeapons[i]))
                {
                    continue;
                }

                migrated[writeIndex++] = UpgradeWeapons[i];
                migrated[writeIndex++] = UpgradeWeapons[i];
            }

            UpgradeWeapons = migrated;
        }
        else
        {
            UpgradeWeapons = new string[0];
        }

        WeaponUpgradeFormatVersion = WeaponUpgradeLevels.CurrentFormatVersion;
        return true;
    }

    public int GetWeaponUpgradeCount(string weaponName)
    {
        if (UpgradeWeapons == null || string.IsNullOrEmpty(weaponName))
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < UpgradeWeapons.Length; i++)
        {
            if (UpgradeWeapons[i] == weaponName)
            {
                count++;
            }
        }

        return count;
    }

    public int GetCompletedLevelsOnStage(int stageIndex)
    {
        EnsureProgressArrays();
        if (stageIndex < 0 || stageIndex >= CompletedLevelsPerStage.Length)
        {
            return 0;
        }

        return CompletedLevelsPerStage[stageIndex];
    }
}
