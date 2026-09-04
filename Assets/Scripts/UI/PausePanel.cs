using InfimaGames.LowPolyShooterPack.Interface;
using UnityEngine;

public class PausePanel : Element
{
    private void OnEnable()
    {
        characterBehaviour.LockCursor(false);
        Time.timeScale = 0.0f;
        PlatformServices.Lifecycle?.NotifyLevelPaused();
        PlatformServices.Banners?.ShowPause();
    }

    private void OnDisable()
    {
        characterBehaviour.LockCursor(true);
        Time.timeScale = 1.0f;
        if (PlatformServices.Lifecycle != null && PlatformServices.Lifecycle.IsGameplayActive)
        {
            PlatformServices.Lifecycle.NotifyLevelResumed();
            PlatformServices.Banners?.Hide();
        }
    }
}
