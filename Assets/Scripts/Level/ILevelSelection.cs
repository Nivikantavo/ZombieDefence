/// <summary>
/// Level the game is going to play, shared between the menu and the gameplay scene.
/// </summary>
public interface ILevelSelection
{
    LevelAddress Current { get; }

    void Select(LevelAddress address);
}
