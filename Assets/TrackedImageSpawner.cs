using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;

    [System.Serializable]
    public struct TrackedPrefab
    {
        public string imageName;
        public GameObject prefab;
    }

    public List<TrackedPrefab> prefabLibrary = new List<TrackedPrefab>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            UpdatePrefab(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            UpdatePrefab(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in args.removed)
        {
            if (spawnedPrefabs.TryGetValue(trackedImage.referenceImage.name, out GameObject go))
            {
                Destroy(go);
                spawnedPrefabs.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    void UpdatePrefab(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!spawnedPrefabs.ContainsKey(imageName))
        {
            GameObject prefabToSpawn = prefabLibrary.Find(p => p.imageName == imageName).prefab;
            if (prefabToSpawn != null)
            {
                GameObject spawned = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
                spawned.transform.parent = trackedImage.transform;
                spawnedPrefabs[imageName] = spawned;
            }
        }
        else
        {
            spawnedPrefabs[imageName].transform.position = trackedImage.transform.position;
            spawnedPrefabs[imageName].transform.rotation = trackedImage.transform.rotation;
            spawnedPrefabs[imageName].SetActive(true);
        }
    }
}
