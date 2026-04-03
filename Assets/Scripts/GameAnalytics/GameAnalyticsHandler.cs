using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsHandler : MonoBehaviour
{
    private void Awake()
    {
        GameAnalytics.Initialize();
    }
}
