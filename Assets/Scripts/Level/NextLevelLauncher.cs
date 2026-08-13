using System;

/// <summary>
/// Starts the level that follows the selected one. All locations live in a single gameplay scene, so switching a level
/// means writing the new selection into the save and reloading that scene.
/// </summary>
public class NextLevelLauncher
{
    private readonly ILevelSequence _sequence;
    private readonly ILevelSelection _selection;
    private readonly ISceneLoader _sceneLoader;
    private readonly int _gameplaySceneId;

    public NextLevelLauncher(ILevelSequence sequence, ILevelSelection selection, ISceneLoader sceneLoader, int gameplaySceneId)
    {
        _sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        _gameplaySceneId = gameplaySceneId;
    }

    public bool HasNextLevel => _sequence.TryGetNext(_selection.Current, out _);

    /// <summary>
    /// Selects and loads the next level. Returns false when the last level of the last location is finished.
    /// </summary>
    public bool TryLaunch()
    {
        if (_sequence.TryGetNext(_selection.Current, out LevelAddress next) == false)
        {
            return false;
        }

        _selection.Select(next);
        _sceneLoader.LoadScene(_gameplaySceneId);
        return true;
    }
}
