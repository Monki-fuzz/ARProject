using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreenController : MonoBehaviour
{
    public Button ButtonVideo; // UI Button in Canvas

    private GameObject videoScreen;
    private VideoPlayer videoPlayer;

    private bool isVideoScreenVisible = false;

    void Start()
    {
        if (ButtonVideo != null)
        {
            ButtonVideo.onClick.AddListener(OnVideoButtonClicked);
            Debug.Log("ButtonVideo listener added.");
        }
        else
        {
            Debug.LogError("ButtonVideo is not assigned in the Inspector.");
        }
    }

    void OnVideoButtonClicked()
    {
        Debug.Log("Video button clicked.");

        if (videoScreen == null)
        {
            videoScreen = GameObject.FindGameObjectWithTag("VideoScreen");
            if (videoScreen == null)
            {
                Debug.LogWarning("VideoScreen not found.");
                return;
            }

            Debug.Log("VideoScreen found.");

            videoPlayer = videoScreen.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogWarning("VideoPlayer component not found on VideoScreen.");
            }
        }

        isVideoScreenVisible = !isVideoScreenVisible;

        if (isVideoScreenVisible)
        {
            videoScreen.SetActive(true);
            SetMaterialAlpha(videoScreen, 1f); // Make visible
            if (videoPlayer != null)
            {
                videoPlayer.Play();
                Debug.Log("VideoPlayer started.");
            }
        }
        else
        {
            if (videoPlayer != null)
            {
                videoPlayer.Pause();
                Debug.Log("VideoPlayer paused.");
            }
            SetMaterialAlpha(videoScreen, 0f); // Make transparent
            videoScreen.SetActive(false);
        }

        Debug.Log("VideoScreen visibility set to: " + isVideoScreenVisible);
    }

    void SetMaterialAlpha(GameObject target, float alpha)
    {
        Renderer renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }

}
