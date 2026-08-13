using System;

/// <summary>
/// Immutable coordinates of a playable level: the location it belongs to and its place inside that location.
/// Survival mode has no address because it is not a level.
/// </summary>
public readonly struct LevelAddress : IEquatable<LevelAddress>
{
    public const int FirstStageNumber = 1;
    public const int FirstLevelIndex = 0;

    /// <summary>
    /// Location number as it is stored in <see cref="PlayerData.SelectedStage"/>, counted from one.
    /// </summary>
    public int StageNumber { get; }

    /// <summary>
    /// Level index inside the location as it is stored in <see cref="PlayerData.SelectedLevel"/>, counted from zero.
    /// </summary>
    public int LevelIndex { get; }

    public LevelAddress(int stageNumber, int levelIndex)
    {
        StageNumber = stageNumber;
        LevelIndex = levelIndex;
    }

    public static LevelAddress FirstOfStage(int stageNumber)
    {
        return new LevelAddress(stageNumber, FirstLevelIndex);
    }

    public bool Equals(LevelAddress other)
    {
        return StageNumber == other.StageNumber && LevelIndex == other.LevelIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is LevelAddress other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (StageNumber * 397) ^ LevelIndex;
    }

    public override string ToString()
    {
        return $"stage {StageNumber}, level {LevelIndex + 1}";
    }
}
