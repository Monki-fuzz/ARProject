using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;

    public GameObject editorTestPrefab; // add this to the class

    void Start()
    {
#if UNITY_EDITOR
    if (editorTestPrefab != null)
    {
        Vector3 testPosition = new Vector3(0, 0, 1f); // in front of camera
        Quaternion testRotation = Quaternion.identity;

        GameObject spawned = Instantiate(editorTestPrefab, testPosition, testRotation);
        spawned.tag = "modelObject";

        PrefabDataLoader loader = spawned.AddComponent<PrefabDataLoader>();
        loader.Initialize("ArduinoPrefab"); // match name in DB

        Debug.Log("Spawned test prefab in Editor.");
    }
#endif
    }


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

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("Tracked image name is null or empty. Skipping.");
            return;
        }

        if (!spawnedPrefabs.ContainsKey(imageName))
        {
            GameObject prefabToSpawn = prefabLibrary.Find(p => p.imageName == imageName).prefab;
            if (prefabToSpawn != null)
            {
                //GameObject spawned = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
                //spawned.transform.parent = trackedImage.transform;

                GameObject spawned = Instantiate(prefabToSpawn, trackedImage.transform);
                spawned.transform.localPosition = Vector3.zero;
                spawned.transform.localRotation = Quaternion.identity;
                spawned.transform.localScale = Vector3.one;

                spawned.tag = "modelObject";

                PrefabDataLoader loader = spawned.AddComponent<PrefabDataLoader>();
                loader.Initialize(imageName);

                spawnedPrefabs[imageName] = spawned;
            }
            else
            {
                Debug.LogWarning($"No prefab found in prefabLibrary for imageName: {imageName}");
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
