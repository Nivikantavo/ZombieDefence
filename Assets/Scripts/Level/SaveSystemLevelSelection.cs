using System;

/// <summary>
/// Keeps the selected level in the saved player data, which is what <see cref="LevelChoicer"/> and
/// <see cref="DifficultyChoicer"/> read when the gameplay scene starts.
/// </summary>
public class SaveSystemLevelSelection : ILevelSelection
{
    private readonly SaveSystem _saveSystem;

    public SaveSystemLevelSelection(SaveSystem saveSystem)
    {
        _saveSystem = saveSystem ?? throw new ArgumentNullException(nameof(saveSystem));
    }

    public LevelAddress Current
    {
        get
        {
            PlayerData data = _saveSystem.GetData();
            if (data == null)
            {
                return default;
            }

            return new LevelAddress(data.SelectedStage, data.SelectedLevel);
        }
    }

    public void Select(LevelAddress address)
    {
        _saveSystem.SetSelectedStage(address.StageNumber);
        _saveSystem.SetSelectedLevel(address.LevelIndex);
        _saveSystem.SetSurvivalModeEnabled(false);
    }
}
