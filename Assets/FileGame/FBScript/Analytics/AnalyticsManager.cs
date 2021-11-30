using Firebase.Analytics;

public static class AnalyticsManager
{
    private static void LogEvent(string eventName, params Parameter[] parameters)
    {
        // Method utama untuk menembakan firebase
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    public static void LogUpgradeEvent(int resourceIndex, int level)
    {
        // Kita memakai event dan parameter yang tersedia di firebase(tidak memakai yang custom)
        // agar dapat muncul sebagai report data di analytics firebase
        LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString()),
            new Parameter(FirebaseAnalytics.ParameterLevel, level));
        
        // karena resourceIndex digunakan sebagai id,maka seharusnya kita menyimpannya
        // sebagai string bukan integer
    }

    public static void LogUnlockEvent(int resoourceIndex)
    {
        LogEvent(
            FirebaseAnalytics.EventUnlockAchievement,
            new Parameter(FirebaseAnalytics.ParameterIndex,
            resoourceIndex.ToString()));
    }

    public static void SetUserProperties(string name, string value)
    {
        FirebaseAnalytics.SetUserProperty(name, value);
    }
}
