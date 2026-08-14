using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsHandler : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        GameAnalytics.Initialize();
#endif
    }
}
