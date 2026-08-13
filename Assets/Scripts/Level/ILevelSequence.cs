/// <summary>
/// Order in which levels follow each other across all locations.
/// </summary>
public interface ILevelSequence
{
    /// <summary>
    /// Returns the level that follows <paramref name="current"/>, or false when <paramref name="current"/> is the very
    /// last level of the game or lies outside of the sequence.
    /// </summary>
    bool TryGetNext(LevelAddress current, out LevelAddress next);
}
