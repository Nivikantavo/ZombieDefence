using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;

public class PausePanel : Element
{
    private void OnEnable()
    {
        characterBehaviour.LockCursor(false);
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        characterBehaviour.LockCursor(true);
        Time.timeScale = 1.0f;
    }
}
