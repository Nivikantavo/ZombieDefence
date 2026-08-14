using UnityEngine;

[System.Serializable]
public struct WeaponAttachmentLevel
{
    [Tooltip("Index of the scope in the weapon's Scope Array. Negative value uses ironsights/default.")]
    public int ScopeIndex;

    [Tooltip("Index of the muzzle in the weapon's Muzzle Array.")]
    public int MuzzleIndex;

    [Tooltip("Index of the laser in the weapon's Laser Array. Negative value hides the laser.")]
    public int LaserIndex;

    [Tooltip("Index of the grip in the weapon's Grip Array. Negative value hides the grip.")]
    public int GripIndex;
}

public static class WeaponUpgradeLevels
{
    public const int MaxLevel = 5;
    public const int LegacyMappedLevel = 3;
    public const int CurrentFormatVersion = 1;

    public static int Clamp(int level)
    {
        return Mathf.Clamp(level, 1, MaxLevel);
    }

    public static int ToIndex(int level)
    {
        return Clamp(level) - 1;
    }

    public static int GetLevelFromUpgradeCount(int upgradeCount)
    {
        return Clamp(1 + Mathf.Max(0, upgradeCount));
    }

    public static bool NeedsFill(float[] values)
    {
        if (values == null || values.Length != MaxLevel)
        {
            return true;
        }

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != 0f)
            {
                return false;
            }
        }

        return true;
    }

    public static bool NeedsFill(int[] values)
    {
        if (values == null || values.Length != MaxLevel)
        {
            return true;
        }

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool NeedsFill(WeaponAttachmentLevel[] values)
    {
        return values == null || values.Length != MaxLevel;
    }

    public static float[] CreateInterpolated(float level1, float level3)
    {
        float[] values = new float[MaxLevel];
        FillInterpolated(values, level1, level3);
        return values;
    }

    public static int[] CreateInterpolated(int level1, int level3)
    {
        int[] values = new int[MaxLevel];
        FillInterpolated(values, level1, level3);
        return values;
    }

    public static void FillInterpolated(float[] values, float level1, float level3)
    {
        values[0] = level1;
        values[2] = level3;
        values[1] = Mathf.Lerp(level1, level3, 0.5f);
        float step = (level3 - level1) * 0.5f;
        values[3] = level3 + step;
        values[4] = level3 + step * 2f;
    }

    public static void FillInterpolated(int[] values, int level1, int level3)
    {
        values[0] = level1;
        values[2] = level3;
        values[1] = Mathf.RoundToInt(Mathf.Lerp(level1, level3, 0.5f));
        float step = (level3 - level1) * 0.5f;
        values[3] = Mathf.Max(0, Mathf.RoundToInt(level3 + step));
        values[4] = Mathf.Max(0, Mathf.RoundToInt(level3 + step * 2f));
        values[1] = Mathf.Max(0, values[1]);
    }

    public static WeaponAttachmentLevel[] CreateAttachmentLevels(
        WeaponAttachmentLevel level1,
        WeaponAttachmentLevel level3)
    {
        return new WeaponAttachmentLevel[MaxLevel]
        {
            level1,
            level1,
            level3,
            level3,
            level3
        };
    }
}
