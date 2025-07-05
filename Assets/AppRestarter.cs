using UnityEngine;
using UnityEngine.SceneManagement;

public class AppRestarter : MonoBehaviour
{
    // Call this method from the button
    public void RestartApp()
    {
#if UNITY_EDITOR
        // In the editor, just reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
        // On Android, quit and reopen using AndroidJavaObject
        StartCoroutine(RestartAndroidApp());
#endif
    }

#if !UNITY_EDITOR
    private System.Collections.IEnumerator RestartAndroidApp()
    {
        yield return new WaitForSeconds(0.5f); // short delay

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject pm = activity.Call<AndroidJavaObject>("getPackageManager");
            string packageName = activity.Call<string>("getPackageName");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            intent.Call<AndroidJavaObject>("addFlags", 0x10000000); // FLAG_ACTIVITY_NEW_TASK
            activity.Call("startActivity", intent);
            activity.Call("finish");
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // force kill current
        }
    }
#endif
}
