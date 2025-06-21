using UnityEngine;

public class SlowRotator : MonoBehaviour
{
    public float rotationSpeed = 20f; // degrees per second

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

}
