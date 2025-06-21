using UnityEngine;
using System;

public class AppExpiryChecker : MonoBehaviour
{
    private const string startDateKey = "AppStartDate";
    private const string apkTimeKey = "LastApkUpdateTime";
    private const int expiryMinutes = 1; // Change this to your expiry time

    public GameObject expiredUIPanel; // Assign in the Inspector

    void Start()
    {
        Debug.Log("AppExpiryChecker Start() called");

        // Get current APK install/update time
        long apkTime = GetApkLastUpdateTime();
        long savedApkTime = long.Parse(PlayerPrefs.GetString(apkTimeKey, "0"));

        // If APK has been newly installed or re-downloaded
        if (apkTime != savedApkTime)
        {
            Debug.Log("APK was updated or reinstalled. Resetting expiry.");
            PlayerPrefs.DeleteKey(startDateKey);
            PlayerPrefs.SetString(apkTimeKey, apkTime.ToString());
            PlayerPrefs.Save();
        }

        // Save install time if not already
        if (!PlayerPrefs.HasKey(startDateKey))
        {
            string now = DateTime.UtcNow.ToString("o");
            PlayerPrefs.SetString(startDateKey, now);
            PlayerPrefs.Save();
            Debug.Log("First launch. Start date saved: " + now);
        }

        string savedTimeStr = PlayerPrefs.GetString(startDateKey);

        try
        {
            DateTime savedTime = DateTime.Parse(savedTimeStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
            TimeSpan elapsed = DateTime.UtcNow - savedTime;

            Debug.Log("Minutes passed since install: " + elapsed.TotalMinutes);

            if (elapsed.TotalMinutes < 0)
            {
                Debug.LogWarning("Saved install time is in the future. Resetting.");
                PlayerPrefs.SetString(startDateKey, DateTime.UtcNow.ToString("o"));
                PlayerPrefs.Save();
                elapsed = TimeSpan.Zero;
            }

            if (elapsed.TotalMinutes > expiryMinutes)
            {
                BlockAppAccess();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Date parsing failed: " + e.Message);
            PlayerPrefs.SetString(startDateKey, DateTime.UtcNow.ToString("o"));
            PlayerPrefs.Save();
        }
    }

    void BlockAppAccess()
    {
        Debug.Log("App access expired.");
        Debug.Log("Trying to show expiredUIPanel: " + (expiredUIPanel != null));

        if (expiredUIPanel != null)
        {
            expiredUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("expiredUIPanel not assigned in Inspector!");
        }

        // Optional: Disable other systems here
        // GetComponent<YourARManager>().enabled = false;
    }

    long GetApkLastUpdateTime()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var packageManager = context.Call<AndroidJavaObject>("getPackageManager"))
            using (var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", context.Call<string>("getPackageName"), 0))
            {
                return packageInfo.Get<long>("lastUpdateTime");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to get APK last update time: " + e.Message);
            return 0;
        }
#else
        return 0; // Not available in editor
#endif
    }
}
