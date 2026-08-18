using System.Collections;
using Lean.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_WEBGL
using Playgama;
#endif

public static class GameLanguage
{
    public const string English = "English";
    public const string Russian = "Russian";
    public const string Turkish = "Turkish";

    private static int _spawnedFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SpawnApplier();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnApplier();
    }

    private static void SpawnApplier()
    {
        if (_spawnedFrame == Time.frameCount)
            return;

        _spawnedFrame = Time.frameCount;

        var applier = new GameObject(nameof(GameLanguageApplier));
        applier.hideFlags = HideFlags.HideAndDontSave;
        applier.AddComponent<GameLanguageApplier>();
    }

    public static void SetManual(string language)
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.SetLanguage(language);

        Apply(language);
    }

    public static void Apply(string language)
    {
        if (string.IsNullOrEmpty(language))
            return;

        LeanLocalization.SetCurrentLanguageAll(language);
    }

    public static string MapFromPlatformCode(string language)
    {
        string code = language == null ? string.Empty : language.ToLowerInvariant();

        if (IsRussianFamily(code))
            return Russian;

        if (code == "tr" || code.StartsWith("tr-"))
            return Turkish;

        return English;
    }

    public static IEnumerator ApplyResolvedWhenReady()
    {
        while (SaveSystem.Instance == null || SaveSystem.Instance.DataLoaded == false)
            yield return null;

        while (LeanLocalization.Instances.Count == 0)
            yield return null;

        PlayerData data = SaveSystem.Instance.GetData();
        if (data != null && string.IsNullOrEmpty(data.Language) == false)
        {
            Apply(data.Language);
            yield break;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        while (Bridge.instance == null)
            yield return null;

        Apply(MapFromPlatformCode(Bridge.platform.language));
#endif
    }

    private static bool IsRussianFamily(string code)
    {
        return code == "ru" || code.StartsWith("ru-")
            || code == "be" || code.StartsWith("be-")
            || code == "kk" || code.StartsWith("kk-")
            || code == "uk" || code.StartsWith("uk-")
            || code == "uz" || code.StartsWith("uz-");
    }
}

internal sealed class GameLanguageApplier : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return GameLanguage.ApplyResolvedWhenReady();
        Destroy(gameObject);
    }
}
