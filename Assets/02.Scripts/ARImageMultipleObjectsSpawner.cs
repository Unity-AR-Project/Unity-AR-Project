using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public struct PagePrefabs
{
    public string name; // 인식할 이미지의 이름
    public GameObject prefab; // 생성할 장면 프리팹
}

public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager imgManager;

    public PagePrefabs[] prefabs;
    //private PagePrefabs[] prefabs = new PagePrefabs[2];

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        imgManager = GetComponent<ARTrackedImageManager>();

        foreach (PagePrefabs prefab in prefabs)
        {
            // instantiate the prefab and store to the dictionary
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name;
            spawnedPrefabs.Add(instantiated.name, instantiated);
            instantiated.SetActive(false);
        }

        //prefabs[0].name = "PigBook";
        //prefabs[0].prefab = Resources.Load<GameObject>("Page_1");
        //prefabs[1].name = "Dog";
        //prefabs[1].prefab = Resources.Load<GameObject>("Page_2");
    }

    //private void Start()
    //{
    //    foreach (PagePrefabs prefab in prefabs)
    //    {
    //        // instantiate the prefab and store to the dictionary
    //        GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
    //        instantiated.name = prefab.name;
    //        spawnedPrefabs.Add(instantiated.name, instantiated);
    //        instantiated.SetActive(false);
    //    }
    //}

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        imgManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        imgManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage img in args.added)
        {
            UpdateSpawned(img);
            Debug.Log($"{img.name} 추가");
        }

        foreach (ARTrackedImage img in args.updated)
        {
            UpdateSpawned(img);
        }

        foreach (ARTrackedImage img in args.removed)
        {
            // disable the prefab that has the same name than the image
            spawnedPrefabs[img.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateSpawned(ARTrackedImage img)
    {
        string name = img.referenceImage.name;
        Debug.Log($"{img.referenceImage.name} 추가");
        GameObject spawned = spawnedPrefabs[name];

        Debug.Log("spawned " + spawned.name);


        // only update when tracking state is good
        if (img.trackingState == TrackingState.Tracking)
        {
            Debug.Log("Updating image " + img.referenceImage.name + " at " + img.transform.position);

            spawned.transform.position = img.transform.position;
            spawned.transform.rotation = img.transform.rotation;
            spawned.SetActive(true);
        }
        else
        {
            // poor or no tracking state
            spawned.SetActive(false);
        }
    }
}