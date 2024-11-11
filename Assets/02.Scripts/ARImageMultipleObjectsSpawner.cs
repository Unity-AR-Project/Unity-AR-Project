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

        //현재는 인스펙터창에서 프리팹을 관리하고 있지만 나중에는 코드로 찾아서 사용할 예정
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


    //나중에 trackedImagesChanged()함수를 ARTrackedImageManager의 OnTrackablesChanged함수로 수정 예정
    //유니티6에서는 trackedImagesChanged()함수를 안쓰고 ARTrackedImageManager의 OnTrackablesChanged함수를 사용하기 때문  
    private void OnEnable()
    {
        imgManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        imgManager.trackedImagesChanged -= OnImageChanged;
    }   

    //수정 예정
    //private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    //{
    //    // 새로 추가된 image
    //    foreach (var img in args.added)
    //    {
    //        UpdateSpawned(img);
    //        Debug.Log($"{img.name} 추가");
    //    }

    //    // 갱신된 image
    //    foreach (var img in args.updated)
    //    {
    //        UpdateSpawned(img);
    //    }

    //    // 제거된 image
    //    foreach (var img in args.removed)
    //    {
    //        // disable the prefab that has the same name than the image
    //        spawnedPrefabs[img.Value.name].SetActive(false);
    //    }
    //}


    /// <summary>
    /// 이미지가 갱신되면 새 장면 프리팹을 불러오는 함수
    /// </summary>
    /// <param name="args"></param>
    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage img in args.added)
        {
            UpdateSpawned(img);
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

    /// <summary>
    /// 장면 프리팹 교체 함수
    /// </summary>
    /// <param name="img"></param>
    private void UpdateSpawned(ARTrackedImage img)
    {
        string name = img.referenceImage.name;
        GameObject spawned = spawnedPrefabs[name];

        // 이미지 추적이 되고있는 상태일 때 프리팹 활성화 및 위치 설정
        if (img.trackingState == TrackingState.Tracking)
        {
            spawned.transform.position = img.transform.position;
            spawned.transform.rotation = img.transform.rotation;
            spawned.SetActive(true);
        }
        else
        {
            // 이미지 추적이 안되고 있는 상태일 때 프리팹 비활성화
            spawned.SetActive(false);
        }
    }
}