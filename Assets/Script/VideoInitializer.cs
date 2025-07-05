using UnityEngine;

public class VideoScreenLookAtCamera : MonoBehaviour
{
    [Tooltip("Assign the VideoScreen GameObject here (child of this object)")]
    public GameObject videoScreen;

    private Camera mainCamera;

    void Start()
    {
        if (videoScreen == null)
        {
            Debug.LogWarning("VideoScreen is not assigned in the inspector.");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found in scene.");
        }
    }

    void Update()
    {
        if (videoScreen != null && mainCamera != null)
        {
            Vector3 lookDirection = mainCamera.transform.position - videoScreen.transform.position;
            lookDirection.y = 0; // Lock vertical rotation

            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            videoScreen.transform.rotation = lookRotation * Quaternion.Euler(0, 180f, 0); // Rotate 180° around Y-axis
        }
    }

}
