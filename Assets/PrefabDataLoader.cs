using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class PrefabDataLoader : MonoBehaviour
{
    public string prefabName;
    public string apiUrl = "https://orionplex.com.my/test1/api/get_prefab.php";

    private GameObject modelObject;
    private TextMeshProUGUI introText;
    private TextMeshProUGUI useCaseText;

    private WebViewObject webViewObject;

    public void Initialize(string prefabName)
    {
        this.prefabName = prefabName;
        Debug.Log("[PrefabDataLoader] Initialize called with prefabName: " + prefabName);
        StartCoroutine(FetchPrefabData());
    }


    IEnumerator FetchPrefabData()
    {
        string url = apiUrl + "?prefab_name=" + prefabName;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PrefabContent content = JsonUtility.FromJson<PrefabContent>(json);

            modelObject = GameObject.FindGameObjectWithTag("modelObject");
            if (modelObject == null)
            {
                Debug.LogError("modelObject not found.");
                yield break;
            }

            Transform[] children = modelObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.CompareTag("CanvasPopUp"))
                    introText = child.GetComponentInChildren<TextMeshProUGUI>(true);
                else if (child.CompareTag("ApplicationPopUp"))
                    useCaseText = child.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (introText) introText.text = content.intro_text;
            if (useCaseText) useCaseText.text = content.use_cases;

            Debug.Log("Data applied to prefab: " + prefabName);
        }
        else
        {
            Debug.LogError("Fetch error: " + request.error);
        }
    }

    public void LoadYouTubeVideo(int videoNumber)
    {
        Debug.Log($"Trying to load video {videoNumber} for prefab: {prefabName}");

        GameObject videoScreen = GameObject.FindGameObjectWithTag("VideoScreen");
        if (videoScreen == null)
        {
            Debug.LogError("VideoScreen not found.");
            return;
        }

        if (webViewObject == null)
        {
            Debug.Log("Initializing WebViewObject...");
            webViewObject = videoScreen.AddComponent<WebViewObject>();

            webViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log("WebView message: " + msg);
                    if (msg == "close")
                    {
                        webViewObject.SetVisibility(false);
                        GameObject.Destroy(webViewObject);
                    }
                },
                err: (msg) => Debug.LogError("WebView error: " + msg),
                started: (msg) => Debug.Log("WebView load started"),
                ld: (msg) => Debug.Log("WebView load complete"),
                transparent: false
            );
            webViewObject.SetVisibility(false);
        }

        string playerUrl = $"https://orionplex.com.my/test1/player.html?prefab_name={prefabName}&video={videoNumber}";
        Debug.Log("Loading video URL: " + playerUrl);
        Debug.Log($"Attempting to load video {videoNumber} for prefab: {prefabName}");
        Debug.Log($"Final player URL: {playerUrl}");

        webViewObject.SetMargins(0, 0, 0, 0); // full screen for debug
        webViewObject.LoadURL(playerUrl);
        webViewObject.SetVisibility(true);
        videoScreen.SetActive(true);
    }

    [System.Serializable]
    public class PrefabContent
    {
        public string youtube_link_1;
        public string youtube_link_2;
        public string youtube_link_3;
        public string youtube_link_4;
        public string intro_text;
        public string use_cases;
    }
}
