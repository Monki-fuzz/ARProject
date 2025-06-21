using UnityEngine;

public class RotateAroundVisualCenter : MonoBehaviour
{
    public float rotationSpeed = 20f;

    private Vector3 modelCenter;

    void Start()
    {
        // Calculate the world-space center of the mesh bounds
        modelCenter = CalculateModelCenter();
    }

    void Update()
    {
        transform.RotateAround(modelCenter, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // Get the center of the Renderer bounds
    Vector3 CalculateModelCenter()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return transform.position;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        return bounds.center;
    }
}
