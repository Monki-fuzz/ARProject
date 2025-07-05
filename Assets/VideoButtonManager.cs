using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VideoButtonManager : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private PrefabDataLoader loader;

    void Start()
    {
        StartCoroutine(AssignButtonsWhenPrefabIsReady());
    }

    IEnumerator AssignButtonsWhenPrefabIsReady()
    {
        float timeout = 60f;
        float timer = 0f;

        Debug.Log("Waiting for prefab to spawn...");

        while (timer < timeout)
        {
            GameObject model = GameObject.FindGameObjectWithTag("modelObject");
            if (model != null)
            {
                loader = model.GetComponentInChildren<PrefabDataLoader>();
                if (loader != null)
                    break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (loader == null)
        {
            Debug.LogError("PrefabDataLoader not found after 60 seconds.");
            yield break;
        }

        Debug.Log("PrefabDataLoader found. Hooking up buttons...");

        button1.onClick.AddListener(() => { Debug.Log("Video 1 button clicked"); loader.LoadYouTubeVideo(1); });
        button2.onClick.AddListener(() => { Debug.Log("Video 2 button clicked"); loader.LoadYouTubeVideo(2); });
        button3.onClick.AddListener(() => { Debug.Log("Video 3 button clicked"); loader.LoadYouTubeVideo(3); });
        button4.onClick.AddListener(() => { Debug.Log("Video 4 button clicked"); loader.LoadYouTubeVideo(4); });
    }
}
