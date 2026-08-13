using System;

/// <summary>
/// Levels are ordered inside a location first; after the last level of a location the sequence continues with the
/// first level of the next one. Survival mode is skipped because it is not a level.
/// </summary>
public class LevelSequence : ILevelSequence
{
    private readonly int _stagesCount;
    private readonly int _levelsPerStage;

    public LevelSequence(int stagesCount, int levelsPerStage)
    {
        if (stagesCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stagesCount));
        }

        if (levelsPerStage <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(levelsPerStage));
        }

        _stagesCount = stagesCount;
        _levelsPerStage = levelsPerStage;
    }

    public bool TryGetNext(LevelAddress current, out LevelAddress next)
    {
        next = default;

        if (Contains(current) == false)
        {
            return false;
        }

        int nextLevelIndex = current.LevelIndex + 1;
        if (nextLevelIndex < _levelsPerStage)
        {
            next = new LevelAddress(current.StageNumber, nextLevelIndex);
            return true;
        }

        int nextStageNumber = current.StageNumber + 1;
        if (nextStageNumber > _stagesCount)
        {
            return false;
        }

        next = LevelAddress.FirstOfStage(nextStageNumber);
        return true;
    }

    private bool Contains(LevelAddress address)
    {
        return address.StageNumber >= LevelAddress.FirstStageNumber
            && address.StageNumber <= _stagesCount
            && address.LevelIndex >= LevelAddress.FirstLevelIndex
            && address.LevelIndex < _levelsPerStage;
    }
}
